using System;
using System.Collections.Generic;
using System.IO;
using NBTEditorV2;
using System.Threading.Tasks;

namespace WorldEditor
{
    class Load
    {
        public static CompoundTag LoadLevel(string levelPath) {
            if (File.Exists(levelPath)){
                CompoundTag levelCompound = new CompoundTag("Level", new List<Tag>());

                byte[] bytes = Compression.GZip_Decompress(File.ReadAllBytes(levelPath));

                CompoundTag rootCompoundTag = CompoundTag.LoadTag(bytes);

                Tag compoundTag = rootCompoundTag.GetTag("Data");
                if (compoundTag != null){
                    levelCompound = (CompoundTag)(compoundTag.Item);
                }

                return levelCompound;
            }

            return null;
        }

        public static Region LoadRegion(string regionPath, int xIndex, int zIndex) {
            if (File.Exists(regionPath)) {
                Region region = new Region()
                {
                    XIndex = xIndex,
                    ZIndex = zIndex
                };

                byte[] regionBytes = File.ReadAllBytes(regionPath);

                if (regionBytes.Length <= 8192) {
                    return region;
                }

                int[] locations = new int[1024];
                int[] sizes = new int[1024];
                int[] timestamps = new int[1024];

                //Get Locations
                for (int i = 0; i < 4096; i += 4)
                {
                    byte[] offsetBytes = { regionBytes[i + 2], regionBytes[i + 1], regionBytes[i], 0 };

                    locations[i / 4] = BitConverter.ToInt32(offsetBytes, 0);
                    sizes[i / 4] = (int)regionBytes[i + 3] * 4096;
                }

                //Get Timestamps
                for (int i = 4096; i < 8192; i++)
                {
                    byte[] offsetBytes = {regionBytes[i + 2], regionBytes[i + 1], regionBytes[i], 0 };

                    timestamps[(i - 4096) / 4] = BitConverter.ToInt32(offsetBytes, 0);
                }

                //Get all non-empty chunks
                List<int> chunkLocations = new List<int>();
                for (int i = 0; i < 1024; i++) {
                    if (locations[i] > 0 && sizes[i] > 0) { //If not empty
                        chunkLocations.Add(locations[i]);
                    }
                }

                chunkLocations = Scripts.Shuffle(chunkLocations);

                int chunkCount = chunkLocations.Count;
                int numberOfThreads = chunkCount / 120;
                numberOfThreads = numberOfThreads < 1 ? 1 : numberOfThreads;

                //numberOfThreads = 1;

                Chunk[] allChunks = new Chunk[chunkCount];

                //DateTime time = DateTime.Now;

                //ParallelOptions options = new ParallelOptions();
                //System.Diagnostics.Debug.WriteLine(options.MaxDegreeOfParallelism);

                Parallel.For(0, numberOfThreads, i => {
                    int start = (chunkCount / numberOfThreads) * i;
                    int end = (chunkCount / numberOfThreads) * (i + 1);
                    if (i == numberOfThreads - 1) {
                        end = chunkCount;
                    }

                    for (int b = start; b < end; b++) {
                        int location = chunkLocations[b] * 4096;
                        byte[] lengthInBytes = { regionBytes[location + 3], regionBytes[location + 2], regionBytes[location + 1], regionBytes[location] };

                        int length = BitConverter.ToInt32(lengthInBytes, 0);
                        byte compressionType = regionBytes[location + 4];

                        byte[] chunkBytes = Scripts.Subset(regionBytes, location + 5, length - 1);

                        //Load
                        allChunks[b] = LoadChunk(chunkBytes, compressionType, true);
                        //allChunks[b] = null;
                    }
                });

                //System.Diagnostics.Debug.WriteLine((DateTime.Now - time).TotalSeconds + " seconds < region");

                //Set all chunks
                region.Chunks.AddRange(allChunks);

                return region;
            }

            return null;
        }
        private static Chunk LoadChunk(byte[] bytes, byte compressionType, bool decompress){
            Chunk chunk = new Chunk();

            //Decompress if asked
            if (decompress){
                //bytes = Compression.GZip_Decompress(bytes);

                if (compressionType == 1) {
                    bytes = Compression.GZip_Decompress(bytes);
                } else if (compressionType == 2) {
                    bytes = Compression.ZLib_Decompress(bytes);
                }
            }

            //Load chunk level in nbt editor
            CompoundTag rootCompound = CompoundTag.LoadTag(bytes);
            CompoundTag levelCompound = rootCompound.GetCompoundTag("Level");

            //Get data
            chunk.DataVersion = rootCompound.GetIntTag("DataVersion").Value;
            chunk.LastUpdate = levelCompound.GetLongTag("LastUpdate").Value;
            chunk.Status = levelCompound.GetStringTag("Status").Text;

            chunk.Xpos = levelCompound.GetIntTag("xPos").Value;
            chunk.Zpos = levelCompound.GetIntTag("zPos").Value;

            //Biomes
            int[] biomeArray = levelCompound.GetIntArrayTag("Biomes").Array;
            for (int i = 0; i < biomeArray.Length; i++) {
                chunk.Biomes[i % 16, i >> 4] = biomeArray[i];
            }

            //Sections
            ListTag sections = levelCompound.GetListTag("Sections");
            if (sections != null) {
                ListPayload payload = sections.Payload;

                for (int i = 0; i < payload.Elements.Count; i++) {
                    Section section = LoadSection((List<Tag>)payload.Elements[i]);
                    if (section != null) {
                        chunk.Sections.Add(section);
                    }
                }
            }

            //Remove tags
            rootCompound.Remove("DataVersion");
            rootCompound.Remove("LastUpdate");
            rootCompound.Remove("Status");
            levelCompound.Remove("xPos");
            levelCompound.Remove("zPos");
            levelCompound.Remove("Biomes");
            levelCompound.Remove("Sections");

            //Add level
            chunk.Level = levelCompound;

            return chunk;
        }
        private static Section LoadSection(List<Tag> compoundList)
        {
            CompoundTag compoundTag = new CompoundTag("", compoundList);

            //get yIndex
            int yIndex = compoundTag.GetByteTag("Y").Value;

            if (yIndex > 15 || yIndex < 0) {
                return null;
            }

            //Get palette
            ListTag palette = compoundTag.GetListTag("Palette");

            if (palette == null) {
                return new Section() { YIndex = yIndex };
            }

            Block[] paletteList = Palette.FromListTag(palette).Blocks;

            //Set blockstate & properties
            short[] blockID = new short[4096];
            Dictionary<short, List<Property>> properties = new Dictionary<short, List<Property>>();

            int length = Scripts.GetBitLength(palette.Payload.Elements.Count);

            long[] longs = compoundTag.GetLongArrayTag("BlockStates").Array;

            int reminder = 64 - length;
            int blockIndex = 0;
            int carryOver = 0;
            long carryValue = 0;

            for (int i = 0; i < longs.Length; i++) {
                long value = longs[i];
                int startPos = 64;

                //Reminder end
                if (carryOver > 0) {
                    int left = length - carryOver;
                    int bitsIn = startPos - left;

                    long secondValue = (long)((ulong)(value << bitsIn) >> (64 - left));
                    long index = (secondValue << carryOver) + carryValue;

                    //Set block
                    Block block = paletteList[(int)index];
                    blockID[blockIndex] = block.ID;

                    List<Property> blockProp = block.Properties;
                    if (blockProp.Count > 0) {
                        properties.Add((short)blockIndex, blockProp);
                    }


                    blockIndex++;
                    startPos = bitsIn;
                }

                //Regular
                int nIndexes = startPos / length;
                for (int b = 0; b < nIndexes; b++) {
                    startPos -= length;

                    int index = (int)((ulong)(value << startPos) >> reminder);

                    //Set block
                    Block block = paletteList[index];
                    blockID[blockIndex] = block.ID;

                    List<Property> blockProp = block.Properties;
                    if (blockProp.Count > 0) {
                        properties.Add((short)blockIndex, blockProp);
                    }


                    blockIndex++;
                }

                //Reminder start
                carryOver = startPos;
                if (carryOver > 0) {
                    carryValue = (long)((ulong)value >> 64 - startPos);
                }
            }

            Section section = new Section(yIndex, blockID, properties, false);

            return section;
        }
    }
}

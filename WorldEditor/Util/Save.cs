using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using NBTEditorV2;

namespace WorldEditor
{
    class Save
    {
        public static void SaveLevel(string levelPath, CompoundTag level)
        {
            CompoundTag compoundTag = new CompoundTag("", new List<Tag>());
            compoundTag.Add(level);

            byte[] bytes = CompoundTag.GetTagBytes(compoundTag).ToArray();

            //Check if exists
            if (File.Exists(levelPath)){
                File.WriteAllBytes(levelPath, bytes);
            }else{
                File.Create(levelPath);
                File.WriteAllBytes(levelPath, bytes);
            }
        }

        public static void SaveRegion(string regionPath, Region region) {
            List<Chunk> regionChunks = new List<Chunk>(region.Chunks);
            regionChunks = Scripts.Shuffle(regionChunks);

            byte[] locationTable = new byte[4096];
            byte[] timestampTable = new byte[4096];
            byte[][] chunkByteArray = new byte[regionChunks.Count][];

            //Set chunk bytes
            int chunkCount = regionChunks.Count;
            int numberOfThreads = chunkCount / 100;
            numberOfThreads = numberOfThreads < 1 ? 1 : numberOfThreads;

            Parallel.For(0, numberOfThreads, (i, state) => {
                int start = (chunkCount / numberOfThreads) * i;
                int end = (chunkCount / numberOfThreads) * (i + 1);
                if (i == numberOfThreads - 1) {
                    end = chunkCount;
                }

                for (int b = start; b < end; b++) {
                    chunkByteArray[b] = SaveChunk(regionChunks[b]);
                }
            });

            //Set tables
            int offsetIndex = 2;
            for (int i = 0; i < regionChunks.Count; i++){
                Chunk chunk = regionChunks[i];

                //Add chunk bytes
                byte[] chunkInBytes = chunkByteArray[i];

                //Set location table
                int locationIndex = (Scripts.Mod(chunk.Xpos, 32) + Scripts.Mod(chunk.Zpos, 32) * 32) * 4;
                byte[] offsetBytes = BitConverter.GetBytes(offsetIndex);

                locationTable[locationIndex] = offsetBytes[2];
                locationTable[locationIndex + 1] = offsetBytes[1];
                locationTable[locationIndex + 2] = offsetBytes[0];
                locationTable[locationIndex + 3] = (byte)(chunkInBytes.Length / 4096);

                offsetIndex += (chunkInBytes.Length / 4096);
                //Set timestamp table
                int timestampIndex = (Scripts.Mod(chunk.Xpos, 32) + Scripts.Mod(chunk.Zpos, 32) * 32) * 4;

                byte[] timestampBytes = BitConverter.GetBytes(regionChunks[i].LastUpdate);

                timestampTable[timestampIndex] = timestampBytes[4];
                timestampTable[timestampIndex + 1] = timestampBytes[3];
                timestampTable[timestampIndex + 2] = timestampBytes[2];
                timestampTable[timestampIndex + 3] = timestampBytes[1];
            }

            //Add to listbytes
            List<byte> listBytes = new List<byte>();

            listBytes.AddRange(locationTable);
            listBytes.AddRange(timestampTable);
            for (int i = 0; i < chunkByteArray.GetLength(0); i++){
                listBytes.AddRange(chunkByteArray[i]);
            }

            byte[] bytes = listBytes.ToArray();

            //Test
            string fileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\region\r." + region.XIndex + "." + region.ZIndex + ".mca";
            File.WriteAllBytes(fileName, bytes);
        }
        private static byte[] SaveChunk(Chunk chunk)
        {
            List<byte> bytes = new List<byte>();

            CompoundTag levelCompound = new CompoundTag("Level", new List<Tag>(chunk.Level.GetTags()));

            //Get sections
            ListPayload sectionsPayload = new ListPayload(TagType.Compound, new List<object>());

            for (int i = 0; i < chunk.Sections.Count; i++) {
                sectionsPayload.Elements.Add(SaveSection(chunk.Sections[i]));
            }

            levelCompound.Add(new ListTag("Sections", sectionsPayload));

            //Get biomes
            int[] biomeArray = new int[256];

            for (int z = 0; z < 16; z++) {
                for (int x = 0; x < 16; x++) {
                    biomeArray[(z * 16) + x] = chunk.Biomes[x, z];
                }
            }
            levelCompound.Add(new IntArrayTag("Biomes", biomeArray));

            //Get attributes
            levelCompound.Add(new LongTag("LastUpdate", chunk.LastUpdate));
            levelCompound.Add(new StringTag("Status", chunk.Status));
            levelCompound.Add(new IntTag("xPos", chunk.Xpos));
            levelCompound.Add(new IntTag("zPos", chunk.Zpos));

            CompoundTag rootCompound = new CompoundTag("", new List<Tag>());
            rootCompound.Add(levelCompound);
            rootCompound.Add(new IntTag("DataVersion", chunk.DataVersion));

            //Get bytes and compress            
            byte[] compoundBytes = Compression.ZLib_Compress(CompoundTag.GetTagBytes(rootCompound).ToArray());

            byte[] lengthBytes = BitConverter.GetBytes(compoundBytes.Length + 1);
            if (BitConverter.IsLittleEndian) { Array.Reverse(lengthBytes); }

            bytes.AddRange(lengthBytes);
            bytes.Add(2);
            bytes.AddRange(compoundBytes);
            bytes.AddRange(new byte[4096 - (bytes.Count % 4096)]);

            return bytes.ToArray();
        }
        private static List<Tag> SaveSection(Section section)
        {
            CompoundTag compoundTag = new CompoundTag();

            //Add Y
            compoundTag.Add(new ByteTag("Y", (byte)section.YIndex));

            if (section.Empty) {
                return compoundTag.GetTags();
            }

            //Add palette
            int[] blockIndexes = new int[4096];

            Palette palette = Palette.FromSection(section, ref blockIndexes);

            compoundTag.Add(palette.GetListTag());

            //Get blockstates
            int length = Scripts.GetBitLength(palette.Blocks.Length);
            long[] longArray = new long[4096 * length / 64];

            int index = 0;
            int carryOver = 0;
            int moveSecondValue = 32 - length;
            for (int i = 0; i < longArray.Length; i++) {
                int startPos = 64;
                long value = 0;

                //Reminder end
                if (carryOver > 0) {
                    int left = length - carryOver;
                    int bitsIn = startPos - left;

                    value = (int)((uint)(blockIndexes[index] << moveSecondValue) >> (32 - left));

                    index++;
                    startPos = bitsIn;
                }

                //Regular
                int nIndexes = startPos / length;
                for (int b = 0; b < nIndexes; b++) {
                    value += (long)blockIndexes[index] << (64 - startPos);

                    startPos -= length;
                    index++;
                }

                //Reminder start
                carryOver = startPos;
                if (carryOver > 0) {
                    int reminder = 64 - startPos;
                    int firstValue = (int)((uint)(blockIndexes[index] << reminder) >> (reminder));

                    value += (long)firstValue << reminder;
                }

                //Set the value
                longArray[i] = value;
            }

            compoundTag.Add(new LongArrayTag("BlockStates", longArray));
            
            return compoundTag.GetTags();
        }
    }
}

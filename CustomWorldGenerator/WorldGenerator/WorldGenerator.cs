using System;
using System.Collections.Generic;
using System.Linq;
using WorldEditor;
using System.Threading;

namespace CustomWorldGenerator
{
    public class WorldGenerator
    {
        private static Random Random = new Random();

        public static World Generate(GenerationProfile profile) {
            World world = new World();

            //Get noise map
            Tuple<byte[,], int[,]> noiseMap = GenerateNoiseMap(profile);

            byte[,] heightmap = noiseMap.Item1;
            int[,] biomemap = noiseMap.Item2;

            //Form
            world = LayStone(heightmap);
            world = FillWater(world);
            world = LayGrass(world);

            //Set biome
            world = SetBiome(world, biomemap);

            return world;
        }

        //Get heightmap and biomes
        private static Tuple<byte[,], int[,]> GenerateNoiseMap(GenerationProfile profile){
            byte[,] heightmap = GetHeightMap(profile);
            int[,] biomemap = GetBiomeMap(profile);

            return Tuple.Create(heightmap, biomemap);
        }

        public static byte[,] GetHeightMap(GenerationProfile profile) {
            //Turn octaves to noise
            double[,] noiseMap = PerlinNoise2DGenerator.GenerateNoise(profile.PerlinNoiseProfile);

            //Turn noise to bytes
            byte[,] heightmap = new byte[profile.Width, profile.Height];

            for (int z = 0; z < heightmap.GetLength(0); z++){
                for (int x = 0; x < heightmap.GetLength(1); x++){
                    double noise = noiseMap[x, z];

                    noise = noise * 128;
                    noise = (noise / 2) + 22;

                    heightmap[x, z] = (byte)noise;
                }
            }

            return heightmap;
        }
        public static int[,] GetBiomeMap(GenerationProfile profile) {
            int[,] biomemap = new int[profile.Width, profile.Height];

            int biomeID = ID.BiomeIDReverse["minecraft:plains"];
            for (int z = 0; z < biomemap.GetLength(0); z++){
                for (int x = 0; x < biomemap.GetLength(1); x++){
                    biomemap[x, z] = biomeID;
                }
            }

            return biomemap;
        }
        public static int[,] GetBiomeMap1(GenerationProfile profile) {
            int[,] mapID = new int[profile.Width, profile.Height];

            List<Biome> biomeList = new List<Biome>() {
                new Biome("desert", 3, new Range(75, 100), new Range(0, 25)),
                new Biome("plains", 0, new Range(75, 100), new Range(25, 75)),
                new Biome("forest", 1, new Range(75, 100), new Range(75, 100)),
                new Biome("mountains", 2, new Range(25, 75), new Range(0, 25)),
                new Biome("forest", 1, new Range(50, 75), new Range(25, 50)),
                new Biome("snowy_forest", 5, new Range(25, 50), new Range(25, 50)),
                new Biome("plains", 0, new Range(50, 75), new Range(50, 75)),
                new Biome("snowy_tundra", 4, new Range(0, 25), new Range(0, 25)),
                new Biome("ocean", 6, new Range(0, 25), new Range(25, 100)),
                new Biome("ocean", 6, new Range(25, 50), new Range(50, 100)),
                new Biome("ocean", 6, new Range(50, 75), new Range(75, 100)),
            };

            double[,] temperatureMap = PerlinNoise2DGenerator.GetNoiseGrid(profile.Width, profile.Height, 2, 1, profile.Seed + 10);
            double[,] humidityMap = PerlinNoise2DGenerator.GetNoiseGrid(profile.Width, profile.Height, 2, 1, profile.Seed + 11);

            for (int z = 0; z < profile.Width; z++) {
                for (int x = 0; x < profile.Height; x++) {
                    temperatureMap[x, z] *= 50;
                    humidityMap[x, z] *= 50;
                }
            }

            for (int z = 0; z < profile.Width; z++) {
                for (int x = 0; x < profile.Height; x++) {
                    Biome biome = GetBiome(biomeList, temperatureMap[x, z], humidityMap[x, z]);

                    if (biome != null) {
                        mapID[x, z] = biome.ID;
                    }
                }
            }

            return mapID;
        }

        //Lay Stone
        private static World LayStone(byte[,] heightmap) {
            World world = new World();

            int xRegions = (int)Math.Ceiling(heightmap.GetLength(1) / 512.0);
            int zRegions = (int)Math.Ceiling(heightmap.GetLength(0) / 512.0);

            int xIndex = 0;
            int zIndex = 0;
            for (int z = 0; z < zRegions; z++) {
                for (int x = 0; x < xRegions; x++) {
                    int endX = xIndex + 512;
                    endX = endX > heightmap.GetLength(1) ? heightmap.GetLength(1) : endX;

                    int endZ = zIndex + 512;
                    endZ = endZ > heightmap.GetLength(0) ? heightmap.GetLength(0) : endZ;

                    byte[,] regionHeightMap = Subset2D(heightmap, xIndex, zIndex, endX, endZ);
                    world.Regions.Add(LayStoneToRegion(x, z, regionHeightMap));

                    xIndex = x == xRegions - 1 ? 0 : endX;
                }

                zIndex = (z + 1) * 512;
            }

            return world;
        }
        private static Region LayStoneToRegion(int xIndex, int zIndex, byte[,] heightmap) {
            Region region = new Region() {
                XIndex = xIndex,
                ZIndex = zIndex
            };

            int xChunks = (int)Math.Ceiling(heightmap.GetLength(1) / 16.0);
            int zChunks = (int)Math.Ceiling(heightmap.GetLength(0) / 16.0);

            //Chunk[,] chunks = new Chunk[xChunks, zChunks];

            //Thread[] threadArray = new Thread[Environment.ProcessorCount];
            //for (int i = 0; i < threadArray.Length; i++){
            //    int chunkStart = (xChunks / threadArray.Length) * i;
            //    int chunkEnd = (xChunks / threadArray.Length) * (i + 1);
            //    if (i == threadArray.Length - 1){
            //        chunkEnd = xChunks;
            //    }

            //    int i1 = i;
            //    threadArray[i1] = new Thread(() =>
            //    {
            //        int xStart = chunkStart * 16;
            //        int zStart = 0;
            //        for (int z = 0; z < zChunks; z++) {
            //            for (int x = chunkStart; x < chunkEnd; x++){
            //                int endX = xStart + 16;
            //                endX = endX > heightmap.GetLength(1) ? heightmap.GetLength(1) : endX;

            //                int endZ = zStart + 16;
            //                endZ = endZ > heightmap.GetLength(0) ? heightmap.GetLength(0) : endZ;

            //                byte[,] chunkHeightMap = Subset2D(heightmap, xStart, zStart, endX, endZ);
            //                chunks[x, z] = LayStoneToChunk(x, z, chunkHeightMap);

            //                xStart = x == chunkEnd - 1 ? 0 : endX;
            //            }

            //            zStart = (z + 1) * 16;
            //        }
            //    });
            //    threadArray[i1].Start();
            //}
            //for (int i = 0; i < threadArray.Length; i++){
            //    threadArray[i].Join();
            //}

            ////Add chunks
            //for (int z = 0; z < zChunks; z++) {
            //    for (int x = 0; x < xChunks; x++) {
            //        region.Chunks.Add(chunks[x, z]);
            //    }
            //}

            int xStart = 0;
            int zStart = 0;
            for (int z = 0; z < zChunks; z++){
                for (int x = 0; x < xChunks; x++){

                    int endX = xStart + 16;
                    endX = endX > heightmap.GetLength(1) ? heightmap.GetLength(1) : endX;

                    int endZ = zStart + 16;
                    endZ = endZ > heightmap.GetLength(0) ? heightmap.GetLength(0) : endZ;

                    byte[,] chunkHeightMap = Subset2D(heightmap, xStart, zStart, endX, endZ);
                    region.Chunks.Add(LayStoneToChunk(x, z, chunkHeightMap));

                    xStart = x == xChunks - 1 ? 0 : endX;
                }

                zStart = (z + 1) * 16;
            }

            return region;
        }
        private static Chunk LayStoneToChunk(int xPos, int zPos, byte[,] heightmap) {
            Chunk chunk = new Chunk() {
                Xpos = xPos,
                Zpos = zPos,
                Status = "full",
                DataVersion = 1976
            };

            for (int y = 0; y < 16; y++) {
                Section section = LayStoneToSection(y, heightmap);
                if (section != null) {
                    chunk.Sections.Add(section);
                }
            }

            return chunk;
        }
        private static Section LayStoneToSection(int yPos, byte[,] heightmap) {
            Section section = new Section() {
                YIndex = yPos,
                Empty = false
            };

            for (int z = 0; z < 16; z++) {
                for (int x = 0; x < 16; x++) {
                    int blocksToLay = heightmap[x, z] - (yPos * 16);
                    blocksToLay = blocksToLay > 16 ? 16 : blocksToLay;

                    for (int y = 0; y < blocksToLay; y++) {
                        section.SetBlockID(ID.BlockIDReverse["minecraft:stone"], x, y, z);
                    }
                }
            }

            if (SectionIsEmpty(section)) {
                return null;
            }

            return section;
        }

        //Fill Water
        private static World FillWater(World world) {
            for (int i = 0; i < world.Regions.Count; i++) {
                world.Regions[i] = FillWaterRegion(world.Regions[i]);
            }

            return world;
        }
        private static Region FillWaterRegion(Region region) {
            for (int i = 0; i < region.Chunks.Count; i++) {
                region.Chunks[i] = FillWaterChunk(region.Chunks[i]);
            }

            return region;
        }
        private static Chunk FillWaterChunk(Chunk chunk) {
            //Check for missing sections
            for (int y = 0; y < 4; y++) {
                bool exists = false;
                for (int b = 0; b < chunk.Sections.Count; b++) {
                    if (chunk.Sections[b].YIndex == y) {
                        exists = true;
                        b = chunk.Sections.Count;
                    }
                }

                if (!exists) {
                    Section section = new Section() {
                        YIndex = y,
                        Empty = false
                    };
                    chunk.Sections.Add(section);
                }
            }

            for (int i = 0; i < chunk.Sections.Count; i++) {
                if (chunk.Sections[i].YIndex < 4) {
                    chunk.Sections[i] = FilLWaterSection(chunk.Sections[i]);
                }
            }

            return chunk;
        }
        private static Section FilLWaterSection(Section section) {
            for (int y = 0; y < 16; y++) {
                for (int z = 0; z < 16; z++) {
                    for (int x = 0; x < 16; x++) {
                        int height = section.YIndex * 16 + y;

                        if (ID.BlockID[section.GetBlockID(x, y, z)] == "minecraft:air" && height < 63) {
                            section.SetBlockID(ID.BlockIDReverse["minecraft:water"], x, y, z);
                        }
                    }
                }
            }

            return section;
        }

        //Lay Grass
        private static World LayGrass(World world) {
            for (int i = 0; i < world.Regions.Count; i++){
                world.Regions[i] = LayGrassRegion(world.Regions[i]);
            }

            return world;
        }
        private static Region LayGrassRegion(Region region) {
            for (int i = 0; i < region.Chunks.Count; i++){
                region.Chunks[i] = LayGrassChunk(region.Chunks[i]);
            }

            return region;
        }
        private static Chunk LayGrassChunk(Chunk chunk) {
            List<Section> sectionsInOrder = chunk.Sections.OrderBy(o => o.YIndex).ToList();

            short airID =   ID.BlockIDReverse["minecraft:air"];
            short waterID = ID.BlockIDReverse["minecraft:water"];
            short stoneID = ID.BlockIDReverse["minecraft:stone"];
            short grassBlockID = ID.BlockIDReverse["minecraft:grass_block"];

            for (int z = 0; z < 16; z++){
                for (int x = 0; x < 16; x++) {
                    for (int sectionIndex = sectionsInOrder.Count - 1; sectionIndex > -1; sectionIndex--) {
                        for (int y = 15; y > -1; y--) {
                            short blockID = sectionsInOrder[sectionIndex].GetBlockID(x, y, z);

                            if (blockID != airID) {
                                if (blockID == stoneID){
                                    //Set grass block
                                    int height = sectionsInOrder[sectionIndex].YIndex * 16 + y;

                                    sectionsInOrder[sectionIndex].SetBlockID(grassBlockID, x, y, z);

                                    //Add grass
                                    int sectionOrderIndex = sectionIndex;
                                    int blockIndex = y + 1;

                                    if (y < 15){
                                        if (Random.Next(6) == 0){
                                            sectionsInOrder[sectionOrderIndex].SetBlockID(GetGardeningID(), x, blockIndex, z);
                                        }
                                    }
                                }

                                y = 0;
                                sectionIndex = 0;
                            }
                        }
                    }
                }
            }

            return chunk;
        }

        //Set Biome
        private static World SetBiome(World world, int[,] biomes) {
            for (int i = 0; i < world.Regions.Count; i++) {
                for (int b = 0; b < world.Regions[i].Chunks.Count; b++) {
                    int xLocation = (world.Regions[i].XIndex * 512) + (world.Regions[i].Chunks[b].Xpos * 16);
                    int zLocation = (world.Regions[i].ZIndex * 512) + (world.Regions[i].Chunks[b].Zpos * 16);

                    for (int z = 0; z < 16; z++) {
                        for (int x = 0; x < 16; x++) {
                            world.Regions[i].Chunks[b].Biomes[x, z] = biomes[xLocation + x, zLocation + z];
                        }
                    }
                }
            }

            return world;
        }

        //Scripts
        private static Biome GetBiome(List<Biome> biomeList, double temperature, double humidity) {
            for (int i = 0; i < biomeList.Count; i++) {
                bool tempWithinRange = false;
                bool humidityWithinRange = false;

                //Check if temperature is outside the range
                if (temperature >= biomeList[i].TemperatureRange.Start && temperature <= biomeList[i].TemperatureRange.End) {
                    tempWithinRange = true;
                }

                //Check if humidity is outside the range
                if (humidity >= biomeList[i].HumidityRange.Start && humidity <= biomeList[i].HumidityRange.End){
                    humidityWithinRange = true;
                }

                if (tempWithinRange && humidityWithinRange) {
                    return biomeList[i];
                }
            }

            return null;
        }

        private static byte[,] Subset2D(byte[,] bytes, int point1X, int point1Z, int point2X, int point2Z) {
            byte[,] array = new byte[point2X - point1X, point2Z - point1Z];

            for (int z = 0; z < array.GetLength(0); z++) {
                for (int x = 0; x < array.GetLength(1); x++) {
                    array[x, z] = bytes[x + point1X, z + point1Z];
                }
            }

            return array;
        }
        private static bool SectionIsEmpty(Section section) {
            short airID = ID.BlockIDReverse["minecraft:air"];

            for (int y = 0; y < 16; y++) {
                for (int z = 0; z < 16; z++) {
                    for (int x = 0; x < 16; x++) {
                        if (section.GetBlockID(x, y, z) != airID) {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static short[] gardenIDs = {ID.BlockIDReverse["minecraft:grass"], ID.BlockIDReverse["minecraft:poppy"],
                                        ID.BlockIDReverse["minecraft:dandelion"], ID.BlockIDReverse["minecraft:fern"]};
        private static short GetGardeningID() {
            return gardenIDs[Random.Next(gardenIDs.Length)];
        }
    }
}

using System;
using System.Collections.Generic;
using WorldEditor;

namespace Schematics.Util{
    class Extract {
        public static Schematic ExtractSchematic(World world, Cords cords, Size size)
        {
            Schematic schematic = new Schematic(size.Width, size.Height, size.Length);

            List<RegionIndex> regionIndexes = Scripts.GetAllRegionIndexes(cords.X, cords.Z, size.Width, size.Length);

            for (int i = 0; i < regionIndexes.Count; i++) {
                Region region = world.GetRegionByIndex(regionIndexes[i].X, regionIndexes[i].Z);
                if (region == null) {
                    region = Region.LoadRegion(world.Path, regionIndexes[i].X, regionIndexes[i].Z);
                }

                if (region != null) {
                    ExtractRegion(region, regionIndexes[i], cords, schematic);

                    world.Regions.Add(region);
                }
            }

            return schematic;
        }
        private static void ExtractRegion(Region region, RegionIndex regionIndexes, Cords cords, Schematic schematic)
        {
            for (int i = 0; i < regionIndexes.ChunkIndexes.Count; i++) {
                Chunk chunk = region.GetChunk(regionIndexes.ChunkIndexes[i].X, regionIndexes.ChunkIndexes[i].Z);

                if (chunk != null) {
                    Index minIndex = new Index(chunk.Xpos * 16, chunk.Zpos * 16);

                    int currentX = minIndex.X < cords.X ? 0 : minIndex.X - cords.X;
                    int currentZ = minIndex.Z < cords.Z ? 0 : minIndex.Z - cords.Z;

                    ExtractChunk(chunk, cords, new Index(currentX, currentZ), schematic);
                }
            }
        }
        private static void ExtractChunk(Chunk chunk, Cords cords, Index index, Schematic schematic)
        {
            Range range = Scripts.GetChunkRange(cords.X, cords.Z, index, schematic.Size);

            int[] heightIndexes = Scripts.GetHeightIndexes(cords.Y, cords.Y + schematic.Size.Height);
            for (int b = 0; b < heightIndexes.Length; b++) {
                Section section = chunk.GetSection(heightIndexes[b]);

                if (section == null) {
                    section = new Section() { YIndex = heightIndexes[b] };
                }

                int[] yRange = Scripts.GetVerticalRange(heightIndexes[b], cords.Y, cords.Y + schematic.Size.Height);
                int currentY = section.YIndex * 16 + yRange[0];

                for (int y = yRange[0]; y < yRange[1]; y++) {
                    for (int z = range.StartZ; z < range.EndZ; z++) {
                        for (int x = range.StartX; x < range.EndX; x++) {
                            short id = section.GetBlockID(x, y, z);
                            List<Property> properties = section.GetProperties(x, y, z);

                            schematic.SetBlock(
                                index.X + (x - range.StartX),
                                currentY + (y - yRange[0]) - cords.Y,
                                index.Z + (z - range.StartZ),
                                id,
                                properties
                            );
                        }
                    }
                }
            }
        }
    }
}

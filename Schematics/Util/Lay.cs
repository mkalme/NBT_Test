using System;
using System.Collections.Generic;
using WorldEditor;

namespace Schematics.Util{
    class Lay {
        public static void LaySchematic(World world, Schematic schematic, int x, int y, int z)
        {
            List<RegionIndex> regionIndexes = Scripts.GetAllRegionIndexes(x, z, schematic.Size.Width, schematic.Size.Length);

            for (int i = 0; i < regionIndexes.Count; i++) {
                Region region = world.GetRegionByIndex(regionIndexes[i].X, regionIndexes[i].Z);
                if (region == null) {
                    region = Region.LoadRegion(world.Path, regionIndexes[i].X, regionIndexes[i].Z);
                }

                if (region != null) {
                    LayRegion(region, schematic, regionIndexes[i], x, y, z);

                    world.Regions.Add(region);
                }
            }
        }
        private static void LayRegion(Region region, Schematic schematic, RegionIndex regionIndexes, int x, int y, int z)
        {
            for (int i = 0; i < regionIndexes.ChunkIndexes.Count; i++) {
                Chunk chunk = region.GetChunk(regionIndexes.ChunkIndexes[i].X, regionIndexes.ChunkIndexes[i].Z);

                if (chunk != null) {
                    Index minIndex = new Index(chunk.Xpos * 16, chunk.Zpos * 16);

                    int currentX = minIndex.X < x ? 0 : minIndex.X - x;
                    int currentZ = minIndex.Z < z ? 0 : minIndex.Z - z;

                    LayChunk(chunk, schematic, x, y, z, new Index(currentX, currentZ));
                }
            }
        }
        private static void LayChunk(Chunk chunk, Schematic schematic, int xp, int yp, int zp, Index index)
        {
            Range range = Scripts.GetChunkRange(xp, zp, index, schematic.Size);

            int[] heightIndexes = Scripts.GetHeightIndexes(yp, yp + schematic.Size.Height);
            for (int b = 0; b < heightIndexes.Length; b++) {
                Section section = chunk.GetSection(heightIndexes[b]);

                if (section == null) {
                    section = new Section() { YIndex = heightIndexes[b] };
                    chunk.Sections.Add(section);
                }

                int[] yRange = Scripts.GetVerticalRange(heightIndexes[b], yp, yp + schematic.Size.Height);
                int currentY = section.YIndex * 16 + yRange[0];

                for (int y = yRange[0]; y < yRange[1]; y++) {
                    for (int z = range.StartZ; z < range.EndZ; z++) {
                        for (int x = range.StartX; x < range.EndX; x++) {

                            int xIndex = index.X + (x - range.StartX);
                            int yIndex = currentY + (y - yRange[0]) - yp;
                            int zIndex = index.Z + (z - range.StartZ);

                            section.SetBlockID(schematic.GetID(xIndex, yIndex, zIndex), x, y, z);
                            section.SetProperties(x, y, z, schematic.GetProperties(xIndex, yIndex, zIndex));
                            section.Empty = false;
                        }
                    }
                }
            }
        }
    }
}

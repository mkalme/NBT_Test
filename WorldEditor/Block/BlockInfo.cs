using System;

namespace WorldEditor
{
    public class BlockInfo
    {
        public Block Block { get; set; }
        public int BiomeID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int SectionY { get; set; }

        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }

        public int RegionX { get; set; }
        public int RegionZ { get; set; }
    }
}

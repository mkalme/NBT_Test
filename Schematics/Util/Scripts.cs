using System;
using System.Collections.Generic;
using WorldEditor;

namespace Schematics {
    class Scripts {
        public static List<RegionIndex> GetAllRegionIndexes(int x, int z, int width, int length)
        {
            List<RegionIndex> regionIndexes = new List<RegionIndex>();

            int regionXMin = (int)Math.Floor(x / 512.0);
            int regionXMax = (int)Math.Ceiling((x + width) / 512.0);

            int regionZMin = (int)Math.Floor(z / 512.0);
            int regionZMax = (int)Math.Ceiling((z + length) / 512.0);

            for (int zRegion = regionZMin; zRegion < regionZMax; zRegion++) {
                for (int xRegion = regionXMin; xRegion < regionXMax; xRegion++) {
                    RegionIndex regionIndex = new RegionIndex() {
                        X = xRegion,
                        Z = zRegion
                    };

                    Range range = GetRange(new Index(xRegion, zRegion), new Range(x, z, x + width, z + length), 512);
                    regionIndex.ChunkIndexes = GetRegionChunkIndexes(range);

                    regionIndexes.Add(regionIndex);
                }
            }

            return regionIndexes;
        }
        public static List<Index> GetRegionChunkIndexes(Range range)
        {
            List<Index> chunkIndexes = new List<Index>();

            int chunkXMin = (int)Math.Floor(range.StartX / 16.0);
            int chunkXMax = (int)Math.Ceiling(range.EndX / 16.0);

            int chunkZMin = (int)Math.Floor(range.StartZ / 16.0);
            int chunkZMax = (int)Math.Ceiling(range.EndZ / 16.0);

            for (int zChunk = chunkZMin; zChunk < chunkZMax; zChunk++) {
                for (int xChunk = chunkXMin; xChunk < chunkXMax; xChunk++) {
                    chunkIndexes.Add(new Index(xChunk, zChunk));
                }
            }

            return chunkIndexes;
        }

        public static Range GetRange(Index regionIndex, Range range, int interval)
        {
            Index regionStart = new Index(regionIndex.X * interval, regionIndex.Z * interval);
            Index regionEnd = new Index((regionIndex.X + 1) * interval, (regionIndex.Z + 1) * interval);

            int startX = regionStart.X < range.StartX ? range.StartX : regionStart.Z;
            int startZ = regionStart.Z < range.StartZ ? range.StartZ : regionStart.Z;

            int endX = regionEnd.X > range.EndX ? range.EndX : regionEnd.X;
            int endZ = regionEnd.Z > range.EndZ ? range.EndZ : regionEnd.Z;

            return new Range(startX, startZ, endX, endZ);
        }
        public static Range GetChunkRange(int x, int z, Index index, Size size) {
            int xStart = Mod(x + index.X, 16);
            int zStart = Mod(z + index.Z, 16);

            Index maxIndex = new Index((16 - xStart) + index.X, (16 - zStart) + index.Z);

            int xEnd = maxIndex.X > size.Width ? xStart + (size.Width - index.X) : 16;
            int zEnd = maxIndex.Z > size.Length ? zStart + (size.Length - index.Z) : 16;

            return new Range(xStart, zStart, xEnd, zEnd);
        }

        public static int[] GetHeightIndexes(int y1, int y2)
        {
            int yMin = (int)Math.Floor(y1 / 16.0);
            int yMax = (int)Math.Ceiling(y2 / 16.0);

            int count = yMax - yMin;
            count = count < 0 ? 0 : count;

            int[] indexes = new int[count];
            for (int i = 0; i < count; i++) {
                indexes[i] = yMin + i;
            }

            return indexes;
        }
        public static int[] GetVerticalRange(int intervalIndex, int y1, int y2)
        {
            int regionStart = intervalIndex * 16;
            int regionEnd = (intervalIndex + 1) * 16;

            int yMin = y1 < regionStart ? regionStart : y1;
            int yMax = y2 > regionEnd ? regionEnd : y2;

            int yMinMod = Mod(yMin, 16);

            return new int[] { yMinMod, yMax - yMin + yMinMod };
        }

        public static int Mod(int number, int mod)
        {
            int result = number % mod;

            if (result < 0) {
                result += mod;
            }

            return result;
        }
        public static int CeilingMultiple(int value, int multiple) {
            int rem = value % multiple;
            
            int result = value - rem;
            if (rem > 0) {
                result += multiple;
            }

            return result;
        }
        public static void SetCordsAndSize(Cords cords1, Cords cords2, out Cords newCords, out Size newSize) {
            int x1 = cords1.X > cords2.X ? cords2.X : cords1.X;
            int y1 = cords1.Y > cords2.Y ? cords2.Y : cords1.Y;
            int z1 = cords1.Z > cords2.Z ? cords2.Z : cords1.Z;

            int x2 = cords1.X < cords2.X ? cords2.X : cords1.X;
            int y2 = cords1.Y < cords2.Y ? cords2.Y : cords1.Y;
            int z2 = cords1.Z < cords2.Z ? cords2.Z : cords1.Z;

            newCords = new Cords(x1, y1, z1);
            newSize = new Size(x2 - x1 + 1, y2 - y1 + 1, z2 - z1 + 1);
        }

        public static int PaletteIndex(short id, List<Property> properties, List<Block> palette)
        {
            if (palette.Count > 0) {
                for (int i = 0; i < palette.Count; i++) {
                    if (palette[i].Equals(id, properties)) {
                        return i;
                    }
                }
            }

            return -1;
        }

        public static int GetBitLength(int paletteCount)
        {
            int length = 4;
            if (paletteCount > 16) {
                //length = (int)Math.Ceiling(Math.Sqrt(paletteCount));

                while (Math.Pow(2, length) < paletteCount) {
                    length++;
                }
            }

            return length;
        }
    }

    public enum SchematicsRotate {
        RotateX90,
        RotateX180,
        RotateX270,
        RotateY90,
        RotateY180,
        RotateY270,
        RotateZ90,
        RotateZ180,
        RotateZ270,
    }
    public class Index {
        public int X { get; set; }
        public int Z { get; set; }

        public Index(int x, int z){
            this.X = x;
            this.Z = z;
        }
    }
    public class RegionIndex {
        public int X { get; set; }
        public int Z { get; set; }

        public List<Index> ChunkIndexes { get; set; }

        public RegionIndex(){
            this.X = 0;
            this.Z = 0;
            this.ChunkIndexes = new List<Index>();
        }
        public RegionIndex(int x, int z, List<Index> chunkIndexes){
            this.X = x;
            this.Z = z;
            this.ChunkIndexes = chunkIndexes;
        }
    }
    public class Range {
        public int StartX { get; set; }
        public int StartZ { get; set; }
        public int EndX { get; set; }
        public int EndZ { get; set; }

        public Range(){
            this.StartX = 0;
            this.StartZ = 0;
            this.EndX = 0;
            this.EndZ = 0;
        }
        public Range(int startX, int startZ, int endX, int endZ){
            this.StartX = startX;
            this.StartZ = startZ;
            this.EndX = endX;
            this.EndZ = endZ;
        }
    }
    public class Size {
        public int Width { get; set; } // X - Axis
        public int Height { get; set; } // Y - Axis
        public int Length { get; set; } // Z - Axis

        public Size(){
            this.Width = 0;
            this.Length = 0;
            this.Height = 0;
        }
        public Size(int width, int height, int length){
            this.Width = width;
            this.Length = length;
            this.Height = height;
        }
    }
    public class Cords { 
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Cords() {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }
        public Cords(int x, int y, int z){
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}

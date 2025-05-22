using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorldEditor;

namespace RegionMapping {
    public class WorldMap : Scripts {
        private string WorldPath { get; set; }
        private List<RegionIndex> RegionIndexes { get; set; }
        private MapRange MapRange { get; set; }
        private bool[,] RegionsExist { get; set; }

        private readonly string TempFolder = Path.GetTempPath() + @"\" + Path.GetRandomFileName();

        private Render Render { get; set; }

        private Range Range { get; set; }
        private bool RangeOn { get; set; }

        public WorldMap(string worldPath, RenderSettings settings) {
            this.WorldPath = worldPath;
            this.Render = new Render(settings);
        }
        public WorldMap(string worldPath, RenderSettings settings, Range range) {
            this.WorldPath = worldPath;
            this.Render = new Render(settings);
            this.Range = range;
            this.RangeOn = true;
        }

        public Bitmap GetMap() {
            RegionIndexes = GetAllRegionIndexes(WorldPath, Range, RangeOn);
            MapRange = MapRange.GetRange(RegionIndexes);
            RegionsExist = GetRegionsExists(MapRange, RegionIndexes);

            int mapWidth = (MapRange.ZEnd - MapRange.ZStart + 1) * 512;
            int mapHeight = (MapRange.XEnd - MapRange.XStart + 1) * 512;

            CreateAllTempRegions();

            Bitmap[] bitmaps = new Bitmap[RegionIndexes.Count];
            Thread[] threads = new Thread[10];

            TotalRegions = 0;
            int regionsInThread = RegionIndexes.Count / threads.Length;

            for (int i = 0; i < threads.Length; i++) {
                int start = regionsInThread * i;
                int end = regionsInThread * (i + 1);
                if (i == threads.Length - 1) {
                    end = RegionIndexes.Count;
                }

                threads[i] = new Thread(() => {
                    for (int b = start; b < end; b++) {
                        bitmaps[b] = GetRegionBitmap(RegionIndexes[b].X, RegionIndexes[b].Z, new Render(Render.Settings));

                        TotalRegions++;
                        Debug.WriteLine(TotalRegions + " / " + RegionIndexes.Count + " drawing");
                    }
                });
                threads[i].Start();
            }
            for (int i = 0; i < threads.Length; i++) {
                threads[i].Join();
            }

            Bitmap bitmap = JoinBitmaps(bitmaps, mapWidth, mapHeight);

            Directory.Delete(TempFolder, true);

            Debug.WriteLine("Done!");

            return bitmap;
        }

        //Create temp regions
        private int TotalRegions = 0;
        private void CreateAllTempRegions() {
            Directory.CreateDirectory(TempFolder);

            int threadCount = (int)Math.Ceiling(Environment.ProcessorCount / 1.3);

            int loops = (int)Math.Ceiling(RegionIndexes.Count / (double)threadCount);
            for (int i = 0; i < loops; i++) {
                int start = threadCount * i;
                int end = threadCount * (i + 1);
                if (i == loops - 1) {
                    end = RegionIndexes.Count;
                }

                //Load regions
                List<WorldEditor.Region> regions = new List<WorldEditor.Region>();
                for (int r = start; r < end; r++) {
                    WorldEditor.Region region = WorldEditor.Region.LoadRegion(WorldPath, RegionIndexes[r].X, RegionIndexes[r].Z);

                    if (region != null) {
                        regions.Add(region);
                    }
                }

                Parallel.For(0, regions.Count, b => {
                    CreateTempRegion(regions[b]);
                });
            }
        }
        private void CreateTempRegion(WorldEditor.Region region) {
            HeightMap heightMap;
            SetRegionHeightMaps(region, out heightMap);

            byte[] heightBytes = TempMap.GetHeightMapBytes(heightMap);

            File.WriteAllBytes(TempFolder + @"\" + region.XIndex + "." + region.ZIndex + ".TMP", heightBytes);

            TotalRegions++;
            Debug.WriteLine(TotalRegions + " / " + RegionIndexes.Count + " getting heightmaps");
        }

        //Set region heightmap
        private void SetRegionHeightMaps(WorldEditor.Region region, out HeightMap heightMap) {
            heightMap = new HeightMap(512, 512);

            //Initialize heightmap
            for (int z = 0; z < 512; z++) {
                for (int x = 0; x < 512; x++) {
                    heightMap.Columns[z, x] = new Column(ColumnType.Region);
                }
            }

            //Set heightmaps
            for (int c = 0; c < region.Chunks.Count; c++) {
                Chunk chunk = region.Chunks[c];
                List<Section> sectionsInOrder = chunk.Sections.OrderBy(o => o.YIndex).ToList();

                int chunkXpos = Mod(chunk.Xpos, 32);
                int chunkZpos = Mod(chunk.Zpos, 32);

                for (int z = 0; z < 16; z++) {
                    for (int x = 0; x < 16; x++) {
                        int xPixel = (chunkXpos * 16) + x;
                        int zPixel = (chunkZpos * 16) + z;

                        heightMap.Columns[zPixel, xPixel] = GetColumn(chunk, sectionsInOrder, x, z);
                    }
                }
            }
        }

        //Render
        private Bitmap GetRegionBitmap(int xPos, int zPos, Render render){
            HeightMap heightMap = GetTempRegionHeightMaps(xPos, zPos);

            return render.RenderMap(heightMap, 512, 512, 1024, 1024);
        }
        private HeightMap GetTempRegionHeightMaps(int xPos, int zPos){
            HeightMap[,] heightMaps = new HeightMap[3, 3];

            for (int zIndex = -1; zIndex < 2; zIndex++) {
                for (int xIndex = -1; xIndex < 2; xIndex++) {
                    HeightMap tempHeightMap = new HeightMap(512, 512);

                    int x = xPos + xIndex;
                    int z = zPos + zIndex;

                    int xCorrected = (xIndex - 1) * -1;
                    int zCorrected = zIndex + 1;

                    if (RegionExists(x, z) && RightIndex(xIndex, zIndex)) {
                        Range chunkRange = Range.GetRange(0, 0, xIndex, zIndex);
                        TempMap tempMapFile = TempMap.Load(x, z, TempFolder, chunkRange);

                        tempHeightMap = tempMapFile.GetHeightMap(chunkRange);
                    }

                    heightMaps[zCorrected, xCorrected] = tempHeightMap;
                }
            }

            HeightMap heightMap = new HeightMap(1536, 1536) {
                XStart = xPos,
                ZStart = zPos
            };

            //Overlay
            for (int z = 0; z < 3; z++) {
                for (int x = 0; x < 3; x++) {
                    OverlayHeightMap(heightMap, heightMaps[z, x], (2 - x) * 512, z * 512);
                }
            }

            return heightMap;
        }

        //Scripts
        private static List<RegionIndex> GetAllRegionIndexes(string path, Range range, bool rangeOn){
            string[] files = Directory.GetFiles(path + @"\region");
            List<RegionIndex> regionIndexes = new List<RegionIndex>();

            for (int i = 0; i < files.Length; i++) {
                string file = Path.GetFileNameWithoutExtension(files[i]);
                string fileName = file.Substring(2, file.Length - 2);

                string[] indexes = fileName.Split('.');

                int xIndex = Int32.Parse(indexes[0]);
                int zIndex = Int32.Parse(indexes[1]);

                if (rangeOn) {
                    if (range.WithinRange(xIndex, zIndex)) {
                        regionIndexes.Add(new RegionIndex(xIndex, zIndex));
                    }
                } else {
                    regionIndexes.Add(new RegionIndex(xIndex, zIndex));
                }
            }

            return regionIndexes;
        }
        private static bool[,] GetRegionsExists(MapRange mapRange, List<RegionIndex> regionIndexes){
            int mapWidth = mapRange.ZEnd - mapRange.ZStart + 1;
            int mapHeight = mapRange.XEnd - mapRange.XStart + 1;

            bool[,] regionsExist = new bool[mapWidth, mapHeight];

            for (int i = 0; i < regionIndexes.Count; i++) {
                int xIndex = mapRange.XEnd - regionIndexes[i].X;
                int zIndex = regionIndexes[i].Z - mapRange.ZStart;

                regionsExist[zIndex, xIndex] = true;
            }

            return regionsExist;
        }
        private bool RegionExists(int x, int z){
            int xIndex = MapRange.XEnd - x;
            int zIndex = z - MapRange.ZStart;

            if (xIndex < 0 || xIndex > RegionsExist.GetLength(1) - 1 || zIndex < 0 || zIndex > RegionsExist.GetLength(0) - 1) {
                return false;
            }

            return RegionsExist[zIndex, xIndex];
        }
        
        private static bool RightIndex(int x, int z){
            return (x == -1 && z == 0) || (x == 0 && z == 1) || (x == 1 && z == 0) || (x == 0 && z == -1) || (x == 0 && z == 0);
        }

        private static void OverlayHeightMap(HeightMap heightMap, HeightMap heightMap2, int x, int z){
            for (int zp = 0; zp < heightMap2.Width; zp++) {
                for (int xp = 0; xp < heightMap2.Height; xp++) {
                    heightMap.Columns[z + zp, x + xp] = heightMap2.Columns[zp, xp];
                }
            }
        }

        private Bitmap JoinBitmaps(Bitmap[] bitmaps, int width, int height) {
            Bitmap bitmap = new Bitmap(width / Render.Settings.Scale, height / Render.Settings.Scale, Render.Settings.PixelFormat);

            using (Graphics g = Graphics.FromImage(bitmap)) {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                for (int i = 0; i < bitmaps.Length; i++) {
                    int x = (MapRange.XEnd - RegionIndexes[i].X) * 512 / Render.Settings.Scale;
                    int z = (RegionIndexes[i].Z - MapRange.ZStart) * 512 / Render.Settings.Scale;

                    g.DrawImage(bitmaps[i], z, x, 512 / Render.Settings.Scale, 512 / Render.Settings.Scale);

                    bitmaps[i].Dispose();
                }
            }

            return bitmap;
        }

        public static List<T> ShuffleList<T>(List<T> list){
            List<T> randomizedList = new List<T>();
            Random rnd = new Random();

            while (list.Count > 0) {
                int index = rnd.Next(0, list.Count); //pick a random item from the master list

                randomizedList.Add(list[index]); //place it at the end of the randomized list

                list.RemoveAt(index);
            }

            return randomizedList;
        }
    }

    public struct RegionIndex {
        public int X { get; set; }
        public int Z { get; set; }

        public RegionIndex(int x, int z){
            this.X = x;
            this.Z = z;
        }
        public static List<RegionIndex> GetAllIndexes(string regionFolder) {
            string[] files = Directory.GetFiles(regionFolder);
            List<RegionIndex> regionIndexes = new List<RegionIndex>();

            for (int i = 0; i < files.Length; i++) {
                string file = Path.GetFileNameWithoutExtension(files[i]);
                string fileName = file.Substring(2, file.Length - 2);

                string[] indexes = fileName.Split('.');

                int xIndex = Int32.Parse(indexes[0]);
                int zIndex = Int32.Parse(indexes[1]);

                regionIndexes.Add(new RegionIndex(xIndex, zIndex));
            }

            return regionIndexes;
        }
    }
    public struct Range {
        public static readonly Range WholeRange = new Range(0, 0, 31, 31);
        public static readonly Range LeftRange = new Range(0, 0, 31, 0);
        public static readonly Range TopRange = new Range(31, 0, 31, 31);
        public static readonly Range RightRange = new Range(0, 31, 31, 31);
        public static readonly Range BottomRange = new Range(0, 0, 0, 31);

        public int X1 { get; set; }
        public int Z1 { get; set; }
        public int X2 { get; set; }
        public int Z2 { get; set; }

        public Range(int x1, int z1, int x2, int z2){
            this.X1 = x1;
            this.Z1 = z1;
            this.X2 = x2;
            this.Z2 = z2;
        }

        public bool WithinRange(int x, int z){
            return x >= X1 && x <= X2 && z >= Z1 && z <= Z2;
        }
        public static Range GetRange(int x1, int z1, int x2, int z2){
            if (x2 > x1 && z2 == x1) {
                return BottomRange;
            } else if (x2 == x1 && z2 > z1) {
                return LeftRange;
            } else if (x2 < x1 && z2 == z1) {
                return TopRange;
            } else if (x2 == x1 && z2 < z1) {
                return RightRange;
            } else {
                return WholeRange;
            }
        }
        public static Range FromString(string input) {
            string[] cords = input.Split(';');

            string[] cords1 = cords[0].Split(',');
            string[] cords2 = cords[1].Split(',');

            return new Range(
                int.Parse(cords1[0]),
                int.Parse(cords1[1]),
                int.Parse(cords2[0]),
                int.Parse(cords2[1]));
        }
    }
    public class TempMap {
        public byte[,][] Columns { get; set; }
        public int X { get; set; }
        public int Z { get; set; }

        public static Dictionary<byte, int> ColumnLengths = new Dictionary<byte, int>() {
            {0, 1 },
            {1, 1 },
            {2, 7 },
            {3, 12 }
        };

        //Load
        public static TempMap Load(int x, int z, string tempFolder, Range chunkRange) {
            TempMap tempMap = new TempMap() { Columns = new byte[512, 512][], X = x, Z = z };

            string filePath = tempFolder + @"\" + x + "." + z + ".TMP";

            if (File.Exists(filePath)) {
                byte[] bytes = File.ReadAllBytes(filePath);

                int index = 0;
                int loop = 0;
                while (index < bytes.Length) {
                    int xIndex = loop % 512;
                    int zIndex = loop / 512;

                    int size = ColumnLengths[bytes[index]];

                    if (chunkRange.WithinRange(xIndex / 16, zIndex / 16)) {
                        if (bytes[index] == 0) {
                            tempMap.Columns[zIndex, xIndex] = new byte[] { 0 };
                        } else if (bytes[index] == 1) {
                            tempMap.Columns[zIndex, xIndex] = new byte[] { 1 };
                        } else {
                            byte[] tempBytes = new byte[size];

                            for (int i = 0; i < tempBytes.Length; i++) {
                                tempBytes[i] = bytes[index + i];
                            }

                            tempMap.Columns[zIndex, xIndex] = tempBytes;
                        }
                    }

                    index += size;
                    loop++;
                }

                return tempMap;
            } else {
                return null;
            }
        }

        //Get heightmap
        public HeightMap GetHeightMap(Range chunkRange) {
            HeightMap heightMap = new HeightMap(512, 512);

            for (int z = 0; z < heightMap.Width; z++) {
                for (int x = 0; x < heightMap.Height; x++) {
                    if (chunkRange.WithinRange(x / 16, z / 16)) {
                        byte[] columnBytes = Columns[z, x];

                        if (columnBytes[0] != 0) {
                            Column column = new Column(ColumnType.Region);

                            if (columnBytes[0] == 2) {
                                column = GetLandColumn(x, z);
                            } else if (columnBytes[0] == 3) {
                                column = GetWaterColumn(x, z);
                            }

                            heightMap.Columns[z, x] = column;
                        }
                    }
                }
            }

            return heightMap;
        }

        private LandColumn GetLandColumn(int x, int z) {
            byte[] bytes = Columns[z, x];

            LandColumn column = new LandColumn() {
                Color = Color.FromArgb(bytes[1], bytes[2], bytes[3]).ToArgb(),
                Height = bytes[4],
                Exception = bytes[5] == 1 ? true : false,
                Excluded = bytes[6] == 1 ? true : false
            };

            return column;
        }
        private WaterColumn GetWaterColumn(int x, int z){
            byte[] bytes = Columns[z, x];

            WaterColumn column = new WaterColumn() {
                BaseBlockHeight = bytes[1],
                BaseBlockColor = Color.FromArgb(bytes[2], bytes[3], bytes[4]).ToArgb(),
                WaterHeight = bytes[5],
                WaterColor = Color.FromArgb(bytes[6], bytes[7], bytes[8]).ToArgb(),
                HeightException = bytes[9] == 1 ? true : false,
                UndefinedHeightException = bytes[10] == 1 ? true : false,
                ShadowException = bytes[11] == 1 ? true : false,
            };

            return column;
        }

        //Get bytes
        public static byte[] GetHeightMapBytes(HeightMap heightMap) {
            List<byte> bytes = new List<byte>();

            for (int z = 0; z < heightMap.Width; z++) {
                for (int x = 0; x < heightMap.Height; x++) {
                    Column column = heightMap.Columns[z, x];

                    if (column != null) {
                        if (column.Type == ColumnType.Region) {
                            bytes.Add(1);
                        } else if (column.Type == ColumnType.Land) {
                            bytes.AddRange(GetLandColumnBytes((LandColumn)column));
                        } else if (column.Type == ColumnType.Water) {
                            bytes.AddRange(GetWaterColumnBytes((WaterColumn)column));
                        }
                    } else {
                        bytes.Add(0);
                    }
                }
            }

            return bytes.ToArray();
        }

        private static List<byte> GetLandColumnBytes(LandColumn column) {
            Color color = Color.FromArgb(column.Color);

            List<byte> bytes = new List<byte>() {
                2,
                color.R, color.G, color.B,
                column.Height,
                (byte)(column.Exception == true ? 1 : 0),
                (byte)(column.Excluded == true ? 1 : 0)
            };

            return bytes;
        }
        private static List<byte> GetWaterColumnBytes(WaterColumn column) {
            Color baseBlockColor = Color.FromArgb(column.BaseBlockColor);
            Color waterColor = Color.FromArgb(column.WaterColor);
            
            List<byte> bytes = new List<byte>() {
                3,
                column.BaseBlockHeight,
                baseBlockColor.R, baseBlockColor.G, baseBlockColor.B,
                column.WaterHeight,
                waterColor.R, waterColor.G, waterColor.B,
                (byte)(column.HeightException == true ? 1 : 0),
                (byte)(column.UndefinedHeightException == true ? 1 : 0),
                (byte)(column.ShadowException == true ? 1 : 0)
            };

            return bytes;
        }
    }
}

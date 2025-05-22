using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace RegionMapping {
    public class Render{
        public RenderSettings Settings { get; set; }
        private HeightMap HeightMap { get; set; }

        public Render(RenderSettings settings){
            this.Settings = settings;
        }
        public Render(){
            this.Settings = new RenderSettings();
        }

        public Bitmap RenderMap(HeightMap heightMap){
            return RenderMap(heightMap, 0, 0, heightMap.Height, heightMap.Width);
        }
        public Bitmap RenderMap(HeightMap heightMap, int x1, int z1, int x2, int z2) {
            this.HeightMap = heightMap;

            int width = (z2 - z1) / Settings.Scale;
            int height = (x2 - x1) / Settings.Scale;

            DirectBitmap bitmap = new DirectBitmap(width, height, Settings.PixelFormat);
            for (int z = z1; z < z2; z += Settings.Scale) {
                for (int x = x1; x < x2; x += Settings.Scale) {
                    Column column = HeightMap.Columns[z, x];

                    if (column != null) {
                        int xPixel = ((height * Settings.Scale) - (x - x1) - 1) / Settings.Scale;
                        int zPixel = (z - z1) / Settings.Scale;

                        if (column.Type == ColumnType.Region) { //If empty column i.e. chunk not generated
                            bitmap.SetPixel(zPixel, xPixel, GetRegionColor(x / 512 + HeightMap.XStart, z / 512 + HeightMap.ZStart));
                        } else if (column.Type == ColumnType.Land) { //If block found
                            Color blockColor = ChangeLandColor(x, z);

                            bitmap.SetPixel(zPixel, xPixel, blockColor);
                        } else if (column.Type == ColumnType.Water) {
                            Color blockColor = ChangeWaterColor(x, z);

                            bitmap.SetPixel(zPixel, xPixel, blockColor);
                        }
                    }
                }
            }

            return bitmap.Bitmap;
        }

        private Color ChangeLandColor(int x, int z){
            LandColumn column = HeightMap.Columns[z, x] as LandColumn;

            Color blockColor = Color.FromArgb(column.Color);
            blockColor = ChangeColorBrightness(blockColor, GetColorPercentage(column.Height, Settings.HeightColorTypeLand), ColorChangeType.Multiply);

            //Set block shadow
            float intensity = 1;

            if (!column.Excluded) {
                SetBlockShadow(column, x, z, intensity, ref blockColor);
            }

            return blockColor;
        }
        private Color ChangeWaterColor(int x, int z){
            WaterColumn waterColumn = HeightMap.Columns[z, x] as WaterColumn;

            Color blockColor = Color.FromArgb(waterColumn.BaseBlockColor);
            blockColor = ChangeColorBrightness(blockColor, GetColorPercentage(waterColumn.GetHeight(), Settings.HeightColorTypeLand), ColorChangeType.Multiply);

            //WaterMap
            float intensity;

            if (Settings.ClearWater) {
                Color waterColor = Color.FromArgb(waterColumn.WaterColor);
                waterColor = ChangeColorBrightness(waterColor, GetColorPercentage(waterColumn.WaterHeight, Settings.HeightColorTypeWater), ColorChangeType.Multiply);

                int columnHeight = waterColumn.WaterHeight - waterColumn.BaseBlockHeight;
                float opacityHeight = 0.002F;
                float totalOpacity = 0.85F + columnHeight * opacityHeight;
                totalOpacity = totalOpacity > 0.95F ? 0.95F : totalOpacity;

                blockColor = MergeColors(blockColor, waterColor, totalOpacity);

                intensity = waterColumn.UndefinedHeightException ? 0.1F : 0.2F;
            } else {
                blockColor = Color.FromArgb(waterColumn.WaterColor);
                blockColor = ChangeColorBrightness(blockColor, GetColorPercentage(waterColumn.WaterHeight, Settings.HeightColorTypeWater), ColorChangeType.Multiply);

                intensity = 0;
            }

            //Set block shadow
            if (!waterColumn.ShadowException) {
                SetBlockShadow(waterColumn, x, z, intensity, ref blockColor);
            }

            return blockColor;
        }

        //Scripts
        private Direction GetDirection(Column column, Directions dir){
            Direction direction = Settings.Directions.Land[dir];

            if (column != null) {
                if (column.Type == ColumnType.Water) {
                    direction = Settings.Directions.Water[dir];
                }
            }

            return direction;
        }

        private static int GetHeightDifference(int thisColHeight, int prevColHeight, Direction direction){
            int heightDifference = thisColHeight - prevColHeight;

            heightDifference = heightDifference < 0 ? 0 : heightDifference;
            heightDifference = heightDifference > direction.MaxHeight ? direction.MaxHeight : heightDifference;

            return heightDifference;
        }
        private static void SetBlockDirection(Column column, Column prevCol, Direction direction, float intensity, ref int decreaseBy, ref List<int> heightList){
            if (prevCol != null) {
                if (prevCol.Type == ColumnType.Land || prevCol.Type == ColumnType.Water) {
                    int heightDifference = GetHeightDifference(column.GetHeight(), prevCol.GetHeight(), direction);

                    decreaseBy += (int)((float)direction.ColorDifference * intensity) * heightDifference;

                    if (heightDifference > 0) heightList.Add((int)((float)direction.ColorDifference * intensity) * heightDifference);
                }
            }
        }
        private void SetBlockShadow(Column column, int x, int z, float intensity, ref Color blockColor){
            int decreaseBy = 0;
            List<int> heightList = new List<int>();
            if (z - 1 > -1) {
                Column prevCol = HeightMap.Columns[z - 1, x];
                SetBlockDirection(column, prevCol, GetDirection(prevCol, Directions.Left), intensity, ref decreaseBy, ref heightList);
            }
            if (x + 1 < HeightMap.Height - 1) {
                Column prevCol = HeightMap.Columns[z, x + 1];
                SetBlockDirection(column, prevCol, GetDirection(prevCol, Directions.Top), intensity, ref decreaseBy, ref heightList);
            }
            if (z + 1 < HeightMap.Width - 1) {
                Column prevCol = HeightMap.Columns[z + 1, x];
                SetBlockDirection(column, prevCol, GetDirection(prevCol, Directions.Right), intensity, ref decreaseBy, ref heightList);
            }
            if (x - 1 > 0) {
                Column prevCol = HeightMap.Columns[z, x - 1];
                SetBlockDirection(column, prevCol, GetDirection(prevCol, Directions.Bottom), intensity, ref decreaseBy, ref heightList);
            }

            if (decreaseBy > 0 && heightList.Count > 0) {
                decreaseBy = heightList.Max();
                blockColor = ChangeColorBrightness(blockColor, -(float)decreaseBy, ColorChangeType.Add);
            }
        }

        private static float GetColorPercentage(int height, HeightColorSettings settings){
            if (settings.Type == ColorPercentageType.Multiplied) {
                float average = settings.Average;

                height = height < settings.Min ? settings.Min : height;
                //height = height < 62 ? 62 : height;
                height = height > settings.Max ? settings.Max : height;

                return 1 + (height - average) / average * 1.75F;
            } else if (settings.Type == ColorPercentageType.Incremental) {
                float average = settings.Average;
                float intensity = 6.35F;

                height = height < settings.Min ? settings.Min : height;
                height = height > settings.Max ? settings.Max : height;

                return 1 - ((average - height) * intensity / 256.0F);
            }

            return 1;
        }

        private static Color GetRegionColor(int xIndex, int zIndex){
            if (xIndex % 2 == 0) {
                if (zIndex % 2 == 0) {
                    return Color.Gray;
                } else {
                    return Color.DarkGray;
                }
            } else {
                if (zIndex % 2 == 0) {
                    return Color.DarkGray;
                } else {
                    return Color.Gray;
                }
            }
        }
        private static Color ChangeColorBrightness(Color color, float value, ColorChangeType type){
            float r = color.R;
            float g = color.G;
            float b = color.B;

            if (type == ColorChangeType.Add) {
                r += value;
                g += value;
                b += value;
            } else if (type == ColorChangeType.Multiply) {
                r *= value;
                g *= value;
                b *= value;
            } else if (type == ColorChangeType.Set) {
                r = value;
                g = value;
                b = value;
            }

            CalibrateColor(ref r, ref g, ref b);

            return Color.FromArgb(color.A, (int)r, (int)g, (int)b);
        }
        private static Color MergeColors(Color baseColor, Color layerColor, float opacity){
            float r = baseColor.R;
            float g = baseColor.G;
            float b = baseColor.B;

            r -= ((r - layerColor.R) * opacity);
            g -= ((g - layerColor.G) * opacity);
            b -= ((b - layerColor.B) * opacity);

            CalibrateColor(ref r, ref g, ref b);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        private static void CalibrateColor(ref float r, ref float g, ref float b){
            r = r > 255 ? 255 : r;
            g = g > 255 ? 255 : g;
            b = b > 255 ? 255 : b;

            r = r < 0 ? 0 : r;
            g = g < 0 ? 0 : g;
            b = b < 0 ? 0 : b;
        }
    }

    public enum ColorPercentageType {
        Multiplied,
        Incremental
    }
    public enum ColorChangeType {
        Add,
        Multiply,
        Set
    }

    //Code from https://stackoverflow.com/a/34801225/6597542
    public class DirectBitmap{
        public Bitmap Bitmap { get; private set; }
        private int[] Bits { get; set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height, PixelFormat pixelFormat){
            Width = width;
            Height = height;
            Bits = new int[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, pixelFormat, BitsHandle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, Color colour){
            int index = x + (y * Width);
            int col = colour.ToArgb();

            Bits[index] = col;
        }
    }
}

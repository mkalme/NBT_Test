using System;
using System.Drawing.Imaging;

namespace RegionMapping {
    public class RenderSettings {
        public int Scale = 1;
        public PixelFormat PixelFormat = PixelFormat.Format32bppArgb;

        public HeightColorSettings HeightColorTypeLand = new HeightColorSettings();
        public HeightColorSettings HeightColorTypeWater = new HeightColorSettings();

        public bool ClearWater = true;

        public DirectionProfile Directions = DirectionProfile.Medium;
    }

    public class HeightColorSettings {
        public ColorPercentageType Type = ColorPercentageType.Multiplied;
        public float Average = 70;
        public int Min = 57;
        public int Max = 90;
    }
}

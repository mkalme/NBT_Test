using System;

namespace RegionMapping {
    public class HeightMap {
        public Column[,] Columns { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int XStart { get; set; }
        public int ZStart { get; set; }

        public HeightMap(int width, int height){
            this.Columns = new Column[width, height];
            this.Width = width;
            this.Height = height;
            this.XStart = 0;
            this.ZStart = 0;
        }
        public HeightMap(int width, int height, int xStart, int zStart){
            this.Columns = new Column[width, height];
            this.Width = width;
            this.Height = height;
            this.XStart = xStart;
            this.ZStart = zStart;
        }
        public HeightMap(Column[,] columns, int xStart, int zStart){
            this.Columns = columns;
            this.Width = columns.GetLength(0);
            this.Height = columns.GetLength(1);
            this.XStart = xStart;
            this.ZStart = zStart;
        }
    }
}

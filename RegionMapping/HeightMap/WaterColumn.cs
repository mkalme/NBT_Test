using System;

namespace RegionMapping {
    public class WaterColumn : Column {
        public byte BaseBlockHeight { get; set; }
        public int BaseBlockColor { get; set; }

        public byte WaterHeight { get; set; }
        public int WaterColor { get; set; }

        public bool HeightException { get; set; }
        public bool UndefinedHeightException { get; set; }
        public bool ShadowException { get; set; }

        public WaterColumn() : base(ColumnType.Water){

        }

        public override byte GetHeight(){
            return BaseBlockHeight;
        }
    }
}

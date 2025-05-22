using System;

namespace RegionMapping {
    public class LandColumn : Column {
        public int Color { get; set; }
        public byte Height { get; set; }
        public bool Exception { get; set; }
        public bool Excluded { get; set; }

        public LandColumn() : base(ColumnType.Land){

        }

        public override byte GetHeight(){
            return Height;
        }
    }
}

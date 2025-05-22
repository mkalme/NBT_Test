using System;

namespace RegionMapping {
    public class Column {
        public ColumnType Type { get; set; }

        public Column(ColumnType type){
            this.Type = type;
        }

        public virtual byte GetHeight(){
            return 0;
        }
    }

    public enum ColumnType {
        None,
        Region,
        Land,
        Water
    }
}

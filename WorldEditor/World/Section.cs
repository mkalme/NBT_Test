using System.Collections.Generic;

namespace WorldEditor
{
    public class Section {
        public int YIndex { get; set; }
        public short[] BlockID { get; set; }
        public Dictionary<short, List<Property>> Properties { get; set; }

        public bool Empty { get; set; }

        public Section() {
            this.YIndex = 0;
            this.BlockID = new short[4096];
            this.Properties = new Dictionary<short, List<Property>>();
            this.Empty = true;
        }
        public Section(int yIndex, short[] blockID, Dictionary<short, List<Property>> properties, bool empty) {
            this.YIndex = yIndex;
            this.BlockID = blockID;
            this.Properties = properties;
            this.Empty = empty;
        }

        public void SetBlockID(short id, int x, int y, int z) {
            BlockID[y * 256 + z * 16 + x] = id;
        }
        public short GetBlockID(int x, int y, int z) {
            return BlockID[y * 256 + z * 16 + x];
        }

        public void SetProperties(int x, int y, int z, List<Property> properties) {
            short key = (short)(y * 256 + z * 16 + x);

            if (properties != null) {
                if (Properties.ContainsKey(key)) {
                    Properties[key] = properties;
                } else {
                    Properties.Add(key, properties);
                }
            }
        }
        public List<Property> GetProperties(int x, int y, int z) {
            short key = (short)(y * 256 + z * 16 + x);

            if (Properties.ContainsKey(key)) {
                return Properties[key];
            } else {
                return new List<Property>();
            }
        }
        public List<Property> GetProperties(short index) {
            if (Properties.ContainsKey(index)) {
                return Properties[index];
            } else {
                return new List<Property>();
            }
        }
        public List<Property> GetProperties(short index, bool returnNull) {
            if (Properties.ContainsKey(index)) {
                return Properties[index];
            } else{
                if (returnNull) {
                    return null;
                } else {
                    return new List<Property>();
                }
            }
        }

        public Block GetBlock(int x, int y, int z) {
            int index = y * 256 + z * 16 + x;

            return new Block(BlockID[index], GetProperties((short)index));
        }
    }
}

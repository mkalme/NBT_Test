using System.Collections.Generic;

namespace WorldEditor
{
    public class Block
    {
        public short ID { get; set; }
        public List<Property> Properties { get; set; }

        public Block() {
            this.ID = 0;
            this.Properties = new List<Property>();
        }
        public Block(short id) {
            this.ID = id;
            this.Properties = new List<Property>();
        }
        public Block(short id, List<Property> properties) {
            this.ID = id;
            this.Properties = properties;
        }

        public static Block FromNamespace(string nameSpace) {
            return new Block(WorldEditor.ID.BlockIDReverse[nameSpace]);
        }

        public bool Equals(Block block) {
            return Equals(block.ID, block.Properties);
        }
        public bool Equals(short id, List<Property> properties) {
            //Check id
            if (ID != id) {
                return false;
            }

            //Check properties
            if (Properties == null && properties == null) {
                return true;
            } else if (!(Properties != null && properties != null)) {
                return false;
            }

            if (Properties.Count != properties.Count) {
                return false;
            } else {
                for (int i = 0; i < Properties.Count; i++) {
                    if (Properties[i].Name != properties[i].Name ||
                        Properties[i].Value != properties[i].Value) {

                        return false;
                    }
                }
            }

            return true;
        }
    }
}

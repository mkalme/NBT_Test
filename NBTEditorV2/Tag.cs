using System;

namespace NBTEditorV2 {
    public class Tag {
        public TagType Type { get; set; }
        public string Name { get; set; }
        public object Item { get; set; }

        public Tag() {
            this.Type = TagType.End;
            this.Name = "";
            this.Item = null;
        }
        public Tag(TagType type, string name, object item) {
            this.Type = type;
            this.Name = name;
            this.Item = item;
        }

        public bool Equals(Tag tag) {
            if (Name != tag.Name) {
                return false;
            }
            if (Type != tag.Type) {
                return false;
            }

            if (Type == TagType.Compound) {
                return ((CompoundTag)Item).Equals((CompoundTag)tag.Item);
            } else if (Type == TagType.List) {
                return ((ListTag)Item).Equals((ListTag)tag.Item);
            } else if (Type == TagType.ByteArray) {
                return ((ByteArrayTag)Item).Equals((ByteArrayTag)tag.Item);
            } else if (Type == TagType.LongArray) {
                return ((LongArrayTag)Item).Equals((LongArrayTag)tag.Item);
            } else if (Type == TagType.IntArray) {
                return ((IntArrayTag)Item).Equals((IntArrayTag)tag.Item);
            } else if (Type == TagType.String) {
                return ((StringTag)Item).Equals((StringTag)tag.Item);
            } else if (Type == TagType.Double) {
                return ((DoubleTag)Item).Equals((DoubleTag)tag.Item);
            } else if (Type == TagType.Short) {
                return ((ShortTag)Item).Equals((ShortTag)tag.Item);
            } else if (Type == TagType.Float) {
                return ((FloatTag)Item).Equals((FloatTag)tag.Item);
            } else if (Type == TagType.Byte) {
                return ((ByteTag)Item).Equals((ByteTag)tag.Item);
            } else if (Type == TagType.Long) {
                return ((LongTag)Item).Equals((LongTag)tag.Item);
            } else if (Type == TagType.Int) {
                return ((IntTag)Item).Equals((IntTag)tag.Item);
            }

            return false;
        }
    }
}

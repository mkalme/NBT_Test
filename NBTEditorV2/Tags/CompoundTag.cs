using System;
using System.Collections.Generic;
using System.Linq;

namespace NBTEditorV2
{
    public class CompoundTag{
        public string Name { get; set; }
        private List<Tag> Tags { get; set; }

        public CompoundTag() {
            this.Name = "";
            this.Tags = new List<Tag>();
        }
        public CompoundTag(string name) {
            this.Name = name;
            this.Tags = new List<Tag>();
        }
        public CompoundTag(string name, List<Tag> tags) {
            this.Name = name;
            this.Tags = tags;
        }

        public static CompoundTag LoadTag(byte[] bytes) {
            int startIndex = 0;
            CompoundTag compoundTag = LoadTag(bytes, ref startIndex);

            return compoundTag;
        }
        public static CompoundTag LoadTag(byte[] bytes, ref int startIndex) {
            startIndex++;

            string name = Scripts.GetName(bytes, ref startIndex);

            List<Tag> tags = LoadPayload(bytes, ref startIndex);

            return new CompoundTag(name, tags);
        }
        public static List<Tag> LoadPayload(byte[] bytes, ref int startIndex) {
            List<Tag> tags = new List<Tag>();

            for (int i = startIndex; i < bytes.Length;) {
                TagType type = Scripts.TagTypeID[bytes[i]];

                if (type == TagType.End) {
                    startIndex = i + 1;

                    i = bytes.Length;
                } else if (type == TagType.Byte) {
                    ByteTag tag = ByteTag.LoadTag(bytes, ref i);

                    tags.Add(new Tag(TagType.Byte, tag.Name, tag));
                } else if (type == TagType.Short) {
                    ShortTag tag = ShortTag.LoadTag(bytes, ref i);

                    tags.Add(new Tag(TagType.Short, tag.Name, tag));
                } else if (type == TagType.Int) {
                    IntTag tag = IntTag.LoadTag(bytes, ref i);

                    tags.Add(new Tag(TagType.Int, tag.Name, tag));
                } else if (type == TagType.Long) {
                    LongTag tag = LongTag.LoadTag(bytes, ref i);

                    tags.Add(new Tag(TagType.Long, tag.Name, tag));
                } else if (type == TagType.Float) {
                    FloatTag tag = FloatTag.LoadTag(bytes, ref i);

                    tags.Add(new Tag(TagType.Float, tag.Name, tag));
                } else if (type == TagType.Double) {
                    DoubleTag tag = DoubleTag.LoadTag(bytes, ref i);

                    tags.Add(new Tag(TagType.Double, tag.Name, tag));
                } else if (type == TagType.ByteArray) {
                    ByteArrayTag tag = ByteArrayTag.LoadTag(bytes, ref i);

                    tags.Add(new Tag(TagType.ByteArray, tag.Name, tag));
                } else if (type == TagType.String) {
                    StringTag tag = StringTag.LoadTag(bytes, ref i);

                    tags.Add(new Tag(TagType.String, tag.Name, tag));
                } else if (type == TagType.List) {
                    ListTag tag = ListTag.LoadTag(bytes, ref i);

                    tags.Add(new Tag(TagType.List, tag.Name, tag));
                } else if (type == TagType.Compound) {
                    CompoundTag tag = LoadTag(bytes, ref i);

                    tags.Add(new Tag(TagType.Compound, tag.Name, tag));
                } else if (type == TagType.IntArray) {
                    IntArrayTag tag = IntArrayTag.LoadTag(bytes, ref i);

                    tags.Add(new Tag(TagType.IntArray, tag.Name, tag));
                } else if (type == TagType.LongArray) {
                    LongArrayTag tag = LongArrayTag.LoadTag(bytes, ref i);

                    tags.Add(new Tag(TagType.LongArray, tag.Name, tag));
                }
            }

            return tags;
        }

        public static List<byte> GetTagBytes(CompoundTag tag)
        {
            List<byte> bytes = new List<byte>() { 10 };

            bytes.AddRange(Scripts.GetNameBytes(tag.Name));
            bytes.AddRange(GetPayloadBytes(tag.GetTags()));

            return bytes;
        }
        public static List<byte> GetPayloadBytes(List<Tag> tags)
        {
            List<byte> bytes = new List<byte>();

            for (int i = 0; i < tags.Count; i++) {
                Tag tag = tags[i];

                if (tag.Type == TagType.Byte) {
                    bytes.AddRange(ByteTag.GetTagBytes((ByteTag)tag.Item));
                } else if (tag.Type == TagType.Short) {
                    bytes.AddRange(ShortTag.GetTagBytes((ShortTag)tag.Item));
                } else if (tag.Type == TagType.Int) {
                    bytes.AddRange(IntTag.GetTagBytes((IntTag)tag.Item));
                } else if (tag.Type == TagType.Long) {
                    bytes.AddRange(LongTag.GetTagBytes((LongTag)tag.Item));
                } else if (tag.Type == TagType.Float) {
                    bytes.AddRange(FloatTag.GetTagBytes((FloatTag)tag.Item));
                } else if (tag.Type == TagType.Double) {
                    bytes.AddRange(DoubleTag.GetTagBytes((DoubleTag)tag.Item));
                } else if (tag.Type == TagType.ByteArray) {
                    bytes.AddRange(ByteArrayTag.GetTagBytes((ByteArrayTag)tag.Item));
                } else if (tag.Type == TagType.String) {
                    bytes.AddRange(StringTag.GetTagBytes((StringTag)tag.Item));
                } else if (tag.Type == TagType.List) {
                    bytes.AddRange(ListTag.GetTagBytes((ListTag)tag.Item));
                } else if (tag.Type == TagType.Compound) {
                    bytes.AddRange(GetTagBytes((CompoundTag)tag.Item));
                } else if (tag.Type == TagType.IntArray) {
                    bytes.AddRange(IntArrayTag.GetTagBytes((IntArrayTag)tag.Item));
                } else if (tag.Type == TagType.LongArray) {
                    bytes.AddRange(LongArrayTag.GetTagBytes((LongArrayTag)tag.Item));
                }
            }

            bytes.Add(0);

            return bytes;
        }

        //Public object functions
        public void Add(object tag) {
            if (tag != null) {

                Type type = tag.GetType();

                Tag tagToAdd = new Tag();

                if (type == typeof(CompoundTag)) {
                    tagToAdd.Type = TagType.Compound;
                    tagToAdd.Name = ((CompoundTag)tag).Name;
                    tagToAdd.Item = tag;
                    Tags.Add(tagToAdd);
                } else if (type == typeof(ListTag)) {
                    tagToAdd.Type = TagType.List;
                    tagToAdd.Name = ((ListTag)tag).Name;
                    tagToAdd.Item = tag;
                    Tags.Add(tagToAdd);
                } else if (type == typeof(ByteArrayTag)) {
                    tagToAdd.Type = TagType.ByteArray;
                    tagToAdd.Name = ((ByteArrayTag)tag).Name;
                    tagToAdd.Item = tag;
                    Tags.Add(tagToAdd);
                } else if (type == typeof(LongArrayTag)) {
                    tagToAdd.Type = TagType.LongArray;
                    tagToAdd.Name = ((LongArrayTag)tag).Name;
                    tagToAdd.Item = tag;
                    Tags.Add(tagToAdd);
                } else if (type == typeof(IntArrayTag)) {
                    tagToAdd.Type = TagType.IntArray;
                    tagToAdd.Name = ((IntArrayTag)tag).Name;
                    tagToAdd.Item = tag;
                    Tags.Add(tagToAdd);
                } else if (type == typeof(StringTag)) {
                    tagToAdd.Type = TagType.String;
                    tagToAdd.Name = ((StringTag)tag).Name;
                    tagToAdd.Item = tag;
                    Tags.Add(tagToAdd);
                } else if (type == typeof(DoubleTag)) {
                    tagToAdd.Type = TagType.Double;
                    tagToAdd.Name = ((DoubleTag)tag).Name;
                    tagToAdd.Item = tag;
                    Tags.Add(tagToAdd);
                } else if (type == typeof(ShortTag)) {
                    tagToAdd.Type = TagType.Short;
                    tagToAdd.Name = ((ShortTag)tag).Name;
                    tagToAdd.Item = tag;
                    Tags.Add(tagToAdd);
                } else if (type == typeof(FloatTag)) {
                    tagToAdd.Type = TagType.Float;
                    tagToAdd.Name = ((FloatTag)tag).Name;
                    tagToAdd.Item = tag;
                    Tags.Add(tagToAdd);
                } else if (type == typeof(ByteTag)) {
                    tagToAdd.Type = TagType.Byte;
                    tagToAdd.Name = ((ByteTag)tag).Name;
                    tagToAdd.Item = tag;
                    Tags.Add(tagToAdd);
                } else if (type == typeof(LongTag)) {
                    tagToAdd.Type = TagType.Long;
                    tagToAdd.Name = ((LongTag)tag).Name;
                    tagToAdd.Item = tag;
                    Tags.Add(tagToAdd);
                } else if (type == typeof(IntTag)) {
                    tagToAdd.Type = TagType.Int;
                    tagToAdd.Name = ((IntTag)tag).Name;
                    tagToAdd.Item = tag;
                    Tags.Add(tagToAdd);
                } else if (type == typeof(Tag)) {
                    tagToAdd = (Tag)tag;
                    Tags.Add(tagToAdd);
                }
            }
        }
        public void RemoveAt(int index) {
            Tags.RemoveAt(index);
        }
        public void Remove(string path) {
            List<string> arrayPath = path.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            RemoveByPath(arrayPath);
        }

        public Tag TagAt(int index)
        {
            return Tags.ElementAt(index);
        }
        public int GetSize() {
            return Tags.Count;
        }
        public List<Tag> GetTags() {
            return Tags;
        }

        public ByteTag GetByteTag(string path) {
            Tag tag = GetTagByPath(path);

            if (tag != null) {
                return (ByteTag)tag.Item;
            } else {
                return null;
            }
        }
        public ShortTag GetShortTag(string path)
        {
            Tag tag = GetTagByPath(path);

            if (tag != null) {
                return (ShortTag)tag.Item;
            } else {
                return null;
            }
        }
        public IntTag GetIntTag(string path)
        {
            Tag tag = GetTagByPath(path);

            if (tag != null) {
                return (IntTag)tag.Item;
            } else {
                return null;
            }
        }
        public LongTag GetLongTag(string path)
        {
            Tag tag = GetTagByPath(path);

            if (tag != null) {
                return (LongTag)tag.Item;
            } else {
                return null;
            }
        }
        public FloatTag GetFloatTag(string path)
        {
            Tag tag = GetTagByPath(path);

            if (tag != null) {
                return (FloatTag)tag.Item;
            } else {
                return null;
            }
        }
        public DoubleTag GetDoubleTag(string path)
        {
            Tag tag = GetTagByPath(path);

            if (tag != null) {
                return (DoubleTag)tag.Item;
            } else {
                return null;
            }
        }
        public ByteArrayTag GetByteArrayTag(string path)
        {
            Tag tag = GetTagByPath(path);

            if (tag != null) {
                return (ByteArrayTag)tag.Item;
            } else {
                return null;
            }
        }
        public StringTag GetStringTag(string path)
        {
            Tag tag = GetTagByPath(path);

            if (tag != null) {
                return (StringTag)tag.Item;
            } else {
                return null;
            }
        }
        public ListTag GetListTag(string path)
        {
            Tag tag = GetTagByPath(path);

            if (tag != null) {
                return (ListTag)tag.Item;
            } else {
                return null;
            }
        }
        public CompoundTag GetCompoundTag(string path) {
            Tag tag = GetTagByPath(path);

            if (tag != null) {
                return (CompoundTag)tag.Item;
            } else {
                return null;
            }
        }
        public IntArrayTag GetIntArrayTag(string path)
        {
            Tag tag = GetTagByPath(path);

            if (tag != null) {
                return (IntArrayTag)tag.Item;
            } else {
                return null;
            }
        }
        public LongArrayTag GetLongArrayTag(string path)
        {
            Tag tag = GetTagByPath(path);

            if (tag != null) {
                return (LongArrayTag)tag.Item;
            } else {
                return null;
            }
        }
        public Tag GetTag(string path) {
            Tag tag = GetTagByPath(path);

            if (tag != null) {
                return tag;
            } else {
                return null;
            }
        }

        public bool Equals(CompoundTag tag) {
            if (Name != tag.Name) {
                return false;
            }
            if (Tags.Count != tag.Tags.Count) {
                return false;
            }
            for (int i = 0; i < Tags.Count; i++) {
                if (!Tags[i].Equals(tag.Tags[i])) {
                    return false;
                }
            }

            return true;
        }

        //Private object functions
        private int GetTagIndexByName(CompoundTag compound, string name) {
            for (int i = 0; i < compound.GetSize(); i++) {
                if (compound.TagAt(i).Name == name) {
                    return i;
                }
            }

            return -1;
        }
        private void RemoveByPath(List<string> path)
        {
            CompoundTag compoundTag = this;
            for (int i = 0; i < path.Count; i++) {
                int index = GetTagIndexByName(compoundTag, path[i]);

                if (index > -1) {
                    Tag tag = compoundTag.Tags[index];
                    if (i == path.Count - 1) {
                        compoundTag.RemoveAt(index);
                    } else {
                        if (tag.Type == TagType.Compound) {
                            compoundTag = (CompoundTag)tag.Item;
                        } else {
                            break;
                        }
                    }
                } else {
                    break;
                }
            }
        }
        private Tag GetTagByPath(string path)
        {
            List<string> arrayPath = path.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            CompoundTag compoundTag = this;
            for (int i = 0; i < arrayPath.Count; i++) {
                int index = GetTagIndexByName(compoundTag, arrayPath[i]);

                if (index > -1) {
                    Tag tag = compoundTag.Tags[index];
                    if (i == arrayPath.Count - 1) {
                        return compoundTag.TagAt(index);
                    } else {
                        if (tag.Type == TagType.Compound) {
                            compoundTag = (CompoundTag)tag.Item;
                        } else {
                            return null;
                        }
                    }
                } else {
                    return null;
                }
            }

            return null;
        }
    }
}

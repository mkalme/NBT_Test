using System;
using System.Collections.Generic;

namespace NBTEditorV2 {
    public class ListTag {
        public string Name { get; set; }
        public ListPayload Payload { get; set; }

        public ListTag() {
            this.Name = "";
            this.Payload = new ListPayload(TagType.End, new List<object>());
        }
        public ListTag(string name, TagType type, List<object> elements) {
            this.Name = name;
            this.Payload = new ListPayload(type, elements);
        }
        public ListTag(string name, ListPayload payload) {
            this.Name = name;
            this.Payload = payload;
        }

        public static ListTag LoadTag(byte[] bytes, ref int startIndex) {
            startIndex++;

            string name = Scripts.GetName(bytes, ref startIndex);

            ListPayload payload = LoadPayload(bytes, ref startIndex);

            return new ListTag(name, payload);
        }
        public static ListPayload LoadPayload(byte[] bytes, ref int startIndex) {
            TagType type = Scripts.TagTypeID[bytes[startIndex]];
            startIndex++;

            int count = BitConverter.ToInt32(new byte[] { bytes[startIndex + 3], bytes[startIndex + 2], bytes[startIndex + 1], bytes[startIndex] }, 0);
            startIndex += 4;

            List<object> elements = new List<object>();
            for (int i = 0; i < count; i++) {
                if (type == TagType.Byte) {
                    elements.Add(ByteTag.LoadPayload(bytes, ref startIndex));
                }else if (type == TagType.Short) {
                    elements.Add(ShortTag.LoadPayload(bytes, ref startIndex));
                } else if (type == TagType.Int) {
                    elements.Add(IntTag.LoadPayload(bytes, ref startIndex));
                } else if (type == TagType.Long) {
                    elements.Add(LongTag.LoadPayload(bytes, ref startIndex));
                } else if (type == TagType.Float) {
                    elements.Add(FloatTag.LoadPayload(bytes, ref startIndex));
                } else if (type == TagType.Double) {
                    elements.Add(DoubleTag.LoadPayload(bytes, ref startIndex));
                } else if (type == TagType.ByteArray) {
                    elements.Add(ByteArrayTag.LoadPayload(bytes, ref startIndex));
                } else if (type == TagType.String) {
                    elements.Add(StringTag.LoadPayload(bytes, ref startIndex));
                } else if (type == TagType.List) {
                    elements.Add(LoadPayload(bytes, ref startIndex));
                } else if (type == TagType.Compound) {
                    elements.Add(CompoundTag.LoadPayload(bytes, ref startIndex));
                } else if (type == TagType.IntArray) {
                    elements.Add(IntArrayTag.LoadPayload(bytes, ref startIndex));
                } else if (type == TagType.LongArray) {
                    elements.Add(LongArrayTag.LoadPayload(bytes, ref startIndex));
                }
            }

            return new ListPayload(type, elements);
        }

        public static List<byte> GetTagBytes(ListTag tag)
        {
            List<byte> bytes = new List<byte>() { 9 };

            bytes.AddRange(Scripts.GetNameBytes(tag.Name));
            bytes.AddRange(GetPayloadBytes(tag.Payload));

            return bytes;
        }
        public static List<byte> GetPayloadBytes(ListPayload listPayload)
        {
            List<byte> bytes = new List<byte>() {Scripts.TagTypeIDReverse[listPayload.Type]};

            byte[] intBytes = BitConverter.GetBytes(listPayload.Elements.Count);
            bytes.AddRange(new byte[] { intBytes[3], intBytes[2], intBytes[1], intBytes[0] });

            TagType type = listPayload.Type;
            for (int i = 0; i < listPayload.Elements.Count; i++) {
                object element = listPayload.Elements[i];

                if (type == TagType.Byte) {
                    bytes.AddRange(ByteTag.GetPayloadBytes((byte)element));
                }else if (type == TagType.Short) {
                    bytes.AddRange(ShortTag.GetPayloadBytes((short)element));
                }else if (type == TagType.Int) {
                    bytes.AddRange(IntTag.GetPayloadBytes((int)element));
                } else if (type == TagType.Long) {
                    bytes.AddRange(LongTag.GetPayloadBytes((long)element));
                } else if (type == TagType.Float) {
                    bytes.AddRange(FloatTag.GetPayloadBytes((float)element));
                } else if (type == TagType.Double) {
                    bytes.AddRange(DoubleTag.GetPayloadBytes((double)element));
                } else if (type == TagType.ByteArray) {
                    bytes.AddRange(ByteArrayTag.GetPayloadBytes((byte[])element));
                } else if (type == TagType.String) {
                    bytes.AddRange(StringTag.GetPayloadBytes((string)element));
                } else if (type == TagType.List) {
                    bytes.AddRange(GetPayloadBytes((ListPayload)element));
                } else if (type == TagType.Compound) {
                    bytes.AddRange(CompoundTag.GetPayloadBytes((List<Tag>)element));
                } else if (type == TagType.IntArray) {
                    bytes.AddRange(IntArrayTag.GetPayloadBytes((int[])element));
                } else if (type == TagType.LongArray) {
                    bytes.AddRange(LongArrayTag.GetPayloadBytes((long[])element));
                }
            }

            return bytes;
        }

        public bool Equals(ListTag tag) {
            if (Name != tag.Name) {
                return false;
            }
            if (Payload.Type != tag.Payload.Type) {
                return false;
            }
            if (Payload.Elements.Count != tag.Payload.Elements.Count) {
                return false;
            }
            for (int i = 0; i < Payload.Elements.Count; i++) {
                if (!ReferenceEquals(Payload.Elements[i], tag.Payload.Elements[i])) {
                    return false;
                };
            }

            return false;
        }
    }

    public class ListPayload {
        public TagType Type { get; set; }
        public List<object> Elements { get; set; }

        public ListPayload(TagType type, List<object> elements) {
            this.Type = type;
            this.Elements = elements;
        }
    }
}

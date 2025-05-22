using System;
using System.Collections.Generic;

namespace NBTEditorV2 {
    public class ByteTag {
        public string Name { get; set; }
        public byte Value { get; set; }

        public ByteTag(string name, byte value)
        {
            this.Name = name;
            this.Value = value;
        }

        public static ByteTag LoadTag(byte[] bytes, ref int startIndex)
        {
            startIndex++;
            
            string name = Scripts.GetName(bytes, ref startIndex);
            
            byte value = LoadPayload(bytes, ref startIndex);

            return new ByteTag(name, value);
        }
        public static byte LoadPayload(byte[] bytes, ref int startIndex)
        {
            byte value = bytes[startIndex];
            
            startIndex++;

            return value;
        }

        public static List<byte> GetTagBytes(ByteTag tag)
        {
            List<byte> bytes = new List<byte>() { 1 };

            bytes.AddRange(Scripts.GetNameBytes(tag.Name));
            bytes.AddRange(GetPayloadBytes(tag.Value));

            return bytes;
        }
        public static List<byte> GetPayloadBytes(byte value)
        {
            List<byte> bytes = new List<byte>() { value};

            return bytes;
        }

        public bool Equals(ByteTag tag)
        {
            if (Name != tag.Name) {
                return false;
            }
            if (Value != tag.Value) {
                return false;
            }

            return true;
        }
    }
}

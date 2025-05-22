using System;
using System.Collections.Generic;

namespace NBTEditorV2 {
    public class ShortTag {
        public string Name { get; set; }
        public short Value { get; set; }

        public ShortTag(string name, short value) {
            this.Name = name;
            this.Value = value;
        }

        public static ShortTag LoadTag(byte[] bytes, ref int startIndex)
        {
            startIndex++;

            string name = Scripts.GetName(bytes, ref startIndex);
            
            short value = LoadPayload(bytes, ref startIndex);

            return new ShortTag(name, value);
        }
        public static short LoadPayload(byte[] bytes, ref int startIndex)
        {
            short value = BitConverter.ToInt16(new byte[] {bytes[startIndex + 1], bytes[startIndex]}, 0);
            startIndex += 2;

            return value;
        }

        public static List<byte> GetTagBytes(ShortTag tag)
        {
            List<byte> bytes = new List<byte>() { 2 };

            bytes.AddRange(Scripts.GetNameBytes(tag.Name));
            bytes.AddRange(GetPayloadBytes(tag.Value));

            return bytes;
        }
        public static List<byte> GetPayloadBytes(short value)
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(new byte[] { (byte)(value >> 8), (byte)(value & 255) });

            return bytes;
        }

        public bool Equals(ShortTag tag) {
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

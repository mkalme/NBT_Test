using System;
using System.Collections.Generic;

namespace NBTEditorV2 {
    public class IntTag {
        public string Name { get; set; }
        public int Value { get; set; }

        public IntTag(string name, int value)
        {
            this.Name = name;
            this.Value = value;
        }

        public static IntTag LoadTag(byte[] bytes, ref int startIndex)
        {
            startIndex++;

            string name = Scripts.GetName(bytes, ref startIndex);
            
            int value = LoadPayload(bytes, ref startIndex);

            return new IntTag(name, value);
        }
        public static int LoadPayload(byte[] bytes, ref int startIndex)
        {
            int value = BitConverter.ToInt32(new byte[] { bytes[startIndex + 3], bytes[startIndex + 2], bytes[startIndex + 1], bytes[startIndex] }, 0);
            startIndex += 4;

            return value;
        }

        public static List<byte> GetTagBytes(IntTag tag)
        {
            List<byte> bytes = new List<byte>() { 3 };

            bytes.AddRange(Scripts.GetNameBytes(tag.Name));
            bytes.AddRange(GetPayloadBytes(tag.Value));

            return bytes;
        }
        public static List<byte> GetPayloadBytes(int value)
        {
            List<byte> bytes = new List<byte>();

            byte[] intBytes = BitConverter.GetBytes(value);

            bytes.AddRange(new byte[] {intBytes[3], intBytes[2], intBytes[1], intBytes[0]});

            return bytes;
        }

        public bool Equals(IntTag tag){
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

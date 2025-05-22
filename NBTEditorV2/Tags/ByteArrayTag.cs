using System;
using System.Collections.Generic;

namespace NBTEditorV2 {
    public class ByteArrayTag {
        public string Name { get; set; }
        public byte[] Array { get; set; }

        public ByteArrayTag(string name, byte[] array)
        {
            this.Name = name;
            this.Array = array;
        }

        public static ByteArrayTag LoadTag(byte[] bytes, ref int startIndex)
        {
            startIndex++;

            string name = Scripts.GetName(bytes, ref startIndex);
            
            byte[] array = LoadPayload(bytes, ref startIndex);

            return new ByteArrayTag(name, array);
        }
        public static byte[] LoadPayload(byte[] bytes, ref int startIndex)
        {
            int length = BitConverter.ToInt32(new byte[] { bytes[startIndex + 3], bytes[startIndex + 2], bytes[startIndex + 1], bytes[startIndex] }, 0);
            startIndex += 4;

            byte[] array = new byte[length];
            for (int i = 0; i < length; i++) {
                startIndex++;

                array[i] = bytes[startIndex];
            }

            return array;
        }

        public static List<byte> GetTagBytes(ByteArrayTag tag)
        {
            List<byte> bytes = new List<byte>() { 7 };

            bytes.AddRange(Scripts.GetNameBytes(tag.Name));
            bytes.AddRange(GetPayloadBytes(tag.Array));

            return bytes;
        }
        public static List<byte> GetPayloadBytes(byte[] array)
        {
            List<byte> bytes = new List<byte>();

            byte[] lengthBytes = BitConverter.GetBytes(array.Length);
            bytes.AddRange(new byte[] { lengthBytes[3], lengthBytes[2], lengthBytes[1], lengthBytes[0] });

            bytes.AddRange(array);

            return bytes;
        }

        public bool Equals(ByteArrayTag tag)
        {
            if (Name != tag.Name) {
                return false;
            }
            if (Array.Length != tag.Array.Length) {
                return false;
            }
            for (int i = 0; i < Array.Length; i++) {
                if (Array[i] != tag.Array[i]) {
                    return false;
                }
            }

            return true;
        }
    }
}

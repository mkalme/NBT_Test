using System;
using System.Collections.Generic;

namespace NBTEditorV2 {
    public class LongArrayTag {
        public string Name { get; set; }
        public long[] Array { get; set; }

        public LongArrayTag(string name, long[] array)
        {
            this.Name = name;
            this.Array = array;
        }

        public static LongArrayTag LoadTag(byte[] bytes, ref int startIndex)
        {
            startIndex++;

            string name = Scripts.GetName(bytes, ref startIndex);
            
            long[] array = LoadPayload(bytes, ref startIndex);

            return new LongArrayTag(name, array);
        }
        public static unsafe long[] LoadPayload(byte[] bytes, ref int index)
        {
            int length = BitConverter.ToInt32(new byte[] { bytes[index + 3], bytes[index + 2], bytes[index + 1], bytes[index] }, 0);
            index += 4;

            long[] array = new long[length];

            for (int i = 0; i < length; i++) {
                long value = 0;

                for (int b = index; b < index + 8; b++) {
                    value = (value << 8) + (bytes[b] & 0xff);
                }
                array[i] = value;

                index += 8;
            }

            return array;
        }

        public static List<byte> GetTagBytes(LongArrayTag tag)
        {
            List<byte> bytes = new List<byte>() { 12 };

            bytes.AddRange(Scripts.GetNameBytes(tag.Name));
            bytes.AddRange(GetPayloadBytes(tag.Array));

            return bytes;
        }
        public static List<byte> GetPayloadBytes(long[] array)
        {
            List<byte> bytes = new List<byte>();

            byte[] lengthBytes = BitConverter.GetBytes(array.Length);
            bytes.AddRange(new byte[] { lengthBytes[3], lengthBytes[2], lengthBytes[1], lengthBytes[0] });

            for (int i = 0; i < array.Length; i++) {
                byte[] longBytes = BitConverter.GetBytes(array[i]);
                System.Array.Reverse(longBytes);

                bytes.AddRange(longBytes);
                //bytes.AddRange(new byte[] {
                //    longBytes[7], longBytes[6], longBytes[5], longBytes[4],
                //    longBytes[3], longBytes[2], longBytes[1], longBytes[0]
                //});
            }

            return bytes;
        }

        public bool Equals(LongArrayTag tag)
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

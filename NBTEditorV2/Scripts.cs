using System;
using System.Collections.Generic;
using System.Text;

namespace NBTEditorV2 {
    public class Scripts {
        public static string GetName(byte[] bytes, ref int startIndex) {
            int length = (bytes[startIndex] * 256) + bytes[startIndex + 1];

            string name = Encoding.UTF8.GetString(bytes, startIndex + 2, length);

            startIndex += 2 + length;

            return name;
        }

        public static long BytesToInt64(byte[] bytes, int startIndex){
            long value = 0;
            for (int b = startIndex; b < startIndex + 8; b++) {
                value = (value << 8) + (bytes[b] & 0xff);
            }

            return value;
        }
        public static int BytesToInt32(byte[] bytes, int startIndex){
            int value = 0;
            int add = 1;
            for (int i = startIndex + 4; i > startIndex - 1; i--) {
                value += bytes[i] * add;

                add <<= 8;
            }

            return value;
        }

        public static List<byte> GetNameBytes(string name) {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(new byte[] { (byte)(name.Length >> 8), (byte)(name.Length & 255) });
            bytes.AddRange(Encoding.ASCII.GetBytes(name));

            return bytes;
        }

        public static Dictionary<byte, TagType> TagTypeID = new Dictionary<byte, TagType>() {
            { 0, TagType.End },
            { 1, TagType.Byte },
            { 2, TagType.Short },
            { 3, TagType.Int },
            { 4, TagType.Long },
            { 5, TagType.Float },
            { 6, TagType.Double },
            { 7, TagType.ByteArray },
            { 8, TagType.String },
            { 9, TagType.List },
            { 10, TagType.Compound },
            { 11, TagType.IntArray },
            { 12, TagType.LongArray }
        };
        public static Dictionary<TagType, byte> TagTypeIDReverse = new Dictionary<TagType, byte>();

        static Scripts() {
            foreach (var element in TagTypeID) {
                TagTypeIDReverse.Add(element.Value, element.Key);
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace NBTEditorV2 {
    public class LongTag {
        public string Name { get; set; }
        public long Value { get; set; }

        public LongTag(string name, long value)
        {
            this.Name = name;
            this.Value = value;
        }

        public static LongTag LoadTag(byte[] bytes, ref int startIndex)
        {
            startIndex++;

            string name = Scripts.GetName(bytes, ref startIndex);
            
            long value = LoadPayload(bytes, ref startIndex);

            return new LongTag(name, value);
        }
        public static long LoadPayload(byte[] bytes, ref int startIndex)
        {
            long value = Scripts.BytesToInt64(bytes, startIndex);
            startIndex += 8;

            return value;
        }

        public static List<byte> GetTagBytes(LongTag tag)
        {
            List<byte> bytes = new List<byte>() { 4 };

            bytes.AddRange(Scripts.GetNameBytes(tag.Name));
            bytes.AddRange(GetPayloadBytes(tag.Value));

            return bytes;
        }
        public static List<byte> GetPayloadBytes(long value)
        {
            List<byte> bytes = new List<byte>();

            byte[] longBytes = BitConverter.GetBytes(value);

            bytes.AddRange(new byte[] {
                longBytes[7], longBytes[6], longBytes[5], longBytes[4],
                longBytes[3], longBytes[2], longBytes[1], longBytes[0]
            });

            return bytes;
        }

        public bool Equals(LongTag tag){
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

using System;
using System.Collections.Generic;

namespace NBTEditorV2 {
    public class DoubleTag {
        public string Name { get; set; }
        public double Value { get; set; }

        public DoubleTag(string name, double value)
        {
            this.Name = name;
            this.Value = value;
        }

        public static DoubleTag LoadTag(byte[] bytes, ref int startIndex)
        {
            startIndex++;

            string name = Scripts.GetName(bytes, ref startIndex);
            
            double value = LoadPayload(bytes, ref startIndex);

            return new DoubleTag(name, value);
        }
        public static double LoadPayload(byte[] bytes, ref int startIndex)
        {
            double value = BitConverter.ToDouble(new byte[] {
                bytes[startIndex + 7], bytes[startIndex + 6], bytes[startIndex + 5], bytes[startIndex + 4],
                bytes[startIndex + 3], bytes[startIndex + 2], bytes[startIndex + 1], bytes[startIndex]

            }, 0);
            startIndex += 8;

            return value;
        }

        public static List<byte> GetTagBytes(DoubleTag tag)
        {
            List<byte> bytes = new List<byte>() { 6 };

            bytes.AddRange(Scripts.GetNameBytes(tag.Name));
            bytes.AddRange(GetPayloadBytes(tag.Value));

            return bytes;
        }
        public static List<byte> GetPayloadBytes(double value)
        {
            List<byte> bytes = new List<byte>();

            byte[] doubleBytes = BitConverter.GetBytes(value);

            bytes.AddRange(new byte[] {
                doubleBytes[7], doubleBytes[6], doubleBytes[5], doubleBytes[4],
                doubleBytes[3], doubleBytes[2], doubleBytes[1], doubleBytes[0]
            });

            return bytes;
        }

        public bool Equals(DoubleTag tag){
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

using System;
using System.Collections.Generic;

namespace NBTEditorV2 {
    public class FloatTag {
        public string Name { get; set; }
        public float Value { get; set; }

        public FloatTag(string name, float value)
        {
            this.Name = name;
            this.Value = value;
        }

        public static FloatTag LoadTag(byte[] bytes, ref int startIndex)
        {
            startIndex++;

            string name = Scripts.GetName(bytes, ref startIndex);
            
            float value = LoadPayload(bytes, ref startIndex);

            return new FloatTag(name, value);
        }
        public static float LoadPayload(byte[] bytes, ref int startIndex)
        {
            float value = BitConverter.ToSingle(new byte[] { bytes[startIndex + 3], bytes[startIndex + 2], bytes[startIndex + 1], bytes[startIndex] }, 0);
            startIndex += 4;

            return value;
        }

        public static List<byte> GetTagBytes(FloatTag tag)
        {
            List<byte> bytes = new List<byte>() { 5 };

            bytes.AddRange(Scripts.GetNameBytes(tag.Name));
            bytes.AddRange(GetPayloadBytes(tag.Value));

            return bytes;
        }
        public static List<byte> GetPayloadBytes(float value)
        {
            List<byte> bytes = new List<byte>();

            byte[] floatBytes = BitConverter.GetBytes(value);

            bytes.AddRange(new byte[] {floatBytes[3], floatBytes[2], floatBytes[1], floatBytes[0]});

            return bytes;
        }

        public bool Equals(FloatTag tag){
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

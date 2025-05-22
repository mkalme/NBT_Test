using System;
using System.Collections.Generic;

namespace NBTEditorV2 {
    public class IntArrayTag {
        public string Name { get; set; }
        public int[] Array { get; set; }

        public IntArrayTag(string name, int[] array)
        {
            this.Name = name;
            this.Array = array;
        }

        public static IntArrayTag LoadTag(byte[] bytes, ref int startIndex)
        {
            startIndex++;

            string name = Scripts.GetName(bytes, ref startIndex);
            
            int[] array = LoadPayload(bytes, ref startIndex);

            return new IntArrayTag(name, array);
        }
        public static int[] LoadPayload(byte[] bytes, ref int startIndex)
        {
            int length = BitConverter.ToInt32(new byte[] { bytes[startIndex + 3], bytes[startIndex + 2], bytes[startIndex + 1], bytes[startIndex] }, 0);
            startIndex += 4;

            int[] array = new int[length];
            for (int i = 0; i < length; i++) {
                //array[i] = Scripts.BytesToInt32(bytes, startIndex);
                array[i] = BitConverter.ToInt32(new byte[] { bytes[startIndex + 3], bytes[startIndex + 2], bytes[startIndex + 1], bytes[startIndex] }, 0);

                startIndex += 4;
            }

            return array;
        }

        public static List<byte> GetTagBytes(IntArrayTag tag)
        {
            List<byte> bytes = new List<byte>() { 11 };

            bytes.AddRange(Scripts.GetNameBytes(tag.Name));
            bytes.AddRange(GetPayloadBytes(tag.Array));

            return bytes;
        }
        public static List<byte> GetPayloadBytes(int[] array)
        {
            List<byte> bytes = new List<byte>();

            byte[] lengthBytes = BitConverter.GetBytes(array.Length);
            bytes.AddRange(new byte[] {lengthBytes[3], lengthBytes[2], lengthBytes[1], lengthBytes[0]});

            for (int i = 0; i < array.Length; i++) {
                byte[] arrayBytes = BitConverter.GetBytes(array[i]);

                bytes.AddRange(new byte[] {arrayBytes[3], arrayBytes[2], arrayBytes[1], arrayBytes[0]});
            }

            return bytes;
        }

        public bool Equals(IntArrayTag tag){
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

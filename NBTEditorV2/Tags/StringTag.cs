using System;
using System.Text;
using System.Collections.Generic;

namespace NBTEditorV2 {
    public class StringTag {
        public string Name { get; set; }
        public string Text { get; set; }

        public StringTag(string name, string text) {
            this.Name = name;
            this.Text = text;
        }

        public static StringTag LoadTag(byte[] bytes, ref int startIndex) {
            startIndex++;

            string name = Scripts.GetName(bytes, ref startIndex);

            string text = LoadPayload(bytes, ref startIndex);

            return new StringTag(name, text);
        }
        public static string LoadPayload(byte[] bytes, ref int startIndex) {
            int length = (bytes[startIndex] * 256) + bytes[startIndex + 1];

            string text = Encoding.UTF8.GetString(bytes, startIndex + 2, length);
            startIndex += 2 + length;

            return text;
        }

        public static List<byte> GetTagBytes(StringTag tag) {
            List<byte> bytes = new List<byte>() {8};

            bytes.AddRange(Scripts.GetNameBytes(tag.Name));
            bytes.AddRange(GetPayloadBytes(tag.Text));

            return bytes;
        }
        public static List<byte> GetPayloadBytes(string text) {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(new byte[] { (byte)(text.Length >> 8), (byte)(text.Length & 255) });
            bytes.AddRange(Encoding.ASCII.GetBytes(text));

            return bytes;
        }

        public bool Equals(StringTag tag) {
            if (Name != tag.Name) {
                return false;
            }
            if (Text != tag.Text) {
                return false;
            }

            return true;
        }
    }
}

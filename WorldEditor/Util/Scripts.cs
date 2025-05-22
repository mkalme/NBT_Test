using System;
using System.Collections.Generic;
using System.Collections;

namespace WorldEditor
{
    class Scripts
    {
        public static int Mod(int number, int mod)
        {
            int result = number % mod;

            if (result < 0) {
                result += mod;
            }

            return result;
        }
        public static byte[] Subset(byte[] bytes, int start, int characters)
        {
            byte[] array = new byte[characters];

            for (int i = 0; i < characters; i++){
                array[i] = bytes[start + i];
            }

            return array;
        }

        public static List<T> Shuffle<T>(List<T> list){
            List<T> randomizedList = new List<T>();
            Random rnd = new Random();

            while (list.Count > 0) {
                int index = rnd.Next(0, list.Count); //pick a random item from the master list
                
                randomizedList.Add(list[index]); //place it at the end of the randomized list
                
                list.RemoveAt(index);
            }

            return randomizedList;
        }

        public static long[] ReverseBitsInLongArray(long[] xArray)
        {
            long[] array = new long[xArray.Length];

            for (int i = 0; i < array.Length; i++) {
                long x = xArray[i];
                long y = 0;

                for (int b = 0; b < 64; ++b) {
                    y <<= 1;
                    y |= (x & 1);
                    x >>= 1;
                }

                array[i] = y;
            }

            return array;
        }

        public static int PaletteIndex(short id, List<Property> properties, List<Block> palette){
            if (palette.Count > 0){
                for (int i = 0; i < palette.Count; i++){
                    if (palette[i].Equals(id, properties)) {
                        return i;
                    }
                }
            }

            return -1;
        }

        public static int GetBitLength(int paletteCount)
        {
            int length = 4;
            if (paletteCount > 16) {
                //length = (int)Math.Ceiling(Math.Sqrt(paletteCount));

                while (Math.Pow(2, length) < paletteCount) {
                    length++;
                }
            }

            return length;
        }
        public static int GetIntBasedOnBits(BitArray bits, int start, int characters)
        {
            int number = 0;

            for (int i = start; i < start + characters; i++) {
                if (bits[i]) { //If 1
                    number += 1 << (i - start);
                }
            }

            return number;
        }

        public static long GetLongBasesOnBits(BitArray bits, int start)
        {
            byte[] bytes = new byte[8];

            for (int i = 0; i < 8; i++) {
                bytes[i] = GetByteBasedOnBits(bits, start + i * 8);
            }

            return BitConverter.ToInt64(bytes, 0);
        }
        public static byte GetByteBasedOnBits(BitArray bits, int start)
        {
            int number = 0;

            for (int i = start; i < start + 8; i++){
                if (bits[i]){ //If 1
                    number += 1 << i - start;
                }
            }

            return (byte)number;
        }
    }
}

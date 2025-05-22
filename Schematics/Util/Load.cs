using System;
using System.Collections.Generic;
using System.IO;
using NBTEditorV2;
using WorldEditor;

namespace Schematics {
    class Load {
        public static Schematic FromFile(string filePath) {
            byte[] bytes = File.ReadAllBytes(filePath);
            CompoundTag tag = CompoundTag.LoadTag(bytes);

            int width = tag.GetShortTag("Width").Value;
            int height = tag.GetShortTag("Height").Value;
            int length = tag.GetShortTag("Length").Value;

            Schematic schematic = new Schematic(width, height, length);

            CompoundTag dataTag = tag.GetCompoundTag("Data");
            SetData(dataTag, ref schematic);

            return schematic;
        }

        private static void SetData(CompoundTag tag, ref Schematic schematic) {
            //Get palette
            ListTag palette = tag.GetListTag("Palette");

            List<Block> paletteList = new List<Block>();
            for (int i = 0; i < palette.Payload.Elements.Count; i++) {
                List<Tag> compoundPalette = (List<Tag>)palette.Payload.Elements[i];
                CompoundTag compoundPaletteTag = new CompoundTag("", compoundPalette);

                //Get namespace
                string nameSpace = compoundPaletteTag.GetStringTag("Name").Text;

                //Get properties
                List<Property> palettePropperties = new List<Property>();

                CompoundTag propertyCompound = compoundPaletteTag.GetCompoundTag("Properties");
                if (propertyCompound != null) {
                    for (int b = 0; b < propertyCompound.GetSize(); b++) {
                        if (propertyCompound.TagAt(b).Type == TagType.String) {
                            string name = propertyCompound.TagAt(b).Name;
                            string value = ((StringTag)propertyCompound.TagAt(b).Item).Text;

                            palettePropperties.Add(new Property(name, value));
                        }
                    }
                }

                paletteList.Add(new Block(ID.BlockIDReverse[nameSpace], palettePropperties));
            }

            //Get length
            int length = Scripts.GetBitLength(palette.Payload.Elements.Count);

            //Set blockstate & properties
            long[] longs = tag.GetLongArrayTag("BlockStates").Array;

            int reminder = 64 - length;
            int blockIndex = 0;
            int carryOver = 0;
            long carryValue = 0;

            Size size = schematic.Size;
            int planeSize = size.Width * size.Length;
            int blockCount = planeSize * size.Height;

            for (int i = 0; i < longs.Length; i++) {
                int startPos = 64;

                //Reminder end
                if (carryOver > 0) {
                    int left = length - carryOver;
                    int bitsIn = startPos - left;

                    long secondValue = (long)((ulong)(longs[i] << bitsIn) >> (64 - left));
                    long index = (secondValue << carryOver) + carryValue;

                    //Set block
                    int y = blockIndex / planeSize;
                    int currentPlane = blockIndex % planeSize;
                    int z = currentPlane / size.Width;
                    int x = currentPlane % size.Width;

                    Block block = paletteList[(int)index];
                    schematic.SetBlock(x, y, z, block.ID, block.Properties);

                    blockIndex++;
                    startPos = bitsIn;

                    if (blockIndex == blockCount) {
                        return;
                    }
                }

                //Regular
                int nIndexes = startPos / length;
                for (int b = 0; b < nIndexes; b++) {
                    startPos -= length;

                    int index = (int)((ulong)(longs[i] << startPos) >> reminder);

                    //Set block
                    int y = blockIndex / planeSize;
                    int currentPlane = blockIndex % planeSize;
                    int z = currentPlane / size.Width;
                    int x = currentPlane % size.Width;

                    Block block = paletteList[index];
                    schematic.SetBlock(x, y, z, block.ID, block.Properties);

                    blockIndex++;

                    if (blockIndex == blockCount) {
                        return;
                    }
                }

                //Reminder start
                carryOver = startPos;
                if (carryOver > 0) {
                    carryValue = (long)((ulong)longs[i] >> 64 - startPos);
                }
            }
        }
    }
}

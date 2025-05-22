using System;
using System.Collections.Generic;
using NBTEditorV2;
using WorldEditor;

namespace Schematics {
    class Save {
        public static CompoundTag SaveToFile(Schematic schematic) {
            CompoundTag tag = new CompoundTag("Schematic");

            tag.Add(new ShortTag("Width", (short)schematic.Size.Width));
            tag.Add(new ShortTag("Height", (short)schematic.Size.Height));
            tag.Add(new ShortTag("Length", (short)schematic.Size.Length));

            CompoundTag data;
            SetData(schematic, out data);

            tag.Add(data);
            tag.Add(new StringTag("Version", "0.2"));

            return tag;
        }

        private static void SetData(Schematic schematic, out CompoundTag dataTag) {
            int[] blockIndexes = new int[schematic.Size.Width * schematic.Size.Height * schematic.Size.Length];

            //Add palette
            List<Block> palette = new List<Block>();

            int blockIndex = 0;
            for (int y = 0; y < schematic.Size.Height; y++) {
                for (int z = 0; z < schematic.Size.Length; z++) {
                    for (int x = 0; x < schematic.Size.Width; x++) {
                        short id = schematic.GetID(x, y, z);
                        List<Property> properties = schematic.GetProperties(x, y, z);

                        if (properties == null) {
                            properties = new List<Property>();
                        }

                        //Check if palette contains
                        int paletteIndex = Scripts.PaletteIndex(id, properties, palette);

                        if (paletteIndex < 0) {
                            palette.Add(new Block(id, properties));

                            paletteIndex = palette.Count - 1;
                        }

                        blockIndexes[blockIndex] = paletteIndex;

                        blockIndex++;
                    }
                }
            }

            ListPayload palettePayload = new ListPayload(TagType.Compound, new List<object>());
            for (int i = 0; i < palette.Count; i++) {
                List<Tag> tag = new List<Tag>();

                //Add namespace
                StringTag nameSpaceTag = new StringTag("Name", ID.BlockID[palette[i].ID]);
                tag.Add(new Tag(TagType.String, nameSpaceTag.Name, nameSpaceTag));

                //Add properties
                if (palette[i].Properties.Count > 0) {
                    CompoundTag propertyTag = new CompoundTag("Properties", new List<Tag>());

                    for (int b = 0; b < palette[i].Properties.Count; b++) {
                        StringTag propertyValue = new StringTag(palette[i].Properties[b].Name, palette[i].Properties[b].Value);
                        propertyTag.Add(new Tag(TagType.String, propertyValue.Name, propertyValue));
                    }

                    tag.Add(new Tag(TagType.Compound, propertyTag.Name, propertyTag));
                }

                palettePayload.Elements.Add(tag);
            }

            ListTag paletteTag = new ListTag("Palette", palettePayload);

            //Add blockstates
            int length = Scripts.GetBitLength(palettePayload.Elements.Count);

            int numberOfBlocks = schematic.Size.Width * schematic.Size.Height * schematic.Size.Length;
            long[] longArray = new long[Scripts.CeilingMultiple(numberOfBlocks * length, 64) / 64];

            int index = 0;
            int carryOver = 0;
            int moveSecondValue = 32 - length;
            for (int i = 0; i < longArray.Length; i++) {
                int startPos = 64;
                long value = 0;

                //Reminder end
                if (carryOver > 0) {
                    int left = length - carryOver;
                    int bitsIn = startPos - left;

                    value = (int)((uint)(blockIndexes[index] << moveSecondValue) >> (32 - left));

                    index++;
                    startPos = bitsIn;

                    if (index == numberOfBlocks) {
                        longArray[i] = value;
                        goto after_loop;
                    }
                }

                //Regular
                int nIndexes = startPos / length;
                for (int b = 0; b < nIndexes; b++) {
                    value += (long)blockIndexes[index] << (64 - startPos);

                    startPos -= length;

                    index++;

                    if (index == numberOfBlocks) {
                        longArray[i] = value;
                        goto after_loop;
                    }
                }

                //Reminder start
                carryOver = startPos;
                if (carryOver > 0) {
                    int reminder = 64 - startPos;
                    int firstValue = (int)((uint)(blockIndexes[index] << reminder) >> (reminder));

                    value += (long)firstValue << reminder;
                }

                //Set the value
                longArray[i] = value;
            }
            after_loop:

            LongArrayTag blockStatesTag = new LongArrayTag("BlockStates", longArray);

            dataTag = new CompoundTag("Data");

            dataTag.Add(paletteTag);
            dataTag.Add(blockStatesTag);
        }
    }
}

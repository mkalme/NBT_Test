using System;
using System.Collections.Generic;
using NBTEditorV2;

namespace WorldEditor {
    class Palette {
        public Block[] Blocks { get; set; }

        public Palette()
        {
            this.Blocks = new Block[0];
        }
        public Palette(Block[] blocks)
        {
            this.Blocks = blocks;
        }

        public static Palette FromListTag(ListTag listTag)
        {
            Block[] blocks = new Block[listTag.Payload.Elements.Count];

            for (int i = 0; i < listTag.Payload.Elements.Count; i++) {
                List<Tag> compoundPalette = (List<Tag>)listTag.Payload.Elements[i];
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

                blocks[i] = new Block(ID.BlockIDReverse[nameSpace], palettePropperties);
            }

            return new Palette(blocks);
        }
        public static Palette FromSection(Section section, ref int[] blockIndexes)
        {
            List<Block> paletteBlock = new List<Block>();

            for (int i = 0; i < 4096; i++) {
                short id = section.BlockID[i];
                List<Property> properties = section.GetProperties((short)i, true);

                if (properties != null) {
                    if (properties.Count == 0) {
                        properties = null;
                    }
                }

                //Check if palette contains
                int paletteIndex = Scripts.PaletteIndex(id, properties, paletteBlock);
                if (paletteIndex < 0) {
                    paletteBlock.Add(new Block(id, properties));

                    paletteIndex = paletteBlock.Count - 1;
                }

                blockIndexes[i] = paletteIndex;
            }
            for (int i = 0; i < paletteBlock.Count; i++) {
                Block block = paletteBlock[i];
                if (block.Properties == null) {
                    block.Properties = new List<Property>();
                }
            }

            return new Palette(paletteBlock.ToArray());
        }

        public ListTag GetListTag()
        {
            ListPayload palettePayload = new ListPayload(TagType.Compound, new List<object>());

            for (int i = 0; i < Blocks.Length; i++) {
                List<Tag> tag = new List<Tag>();

                //Add namespace
                StringTag nameSpaceTag = new StringTag("Name", ID.BlockID[Blocks[i].ID]);
                tag.Add(new Tag(TagType.String, nameSpaceTag.Name, nameSpaceTag));

                //Add properties
                if (Blocks[i].Properties.Count > 0) {
                    CompoundTag propertyTag = new CompoundTag("Properties", new List<Tag>());

                    for (int b = 0; b < Blocks[i].Properties.Count; b++) {
                        StringTag propertyValue = new StringTag(Blocks[i].Properties[b].Name, Blocks[i].Properties[b].Value);
                        propertyTag.Add(new Tag(TagType.String, propertyValue.Name, propertyValue));
                    }

                    tag.Add(new Tag(TagType.Compound, propertyTag.Name, propertyTag));
                }

                palettePayload.Elements.Add(tag);
            }

            return new ListTag("Palette", palettePayload);
        }
    }
}

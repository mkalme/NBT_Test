using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using WorldEditor;
using System.Linq;

namespace RegionMapping
{
    public class BlockColors
    {
        public static Dictionary<short, Color> BlockColor = new Dictionary<short, Color>();

        public static Dictionary<short, Dictionary<string, Color>> PropertyColor = new Dictionary<short, Dictionary<string, Color>>();
        public static Dictionary<short, Color> DefaultPropertyColor = new Dictionary<short, Color>();

        public static Dictionary<short, Dictionary<short, Color>> BiomeColors = new Dictionary<short, Dictionary<short, Color>>();
        public static Dictionary<short, Color> BiomeColorList = new Dictionary<short, Color>();

        public static Dictionary<int, List<short>> ColorTypeList = new Dictionary<int, List<short>>();

        public static readonly int Divider = 80;

        static BlockColors() {
            XmlDocument document = new XmlDocument();
            document.Load(@"Assets\ColorStorage.xml");

            SetBlockColor(document, "block", BlockColor, ID.BlockIDReverse);

            SetPropertyColor(document);
            SetDefaultProeprtyColor(document);

            SetBiomeColor(document);
            SetBiomeColorList(document);

            SetColorTypeList();
        }
        private static void SetBlockColor(XmlDocument document, string name, Dictionary<short, Color> dictionary, Dictionary<string, short> idDictionary) {
            XmlNodeList colorNodes = document.SelectNodes("/root/" + name + "/color");

            for (int i = 0; i < colorNodes.Count; i++){
                Tuple<string, Color> colorT = GetColorFromNode(colorNodes[i]);

                dictionary.Add(idDictionary[colorT.Item1], colorT.Item2);
            }
        }
        private static void SetPropertyColor(XmlDocument document) {
            XmlNodeList propertyNodes = document.SelectNodes("/root/block-property/property");

            for (int i = 0; i < propertyNodes.Count; i++) {
                short id = ID.BlockIDReverse[propertyNodes[i].Attributes["namespace"].Value];
                XmlNodeList colorNodes = propertyNodes[i].SelectNodes("color");

                Dictionary<string, Color> propertyDictionary = new Dictionary<string, Color>();
                for (int b = 0; b < colorNodes.Count; b++) {
                    string value = colorNodes[b].Attributes["value"].Value;
                    Color color = ColorTranslator.FromHtml(colorNodes[b].Attributes["hex"].Value);

                    propertyDictionary.Add(value, color);
                }

                PropertyColor.Add(id, propertyDictionary);
            }
        }
        private static void SetDefaultProeprtyColor(XmlDocument document) {
            XmlNodeList propertyNodes = document.SelectNodes("/root/block-property/property");

            for (int i = 0; i < propertyNodes.Count; i++) {
                short id = ID.BlockIDReverse[propertyNodes[i].Attributes["namespace"].Value];
                Color defaultColor = ColorTranslator.FromHtml(propertyNodes[i].SelectSingleNode("default").Attributes["hex"].Value);

                DefaultPropertyColor.Add(id, defaultColor);
            }
        }
        private static void SetBiomeColor(XmlDocument document) {
            XmlNodeList biomeNodes = document.SelectNodes("/root/biome/place");

            for (int i = 0; i < biomeNodes.Count; i++) {
                short biomeID = ID.BiomeIDReverse[biomeNodes[i].Attributes["namespace"].Value];
                XmlNodeList colorNodes = biomeNodes[i].SelectNodes("color");

                Dictionary<short, Color> biomeDictionary = new Dictionary<short, Color>();
                for (int b = 0; b < colorNodes.Count; b++) {
                    Tuple<string, Color> colorT = GetColorFromNode(colorNodes[b]);
                    Color blockColor = colorT.Item2;

                    if (colorT.Item1 == "minecraft:water") blockColor = ChangeColorBrightness(blockColor, 0.546875F, ColorChangeType.Multiply);

                    biomeDictionary.Add(ID.BlockIDReverse[colorT.Item1], blockColor);
                }

                BiomeColors.Add(biomeID, biomeDictionary);
            }
        }
        private static void SetBiomeColorList(XmlDocument document) {
            XmlNodeList colorList = document.SelectNodes("/root/biome/list/color");

            for (int i = 0; i < colorList.Count; i++) {
                Tuple<string, Color> colorT = GetColorFromNode(colorList[i]);
                Color blockColor = colorT.Item2;

                if (colorT.Item1 == "minecraft:water") blockColor = ChangeColorBrightness(blockColor, 0.546875F, ColorChangeType.Multiply);

                BiomeColorList.Add(ID.BlockIDReverse[colorT.Item1], blockColor);
            }
        }

        private static void SetColorTypeList() {
            for (int i = 0; i < BlockColor.Count; i++) {
                short id = BlockColor.ElementAt(i).Key;

                int newID = id / Divider;
                if (!ColorTypeList.ContainsKey(newID)) {
                    List<short> newList = new List<short>(id);

                    ColorTypeList.Add(newID, newList);
                } else {
                    ColorTypeList[newID].Add(id);
                }
            }
        }
        public static BlockColorType GetType(short id) {
            int newID = id / Divider;

            if (ColorTypeList.ContainsKey(newID)) {
                if (ColorTypeList[newID].Contains(id)) {
                    return BlockColorType.Block;
                }
            } else if (BiomeColorList.ContainsKey(id)) {
                return BlockColorType.Biome;
            } else if (PropertyColor.ContainsKey(id)) {
                return BlockColorType.Property;
            }

            return BlockColorType.None;
        }

        private static Tuple<string, Color> GetColorFromNode(XmlNode node) {
            return Tuple.Create(node.Attributes["namespace"].Value, ColorTranslator.FromHtml(node.Attributes["hex"].Value));
        }

        private static Color ChangeColorBrightness(Color color, float value, ColorChangeType type)
        {
            float r = color.R;
            float g = color.G;
            float b = color.B;

            if (type == ColorChangeType.Add) {
                r += value;
                g += value;
                b += value;
            } else if (type == ColorChangeType.Multiply) {
                r *= value;
                g *= value;
                b *= value;
            } else if (type == ColorChangeType.Set) {
                r = value;
                g = value;
                b = value;
            }

            r = r > 255 ? 255 : r;
            g = g > 255 ? 255 : g;
            b = b > 255 ? 255 : b;

            r = r < 0 ? 0 : r;
            g = g < 0 ? 0 : g;
            b = b < 0 ? 0 : b;

            return Color.FromArgb(color.A, (int)r, (int)g, (int)b);
        }
    }
}

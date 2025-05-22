using System;
using System.Collections.Generic;
using System.Xml;

namespace WorldEditor
{
    public class ID
    {
        public static Dictionary<short, string> BlockID = new Dictionary<short, string>();
        public static Dictionary<string, short> BlockIDReverse = new Dictionary<string, short>();

        public static Dictionary<short, string> BiomeID = new Dictionary<short, string>();
        public static Dictionary<string, short> BiomeIDReverse = new Dictionary<string, short>();

        public static Dictionary<short, string> ItemID = new Dictionary<short, string>();
        public static Dictionary<string, short> ItemIDReverse = new Dictionary<string, short>();

        public static Dictionary<short, string> EntityID = new Dictionary<short, string>();
        public static Dictionary<string, short> EntityIDReverse = new Dictionary<string, short>();

        static ID() {
            XmlDocument document = new XmlDocument();
            document.Load(@"Assets\IDStorage.xml");

            //Set block ids
            SetID(document, "block", BlockID, BlockIDReverse);
            SetID(document, "biome", BiomeID, BiomeIDReverse);
            SetID(document, "item", ItemID, ItemIDReverse);
            SetID(document, "entity", EntityID, EntityIDReverse);

            //Test
            //document.Load(@"Assets\PreFlatteningIDStorage.xml");
            //XmlNodeList idNodes = document.SelectNodes("/block/id");

            //for (int i = 0; i < idNodes.Count; i++) {
            //    string id = idNodes[i].Attributes["short"].Value;
            //    string nameSpace = idNodes[i].Attributes["namespace"].Value;

            //    if (!BlockIDReverse.ContainsKey(nameSpace)) {
            //        System.Diagnostics.Debug.WriteLine(id + ", " + nameSpace);
            //    }
            //}
        }
        private static void SetID(XmlDocument document, string name, Dictionary<short, string> id, Dictionary<string, short> idReverse) {
            XmlNodeList idNodes = document.SelectNodes("/root/" + name + "/id");

            for (int i = 0; i < idNodes.Count; i++)
            {
                Tuple<short, string> idT = GetIDFromNode(idNodes[i]);

                id.Add(idT.Item1, idT.Item2);
                idReverse.Add(idT.Item2, idT.Item1);
            }
        }
        private static Tuple<short, string> GetIDFromNode(XmlNode node){
            return Tuple.Create(Convert.ToInt16(node.Attributes["short"].Value.ToString()), node.Attributes["namespace"].Value.ToString());
        }
    }
}

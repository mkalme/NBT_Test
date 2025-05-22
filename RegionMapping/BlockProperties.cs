using System;
using System.Collections.Generic;
using System.Xml;
using WorldEditor;

namespace RegionMapping {
    public class BlockProperties {
        public static Dictionary<short, int> ExceptionList = new Dictionary<short, int>();
        public static List<short> ExceptionUndefinedList = new List<short>();
        public static List<short> ExcludedList = new List<short>();
        static BlockProperties()
        {
            XmlDocument document = new XmlDocument();
            document.Load(@"Assets\BlockPropertyStorage.xml");

            SetExceptionList(document);
            SetHeightUndefiniedList(document);
            SetExcludedList(document);
        }

        private static void SetExceptionList(XmlDocument document)
        {
            XmlNodeList blockNodes = document.SelectNodes("/root/height-exception/block");

            for (int i = 0; i < blockNodes.Count; i++) {
                short id = ID.BlockIDReverse[blockNodes[i].Attributes["namespace"].Value];
                int height = Int32.Parse(blockNodes[i].Attributes["ignore"].Value);

                ExceptionList.Add(id, height);
            }
        }
        private static void SetHeightUndefiniedList(XmlDocument document) {
            XmlNodeList blockNodes = document.SelectNodes("/root/height-exception-undefined/block");

            for (int i = 0; i < blockNodes.Count; i++) {
                short id = ID.BlockIDReverse[blockNodes[i].Attributes["namespace"].Value];

                ExceptionUndefinedList.Add(id);
            }
        }
        private static void SetExcludedList(XmlDocument document) {
            XmlNodeList blockNodes = document.SelectNodes("/root/shadow-exception/block");

            for (int i = 0; i < blockNodes.Count; i++) {
                short id = ID.BlockIDReverse[blockNodes[i].Attributes["namespace"].Value];

                ExcludedList.Add(id);
            }
        }
    }
}

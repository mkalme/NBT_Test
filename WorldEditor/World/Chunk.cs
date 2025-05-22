using System;
using System.Collections.Generic;
using NBTEditorV2;

namespace WorldEditor {
    public class Chunk{
        public List<Section> Sections { get; set; }
        public int[,] Biomes { get; set; }
        public long LastUpdate { get; set; }
        public string Status { get; set; }
        public int Xpos { get; set; }
        public int Zpos { get; set; }
        public int DataVersion { get; set; }

        public CompoundTag Level { get; set; }

        public Chunk()
        {
            SetTags();
            this.Sections = new List<Section>();

            this.Xpos = 0;
            this.Zpos = 0;
            this.Biomes = new int[16, 16];
        }
        public Chunk(int xPos, int zPos, List<Section> sections, int[,] biomes)
        {
            SetTags();
            this.Sections = sections;

            this.Xpos = xPos;
            this.Zpos = zPos;
            this.Biomes = biomes;
        }

        private void SetTags()
        {
            this.DataVersion = 1976;
            this.LastUpdate = 0;
            this.Status = "";
        }

        public Section GetSection(int yIndex)
        {
            for (int i = 0; i < Sections.Count; i++) {
                if (Sections[i].YIndex == yIndex) {
                    return Sections[i];
                }
            }

            return null;
        }
    }
}

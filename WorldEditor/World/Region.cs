using System.Collections.Generic;

namespace WorldEditor
{
    public class Region
    {
        public int XIndex { get; set; }
        public int ZIndex { get; set; }

        public List<Chunk> Chunks { get; set; }

        public Region() {
            this.XIndex = 0;
            this.ZIndex = 0;
            this.Chunks = new List<Chunk>();
        }
        public Region(int xIndex, int zIndex, List<Chunk> chunks) {
            this.XIndex = xIndex;
            this.ZIndex = zIndex;
            this.Chunks = chunks;
        }

        public Chunk GetChunk(int xPos, int zPos){
            for (int i = 0; i < Chunks.Count; i++) {
                if (Chunks[i].Xpos == xPos && Chunks[i].Zpos == zPos) {
                    return Chunks[i];
                }
            }

            return null;
        }

        public static Region LoadRegion(string worldPath, int xIndex, int zIndex) {
            //Check if exists
            string filePath = worldPath + @"\region\" + "r." + xIndex + "." + zIndex + ".mca";

            return Load.LoadRegion(filePath, xIndex, zIndex);
        }
    }
}

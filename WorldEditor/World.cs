using System;
using System.Collections.Generic;
using NBTEditorV2;

namespace WorldEditor
{
    public class World
    {
        public string Path { get; set; }
        public List<Region> Regions { get; set; }
        public CompoundTag Level { get; set; }

        public World() {
            this.Path = "";
            this.Regions = new List<Region>();
        }
        public World(string path) {
            this.Path = path;
            this.Regions = new List<Region>();
        }

        //Load
        public void LoadLevel() {
            Level = Load.LoadLevel(Path + @"\level.dat");
        }
        public void LoadRegion(int xIndex, int zIndex)
        {
            //Check if exists
            string filePath = Path + @"\region\" + "r." + xIndex + "." + zIndex + ".mca";
            
            Region region = Load.LoadRegion(filePath, xIndex, zIndex);

            if (region != null) {
                Regions.Add(region);
            }
        }

        //Save
        public void SaveLevel() {
            Save.SaveLevel(Path + @"\level.dat", Level);
        }
        public void SaveRegion(int xIndex, int zIndex) {
            Region region = GetRegion(xIndex * 512, zIndex * 512);

            if (region != null) {
                Save.SaveRegion(Path + @"\region\r." + region.XIndex + "." + region.ZIndex + ".mca", region);
            }
        }

        public void SaveAllRegions() {
            for (int i = 0; i < Regions.Count; i++) {
                Save.SaveRegion(Path + @"\region\r." + Regions[i].XIndex + "." + Regions[i].ZIndex + ".mca", Regions[i]);
            }
        }

        //Functions
        public Region GetRegionByIndex(int xIndex, int zIndex) {
            for (int i = 0; i < Regions.Count; i++) {
                if (Regions[i].XIndex == xIndex && Regions[i].ZIndex == zIndex) {
                    return Regions[i];
                }
            }

            return null;
        }
        public Region GetRegion(int x, int z) {
            int xRegionIndex = (int)Math.Floor((double)x / 512.0);
            int zRegionIndex = (int)Math.Floor((double)z / 512.0);

            for (int i = 0; i < Regions.Count; i++){
                if (Regions[i].XIndex == xRegionIndex && Regions[i].ZIndex == zRegionIndex){
                    return Regions[i];
                }
            }

            return null;
        }
        public Chunk GetChunk(int x, int z) {
            Region region = GetRegion(x, z);

            if (region == null) {
                return null;
            }

            int xChunkIndex = (int)Math.Floor((double)x / 16.0);
            int zChunkIndex = (int)Math.Floor((double)z / 16.0);

            for (int i = 0; i < region.Chunks.Count; i++){
                if (region.Chunks[i].Xpos == xChunkIndex && region.Chunks[i].Zpos == zChunkIndex){
                    return region.Chunks[i];
                }
            }

            return null;
        }
        public Section GetSection(int x, int y, int z) {
            Chunk chunk = GetChunk(x, z);

            if (chunk == null) {
                return null;
            }

            int ySectionIndex = (int)Math.Floor((double)y / 16.0);

            for (int i = 0; i < chunk.Sections.Count; i++){
                if (chunk.Sections[i].YIndex == ySectionIndex){
                    return chunk.Sections[i];
                }
            }

            return null;
        }
        public Block GetBlock(int x, int y, int z) {
            Section section = GetSection(x, y, z);

            if (section == null) {
                return null;
            }

            //Get block
            Block block = new Block();
            int xBlockIndex = x % 16;
            int yBlockIndex = y % 16;
            int zBlockIndex = z % 16;

            if (xBlockIndex < 0) { xBlockIndex += 16; }
            if (yBlockIndex < 0) { yBlockIndex += 16; }
            if (zBlockIndex < 0) { zBlockIndex += 16; }

            block.ID = section.GetBlockID(xBlockIndex, yBlockIndex, zBlockIndex);
            block.Properties = section.GetProperties(xBlockIndex, yBlockIndex, zBlockIndex);

            return block;
        }
        public BlockInfo GetBlockInfo(int x, int y, int z) {
            BlockInfo blockInfo = new BlockInfo();

            //Set block
            Block block = GetBlock(x, y, z);
            if (block == null) {
                return null;
            }
            blockInfo.Block = block;

            //Set biome
            Chunk chunk = GetChunk(x, z);
            if (chunk == null) {
                return null;
            }
            int xBiomeIndex = x % 16;
            int zBiomeIndex = z % 16;

            if (xBiomeIndex < 0) {xBiomeIndex += 16; }
            if (zBiomeIndex < 0) {zBiomeIndex += 16; }

            blockInfo.BiomeID = chunk.Biomes[xBiomeIndex, zBiomeIndex];

            //Set coordinates
            blockInfo.X = x;
            blockInfo.Y = y;
            blockInfo.Z = z;

            //Set chunk section
            blockInfo.SectionY = y / 16;

            //Set chunk
            blockInfo.ChunkX = (int)Math.Floor(x / 16.0);
            blockInfo.ChunkZ = (int)Math.Floor(z / 16.0);

            //Set region
            blockInfo.RegionX = (int)Math.Floor(x / 512.0);
            blockInfo.RegionZ = (int)Math.Floor(z / 512.0);

            return blockInfo;
        }

        public void RemoveRegion(int x, int z) {
            for (int i = 0; i < Regions.Count; i++) {
                if (Regions[i].XIndex == x && Regions[i].ZIndex == z) {
                    Regions.RemoveAt(i);

                    i = Regions.Count;
                }
            }
        }

        public void SetBlock(int xCords, int yCords, int zCords, Block block) {
            //Get section
            Section section = GetSection(xCords, yCords, zCords);

            //Set block
            section.SetBlockID(block.ID, xCords % 16, yCords % 16, zCords % 16);
            section.SetProperties(xCords % 16, yCords % 16, zCords % 16, block.Properties);
        }
    }
}

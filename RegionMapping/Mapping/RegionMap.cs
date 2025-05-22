using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using WorldEditor;
using System.Threading.Tasks;
using System.Diagnostics;

namespace RegionMapping {
    class RegionMap : Scripts{
        public List<WorldEditor.Region> Regions { get; set; }
        public HeightMap HeightMap { get; set; }

        private Render Render { get; set; }

        public RegionMap(List<WorldEditor.Region> regions, RenderSettings settings) {
            this.Regions = regions;
            this.Render = new Render(settings);
        }

        public Bitmap GetMap() {
            SetHeightMap();

            return Render.RenderMap(HeightMap);
        }
        public Bitmap GetMapTest() {
            SetHeightMap();

            List<HeightMap> heightMapList = new List<HeightMap>(new List<HeightMap>() { HeightMap }) {HeightMap };

            Bitmap[] bitmaps = new Bitmap[2];
            Parallel.For(0, bitmaps.Length, i => {
                bitmaps[i] = Render.RenderMap(heightMapList[i], (HeightMap.Height / 2) * i, 0, (HeightMap.Height / 2) * (i + 1), HeightMap.Width);
            });

            return JoinBitmaps(bitmaps);
        }

        private void SetHeightMap() {
            MapRange mapRange = MapRange.GetRange(Regions);

            int mapWidth = (mapRange.ZEnd - mapRange.ZStart + 1) * 512;
            int mapHeight = (mapRange.XEnd - mapRange.XStart + 1) * 512;

            HeightMap = new HeightMap(mapWidth, mapHeight, mapRange.XStart, mapRange.ZStart);

            int totalHeightMaps = 1;
            Parallel.For(0, Regions.Count, i => {
                SetRegionHeightMap(Regions[i], mapRange);

                Debug.WriteLine(totalHeightMaps + " / " + Regions.Count + " getting heightmaps");
                totalHeightMaps++;
            });
        }
        private void SetRegionHeightMap(WorldEditor.Region region, MapRange mapRange) {
            int xOffset = (region.XIndex - mapRange.XStart) * 512;
            int zOffset = (region.ZIndex - mapRange.ZStart) * 512;

            //Initialize heightmap
            for (int z = 0; z < 512; z++) {
                for (int x = 0; x < 512; x++) {
                    HeightMap.Columns[zOffset + z, xOffset + x] = new Column(ColumnType.Region);
                }
            }

            //Set heightmaps
            for (int c = 0; c < region.Chunks.Count; c++) {
                Chunk chunk = region.Chunks[c];
                List<Section> sectionsInOrder = chunk.Sections.OrderBy(o => o.YIndex).ToList();

                int chunkXpos = Mod(chunk.Xpos, 32);
                int chunkZpos = Mod(chunk.Zpos, 32);

                for (int z = 0; z < 16; z++) {
                    for (int x = 0; x < 16; x++) {
                        int xPixel = (chunkXpos * 16) + x + xOffset;
                        int zPixel = (chunkZpos * 16) + z + zOffset;

                        HeightMap.Columns[zPixel, xPixel] = GetColumn(chunk, sectionsInOrder, x, z);
                    }
                }
            }
        }

        private Bitmap JoinBitmaps(Bitmap[] bitmaps) {
            Bitmap bitmap = new Bitmap(bitmaps[0].Width, bitmaps[0].Height + bitmaps[1].Height);
            
            using (Graphics g = Graphics.FromImage(bitmap)) {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                int y = 0;
                for (int i = bitmaps.Length - 1; i > -1; i--) {
                    g.DrawImage(bitmaps[i], 0, y, bitmaps[i].Width, bitmaps[i].Height);

                    y += bitmaps[i].Height;

                    bitmaps[i].Dispose();
                }
            }

            return bitmap;
        }
    }
}

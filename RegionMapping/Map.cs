using System;
using System.Collections.Generic;
using System.Drawing;

namespace RegionMapping {
    public class Map{
        public RenderSettings RenderSettings = new RenderSettings();
        public Bitmap Image { get; set; }

        //Region Map
        public Bitmap GetRegionMap(List<WorldEditor.Region> regions){
            RegionMap regionMap = new RegionMap(regions, RenderSettings);

            Image = regionMap.GetMapTest();

            return Image;
        }
        
        //World Map
        public Bitmap GetWorldMap(string worldPath){
            WorldMap worldMap = new WorldMap(worldPath, RenderSettings);

            Image = worldMap.GetMap();

            return Image;
        }
        public Bitmap GetWorldMap(string worldPath, Range range){
            WorldMap worldMap = new WorldMap(worldPath, RenderSettings, range);

            Image = worldMap.GetMap();

            return Image;
        }
    }
}

using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using WorldEditor;
using CustomWorldGenerator;
using System.Linq;
using RegionMapping;
using Schematics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace NBT_Test
{
    public partial class Form1 : Form {
        private static World World { get; set; }
        private static readonly string WorldPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.minecraft\saves\New World (4)";
        //private static readonly string WorldPath = @"D:\Backup\Other Backups\Minecraft\Storage\saves (Storage)\New World (11)";
        private static readonly string MappingFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Mapping\Saved Maps";
        private static readonly string MappingFolderLayer = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Mapping\Layers";
        private static Map Map { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            World = new World(WorldPath);

            //TestSpeed(20);

            TimeSpan timeSpan;
            DateTime time = DateTime.Now;

            //string text = ID.BlockID[0];

            LoadWorld();

            //RenderLayers(140, 2, MappingFolderLayer);

            //Map = new Map();
            //Map.GetRegionMap(World.Regions);//.Save(GetName(MappingFolder));

            //Map = new Map();
            //Map.GetWorldMap(WorldPath, new RegionMapping.Range(-16, -16, 15, 15)).Save(GetName(MappingFolder));

            timeSpan = DateTime.Now - time;
            Debug.WriteLine(timeSpan.TotalSeconds + " seconds");


            //TimeSpan timeSpan = new TimeSpan();
            //DateTime time = DateTime.Now;

            //World = WorldGenerator.Generate(new GenerationProfile(512, 512, 0));

            //timeSpan = DateTime.Now - time;
            //Debug.WriteLine(timeSpan.TotalSeconds + " seconds");

            //World.Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\test_stone";
            //World.SaveRegion(0, 0);
        }
        private void Form1_Shown(object sender, EventArgs e) {
            Hide();

            //NBTViewer nbtViewer = new NBTViewer(CompoundTag);
            //nbtViewer.ShowDialog();

            //BlockInfoViewer blockInfoViewer = new BlockInfoViewer(World);
            //blockInfoViewer.ShowDialog();

            //PerlinNoise2D perlinNoiseForm = new PerlinNoise2D(PerlinNoise);
            //perlinNoiseForm.Show();

            //RegionMap regionMap = new RegionMap(World.Regions);
            //regionMap.ShowDialog();

            RegionMap regionMap = new RegionMap(Map);
            regionMap.ShowDialog();

            //BiomeViewer biomeViewer = new BiomeViewer(WorldGenerator.GetBiomeMap1(new GenerationProfile(512, 512, 0)));
            //biomeViewer.ShowDialog();

            //PropertyViewer propertyViewer = new PropertyViewer(World);
            //propertyViewer.ShowDialog();

            Close();
        }

        public void LoadWorld() {
            World = new World(WorldPath);

            string[] files = Directory.GetFiles(WorldPath + @"\region");
            int[,] regionIndexes = new int[files.Length, 2];

            for (int i = 0; i < regionIndexes.GetLength(0); i++) {
                string file = Path.GetFileNameWithoutExtension(files[i]);
                string fileName = file.Substring(2, file.Length - 2);

                string[] indexes = fileName.Split('.');

                int xIndex = Int32.Parse(indexes[0]);
                int zIndex = Int32.Parse(indexes[1]);

                regionIndexes[i, 0] = xIndex;
                regionIndexes[i, 1] = zIndex;
            }

            for (int i = 0; i < regionIndexes.GetLength(0); i++) {
                World.LoadRegion(regionIndexes[i, 0], regionIndexes[i, 1]);

                Debug.WriteLine((i + 1) + " / " + regionIndexes.GetLength(0));
            }
        }
        public void LoadWorld(RegionMapping.Range range) {
            List<int[]> regionIndexList = new List<int[]>();
            int[,] regionIndexArray = GetRegionIndexes();

            for (int i = 0; i < regionIndexArray.GetLength(0); i++) {
                if (range.WithinRange(regionIndexArray[i, 0], regionIndexArray[i, 1])) {
                    regionIndexList.Add(new int[] { regionIndexArray[i, 0], regionIndexArray[i, 1] });
                }
            }

            int[,] regionIndexes = new int[regionIndexList.Count, 2];
            for (int i = 0; i < regionIndexList.Count; i++) {
                regionIndexes[i, 0] = regionIndexList[i][0];
                regionIndexes[i, 1] = regionIndexList[i][1];
            }

            LoadRegions(regionIndexes);
        }
        public int[,] GetRegionIndexes() {
            string[] files = Directory.GetFiles(WorldPath + @"\region");
            int[,] regionIndexes = new int[files.Length, 2];

            for (int i = 0; i < regionIndexes.GetLength(0); i++) {
                string file = Path.GetFileNameWithoutExtension(files[i]);
                string fileName = file.Substring(2, file.Length - 2);

                string[] indexes = fileName.Split('.');

                int xIndex = Int32.Parse(indexes[0]);
                int zIndex = Int32.Parse(indexes[1]);

                regionIndexes[i, 0] = xIndex;
                regionIndexes[i, 1] = zIndex;
            }

            return regionIndexes;
        }
        public void LoadRegions(int[,] regionIndexes) {
            for (int i = 0; i < regionIndexes.GetLength(0); i++) {
                World.LoadRegion(regionIndexes[i, 0], regionIndexes[i, 1]);

                Debug.WriteLine((i + 1) + " / " + regionIndexes.GetLength(0));
            }
        }

        public void TestConcept()
        {
            byte[,,,] blockBytes = new byte[1024, 5, 4096, 3];

            Random random = new Random();
            for (int i = 0; i < blockBytes.GetLength(0); i++) {
                for (int b = 0; b < blockBytes.GetLength(1); b++) {
                    for (int c = 0; c < blockBytes.GetLength(2); c++) {
                        for (int d = 0; d < blockBytes.GetLength(3); d++) {
                            blockBytes[i, b, c, d] = (byte)random.Next(byte.MaxValue);
                        }
                    }
                }
            }
        }

        public string GetName(string folderPath) {
            string[] fileNames = Directory.GetFiles(folderPath);

            string name = "Map";

            bool nameExists = true;

            int loop = 0;
            while (nameExists) {
                string addOn = " (" + loop + ")";

                nameExists = fileNames.Contains(folderPath + @"\" + name + addOn + ".png");
                if (!nameExists) {
                    name += addOn;
                }

                loop++;  
            }

            return folderPath + @"\" + name + ".png";
        }

        private Schematic GetImageSchematic(string url) {
            int width = 512;

            System.Drawing.Image image = System.Drawing.Image.FromFile(url);
            int height = (int)((image.Height / (double)image.Width) * width);
            
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(image, width, height);

            Schematic schematic = new Schematic(height, 22, width);

            string[] namespaces = { "quartz_block", "diorite", "andesite", "cobblestone", "gray_wool", "gray_terracotta", "coal_block" };
            short[] ids = new short[namespaces.Length];
            for (int i = 0; i < namespaces.Length; i++) {
                ids[i] = ID.BlockIDReverse["minecraft:" + namespaces[i]];
            }

            for (int z = 0; z < width; z++) {
                for (int x = 0; x < height; x++) {
                    System.Drawing.Color color = bitmap.GetPixel(z, x);
                    float brightness = color.GetBrightness();

                    int index = (int)(brightness * ids.Length);
                    index = index == ids.Length ? ids.Length - 1 : index;
                    index = ids.Length - 1 - index;

                    short id = ids[index];

                    int xp = height - x - 1;
                    schematic.SetBlock(xp, 0, z, id, new List<Property>());
                }
            }

            return schematic;
        }

        private void TestSpeed(int repeat) {
            double seconds = 0;

            int[,] regionIndexes = GetRegionIndexes();
            for (int i = 0; i < repeat; i++) {
                DateTime dateTime = DateTime.Now;
                LoadRegions(regionIndexes);
                seconds += (DateTime.Now - dateTime).TotalSeconds;

                Debug.WriteLine((DateTime.Now - dateTime).TotalSeconds + " | " + (i + 1) + " / " + repeat);

                World.Regions = new List<WorldEditor.Region>();
            }

            Debug.WriteLine("Average: " + String.Format("{0:0.#######}", seconds / repeat) + " seconds | " + repeat + " iterations | " + DateTime.Now);
        }

        private void RenderLayers(int max, int min, string path) {
            Map map = new Map();

            for (int i = max; i > min; i--){
                Scripts.MaxHeight = i;

                map.GetRegionMap(World.Regions).Save(GetName(path));

                Debug.WriteLine(i + " / " + min);
            }
        }
    }
}

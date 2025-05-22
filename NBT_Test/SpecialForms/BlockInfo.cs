using System;
using System.Windows.Forms;
using WorldEditor;

namespace NBT_Test
{
    public partial class BlockInfoViewer : Form
    {
        private World World { get; set; }

        public BlockInfoViewer(World world)
        {
            InitializeComponent();

            this.World = world;
        }

        private void Button1_Click(object sender, EventArgs e){
            string[] coordinates = textBox1.Text.Trim().Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);

            namespaceLabel.Text = "Block namespace: ";
            biomeLabel.Text = "Block biome: ";
            coordinatesLabel.Text = "Block coordinates: ";
            sectionLabel.Text = "Block chunk section: ";
            chunkLabel.Text = "Block chunk: ";
            regionLabel.Text = "Block region: ";
            numberOfPropertiesLabel.Text = "Number of properties: ";

            if (coordinates.Length >= 3) {
                int x = 0;
                Int32.TryParse(coordinates[0], out x);

                int y = 0; Int32.Parse(coordinates[1]);
                Int32.TryParse(coordinates[1], out y);

                int z = 0; Int32.Parse(coordinates[2]);
                Int32.TryParse(coordinates[2], out z);

                BlockInfo blockInfo = World.GetBlockInfo(x, y, z);

                if (blockInfo != null) {
                    Display(blockInfo);
                }
            }
        }

        private void Display(BlockInfo blockInfo) {
            namespaceLabel.Text = "Block namespace: " + ID.BlockID[blockInfo.Block.ID];
            biomeLabel.Text = "Block biome: " + ID.BiomeID[(short)blockInfo.BiomeID];
            coordinatesLabel.Text = "Block coordinates: " + blockInfo.X + "; " + blockInfo.Y + "; " + blockInfo.Z;
            sectionLabel.Text = "Block chunk section: " + blockInfo.SectionY;
            chunkLabel.Text = "Block chunk: " + blockInfo.ChunkX + "; " + blockInfo.ChunkZ;
            regionLabel.Text = "Block region: " + "r." + blockInfo.RegionX + "." + blockInfo.RegionZ + ".mca";
            numberOfPropertiesLabel.Text = "Number of properties: " + blockInfo.Block.Properties.Count;
        }
    }
}

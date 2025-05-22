using System;
using System.Windows.Forms;
using WorldEditor;

namespace NBT_Test {
    public partial class PropertyViewer : Form {
        private World World { get; set; }

        public PropertyViewer(World world)
        {
            InitializeComponent();
            this.World = world;
        }

        private void Display(BlockInfo blockInfo) {
            dataGridView1.Rows.Clear();

            for (int i = 0; i < blockInfo.Block.Properties.Count; i++) {
                dataGridView1.Rows.Add(
                    blockInfo.Block.Properties[i].Name,
                    blockInfo.Block.Properties[i].Value
                );
            }

            dataGridView1.ClearSelection();

            blockNamespaceLabel.Text = "Block namespace: " + ID.BlockID[blockInfo.Block.ID];
        }

        private void GetPropertiesButton_Click(object sender, EventArgs e)
        {
            string[] coordinates = textBox1.Text.Trim().Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

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
    }
}

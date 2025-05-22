using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NBT_Test
{
    public partial class BiomeViewer : Form
    {
        private int[,] BiomeMap { get; set; }
        private static Dictionary<int, Color> Dictionary = new Dictionary<int, Color>() {
            {0,  ColorTranslator.FromHtml("#A4BF65")}, //Plains
            {1,  ColorTranslator.FromHtml("#48AF50")}, //Forest
            {2,  ColorTranslator.FromHtml("#C0C0C0")}, //Mountains
            {3,  ColorTranslator.FromHtml("#FFF0B2")}, //Desert
            {4,  ColorTranslator.FromHtml("#A4BFCD")}, //Snowy Tundra
            {5,  ColorTranslator.FromHtml("#4AA593")}, //Snowy Forest
            {6,  ColorTranslator.FromHtml("#216EA5")}, //Ocean
        };

        private Bitmap Bitmap;

        public BiomeViewer(int[,] biomeMap)
        {
            InitializeComponent();

            this.BiomeMap = biomeMap;
        }

        private void BiomeViewer_Load(object sender, EventArgs e){
            Bitmap = GetImage();

            Image image = ResizeBitmap(Bitmap);

            pictureBox1.Image = image;
        }

        private Bitmap GetImage(){
            Bitmap bitmap = new Bitmap(BiomeMap.GetLength(0), BiomeMap.GetLength(1));

            for (int z = 0; z < BiomeMap.GetLength(0); z++) {
                for (int x = 0; x < BiomeMap.GetLength(1); x++) {
                    Color color = Dictionary[BiomeMap[x, z]];

                    bitmap.SetPixel(x, z, color);
                }
            }

            return bitmap;
        }

        private Bitmap ResizeBitmap(Bitmap sourceBMP){
            int width = (int)((sourceBMP.Width / (double)sourceBMP.Height) * pictureBox1.Height);
            int height = pictureBox1.Height;

            if (width > pictureBox1.Width){
                width = pictureBox1.Width;
                height = (int)((sourceBMP.Height / (double)sourceBMP.Width) * width);
            }

            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(sourceBMP, 0, 0, width, height);
            }
            return result;
        }

        private void PictureBox1_SizeChanged(object sender, EventArgs e)
        {
            if (Bitmap != null && pictureBox1.Width > 0 && pictureBox1.Height > 0){
                pictureBox1.Image = ResizeBitmap(Bitmap);
            }
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace NBT_Test
{
    public partial class PerlinNoise2D : Form
    {
        private byte[,] Grid { get; set; }
        private Bitmap Bitmap;

        public PerlinNoise2D(byte[,] grid)
        {
            InitializeComponent();

            this.Grid = grid;
        }

        private void PerlinNoise2D_Load(object sender, EventArgs e)
        {
            Bitmap = GetImage(Grid);

            Image image = ResizeBitmap(Bitmap);

            pictureBox1.Image = image;
        }

        private Bitmap GetImage(byte[,] bytes) {
            Bitmap bitmap = new Bitmap(bytes.GetLength(0), bytes.GetLength(1));

            for (int y = 0; y < bytes.GetLength(1); y++) {
                for (int x = 0; x < bytes.GetLength(0); x++) {
                    bitmap.SetPixel(x, y, Color.FromArgb(bytes[x, y], bytes[x, y], bytes[x, y]));
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

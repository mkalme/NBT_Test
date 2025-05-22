using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using RegionMapping;

namespace NBT_Test
{
    public partial class RegionMap : Form
    {
        private List<WorldEditor.Region> Regions { get; set; }
        private Map Map { get; set; }

        private Bitmap Bitmap;

        private double Zoom = 1;
        private int XOffset = 0;
        private int YOffset = 0;

        public RegionMap(WorldEditor.Region region){
            InitializeComponent();

            this.Regions = new List<WorldEditor.Region>() {region };
            this.Map = new Map();
            this.Bitmap = Map.GetRegionMap(Regions);
        }
        public RegionMap(List<WorldEditor.Region> regions) {
            InitializeComponent();

            this.Regions = regions;
            this.Map = new Map();
            this.Bitmap = Map.GetRegionMap(Regions);
        }
        public RegionMap(Map map) {
            InitializeComponent();

            this.Regions = new List<WorldEditor.Region>();
            this.Map = map;

            if (map != null) {
                if (map.Image != null) {
                    this.Bitmap = map.Image;
                } else {
                    this.Bitmap = new Bitmap(1, 1);
                }
            } else {
                this.Bitmap = new Bitmap(1, 1);
            }
        }

        private void RegionMap_Load(object sender, EventArgs e){
            DisplayImage();
        }

        //Form
        private Bitmap ResizeBitmap(Bitmap sourceBMP){
            int pictureWidth = (int)(panel1.Width * Zoom);
            int pictureHeight = (int)(panel1.Height * Zoom);

            int width = (int)((sourceBMP.Width / (double)sourceBMP.Height) * pictureHeight);
            int height = pictureHeight;

            if (width > pictureWidth){
                width = pictureWidth;
                height = (int)((sourceBMP.Height / (double)sourceBMP.Width) * width);
            }

            if (width == 0 || height == 0) return sourceBMP; 

            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result)){
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(sourceBMP, 0, 0, width, height);
            }
            return result;
        }
        private void RegionMap_SizeChanged(object sender, EventArgs e)
        {
            if (Bitmap != null && panel1.Width > 0 && panel1.Height > 0) {
                DisplayImage();
            }
        }

        private void CopyToClipBoardLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Bitmap != null) {
                Clipboard.SetImage(Bitmap);
            }
        }

        private void DisplayImage() {
            Image image = ResizeBitmap(Bitmap);

            pictureBox1.Size = image.Size;
            MoveImage();

            pictureBox1.Image = image;
        }
        private void MoveImage() {
            pictureBox1.Location = GetLocation();
        }

        private Point GetLocation() {
            return new Point((panel1.Width - pictureBox1.Size.Width) / 2 + XOffset, (panel1.Height - pictureBox1.Size.Height) / 2 + YOffset);
        }

        private void ZoomTrackBar_Scroll(object sender, EventArgs e)
        {
            Zoom = 1 + 0.2 * zoomTrackBar.Value;
            DisplayImage();
        }

        private Point MouseCords = new Point();
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            MouseCords = new Point(e.X, e.Y);
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) {
                XOffset += e.X - MouseCords.X;
                YOffset += e.Y - MouseCords.Y;

                MoveImage();
            }
        }
    }
}
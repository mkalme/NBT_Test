namespace NBT_Test
{
    partial class RegionMap
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.copyToClipBoardLink = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.zoomTrackBar = new System.Windows.Forms.TrackBar();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTrackBar)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(918, 526);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseMove);
            // 
            // copyToClipBoardLink
            // 
            this.copyToClipBoardLink.ActiveLinkColor = System.Drawing.Color.DimGray;
            this.copyToClipBoardLink.AutoSize = true;
            this.copyToClipBoardLink.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.copyToClipBoardLink.LinkColor = System.Drawing.Color.LightGray;
            this.copyToClipBoardLink.Location = new System.Drawing.Point(7, 8);
            this.copyToClipBoardLink.Name = "copyToClipBoardLink";
            this.copyToClipBoardLink.Size = new System.Drawing.Size(89, 13);
            this.copyToClipBoardLink.TabIndex = 1;
            this.copyToClipBoardLink.TabStop = true;
            this.copyToClipBoardLink.Text = "Save to memoru";
            this.copyToClipBoardLink.VisitedLinkColor = System.Drawing.Color.Black;
            this.copyToClipBoardLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CopyToClipBoardLink_LinkClicked);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.copyToClipBoardLink);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(918, 526);
            this.panel1.TabIndex = 2;
            // 
            // zoomTrackBar
            // 
            this.zoomTrackBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.zoomTrackBar.Location = new System.Drawing.Point(3, 1);
            this.zoomTrackBar.Maximum = 20;
            this.zoomTrackBar.Name = "zoomTrackBar";
            this.zoomTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.zoomTrackBar.Size = new System.Drawing.Size(45, 196);
            this.zoomTrackBar.TabIndex = 0;
            this.zoomTrackBar.Scroll += new System.EventHandler(this.ZoomTrackBar_Scroll);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.zoomTrackBar);
            this.panel2.Location = new System.Drawing.Point(7, 23);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(54, 204);
            this.panel2.TabIndex = 2;
            // 
            // RegionMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(918, 526);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(200, 100);
            this.Name = "RegionMap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RegionMap";
            this.Load += new System.EventHandler(this.RegionMap_Load);
            this.SizeChanged += new System.EventHandler(this.RegionMap_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zoomTrackBar)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel copyToClipBoardLink;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TrackBar zoomTrackBar;
        private System.Windows.Forms.Panel panel2;
    }
}
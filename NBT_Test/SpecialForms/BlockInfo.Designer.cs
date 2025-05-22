namespace NBT_Test
{
    partial class BlockInfoViewer
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
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.namespaceLabel = new System.Windows.Forms.Label();
            this.biomeLabel = new System.Windows.Forms.Label();
            this.coordinatesLabel = new System.Windows.Forms.Label();
            this.regionLabel = new System.Windows.Forms.Label();
            this.chunkLabel = new System.Windows.Forms.Label();
            this.sectionLabel = new System.Windows.Forms.Label();
            this.numberOfPropertiesLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 22);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(95, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Get block info";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(113, 24);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(124, 20);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.label1.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.label1.Location = new System.Drawing.Point(243, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "e.g. 17; 87; -10";
            // 
            // namespaceLabel
            // 
            this.namespaceLabel.AutoSize = true;
            this.namespaceLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.namespaceLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.namespaceLabel.Location = new System.Drawing.Point(14, 65);
            this.namespaceLabel.Name = "namespaceLabel";
            this.namespaceLabel.Size = new System.Drawing.Size(112, 15);
            this.namespaceLabel.TabIndex = 3;
            this.namespaceLabel.Text = "Block namespace: ";
            // 
            // biomeLabel
            // 
            this.biomeLabel.AutoSize = true;
            this.biomeLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.biomeLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.biomeLabel.Location = new System.Drawing.Point(14, 86);
            this.biomeLabel.Name = "biomeLabel";
            this.biomeLabel.Size = new System.Drawing.Size(81, 15);
            this.biomeLabel.TabIndex = 4;
            this.biomeLabel.Text = "Block biome: ";
            // 
            // coordinatesLabel
            // 
            this.coordinatesLabel.AutoSize = true;
            this.coordinatesLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.coordinatesLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.coordinatesLabel.Location = new System.Drawing.Point(14, 106);
            this.coordinatesLabel.Name = "coordinatesLabel";
            this.coordinatesLabel.Size = new System.Drawing.Size(108, 15);
            this.coordinatesLabel.TabIndex = 5;
            this.coordinatesLabel.Text = "Block coordinates:";
            // 
            // regionLabel
            // 
            this.regionLabel.AutoSize = true;
            this.regionLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.regionLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.regionLabel.Location = new System.Drawing.Point(14, 167);
            this.regionLabel.Name = "regionLabel";
            this.regionLabel.Size = new System.Drawing.Size(78, 15);
            this.regionLabel.TabIndex = 8;
            this.regionLabel.Text = "Block region:";
            // 
            // chunkLabel
            // 
            this.chunkLabel.AutoSize = true;
            this.chunkLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.chunkLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.chunkLabel.Location = new System.Drawing.Point(14, 147);
            this.chunkLabel.Name = "chunkLabel";
            this.chunkLabel.Size = new System.Drawing.Size(79, 15);
            this.chunkLabel.TabIndex = 7;
            this.chunkLabel.Text = "Block chunk: ";
            // 
            // sectionLabel
            // 
            this.sectionLabel.AutoSize = true;
            this.sectionLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.sectionLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.sectionLabel.Location = new System.Drawing.Point(14, 126);
            this.sectionLabel.Name = "sectionLabel";
            this.sectionLabel.Size = new System.Drawing.Size(122, 15);
            this.sectionLabel.TabIndex = 6;
            this.sectionLabel.Text = "Block chunk section: ";
            // 
            // numberOfPropertiesLabel
            // 
            this.numberOfPropertiesLabel.AutoSize = true;
            this.numberOfPropertiesLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.numberOfPropertiesLabel.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.numberOfPropertiesLabel.Location = new System.Drawing.Point(14, 186);
            this.numberOfPropertiesLabel.Name = "numberOfPropertiesLabel";
            this.numberOfPropertiesLabel.Size = new System.Drawing.Size(127, 15);
            this.numberOfPropertiesLabel.TabIndex = 9;
            this.numberOfPropertiesLabel.Text = "Number of properties:";
            // 
            // BlockInfoViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(404, 219);
            this.Controls.Add(this.numberOfPropertiesLabel);
            this.Controls.Add(this.regionLabel);
            this.Controls.Add(this.chunkLabel);
            this.Controls.Add(this.sectionLabel);
            this.Controls.Add(this.coordinatesLabel);
            this.Controls.Add(this.biomeLabel);
            this.Controls.Add(this.namespaceLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "BlockInfoViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TestForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label namespaceLabel;
        private System.Windows.Forms.Label biomeLabel;
        private System.Windows.Forms.Label coordinatesLabel;
        private System.Windows.Forms.Label regionLabel;
        private System.Windows.Forms.Label chunkLabel;
        private System.Windows.Forms.Label sectionLabel;
        private System.Windows.Forms.Label numberOfPropertiesLabel;
    }
}
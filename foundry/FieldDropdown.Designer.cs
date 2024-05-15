namespace Foundry
{
    partial class FieldDropdown
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            statusImage = new PictureBox();
            splitContainer1 = new SplitContainer();
            comboBox1 = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)statusImage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Top;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(51, 20);
            label1.TabIndex = 1;
            label1.Text = "NAME";
            // 
            // statusImage
            // 
            statusImage.BackgroundImageLayout = ImageLayout.None;
            statusImage.Dock = DockStyle.Fill;
            statusImage.Image = Properties.Resources.bullet_red;
            statusImage.Location = new Point(0, 0);
            statusImage.Name = "statusImage";
            statusImage.Size = new Size(26, 29);
            statusImage.SizeMode = PictureBoxSizeMode.StretchImage;
            statusImage.TabIndex = 2;
            statusImage.TabStop = false;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Bottom;
            splitContainer1.FixedPanel = FixedPanel.Panel2;
            splitContainer1.Location = new Point(0, 20);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(comboBox1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(statusImage);
            splitContainer1.Size = new Size(484, 29);
            splitContainer1.SplitterDistance = 457;
            splitContainer1.SplitterWidth = 1;
            splitContainer1.TabIndex = 3;
            // 
            // comboBox1
            // 
            comboBox1.Dock = DockStyle.Fill;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(0, 0);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(457, 28);
            comboBox1.TabIndex = 0;
            // 
            // FieldDropdown
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Controls.Add(label1);
            Name = "FieldDropdown";
            Size = new Size(484, 49);
            ((System.ComponentModel.ISupportInitialize)statusImage).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private PictureBox statusImage;
        private SplitContainer splitContainer1;
        private ComboBox comboBox1;
    }
}

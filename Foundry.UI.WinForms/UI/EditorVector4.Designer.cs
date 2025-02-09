namespace Chef.Win.UI
{
    partial class EditorVector4
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
            xval = new NumericUpDown();
            yval = new NumericUpDown();
            zval = new NumericUpDown();
            xlabel = new Label();
            ylabel = new Label();
            zlabel = new Label();
            wval = new NumericUpDown();
            wlabel = new Label();
            ((System.ComponentModel.ISupportInitialize)xval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)yval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)zval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)wval).BeginInit();
            SuspendLayout();
            // 
            // xval
            // 
            xval.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            xval.DecimalPlaces = 3;
            xval.Location = new Point(94, 3);
            xval.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            xval.Name = "xval";
            xval.Size = new Size(103, 27);
            xval.TabIndex = 1;
            // 
            // yval
            // 
            yval.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            yval.DecimalPlaces = 3;
            yval.Location = new Point(94, 36);
            yval.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            yval.Name = "yval";
            yval.Size = new Size(103, 27);
            yval.TabIndex = 2;
            // 
            // zval
            // 
            zval.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            zval.DecimalPlaces = 3;
            zval.Location = new Point(94, 69);
            zval.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            zval.Name = "zval";
            zval.Size = new Size(103, 27);
            zval.TabIndex = 3;
            // 
            // xlabel
            // 
            xlabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            xlabel.AutoSize = true;
            xlabel.Location = new Point(72, 5);
            xlabel.Name = "xlabel";
            xlabel.Size = new Size(16, 20);
            xlabel.TabIndex = 4;
            xlabel.Text = "x";
            xlabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ylabel
            // 
            ylabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ylabel.AutoSize = true;
            ylabel.Location = new Point(72, 38);
            ylabel.Name = "ylabel";
            ylabel.Size = new Size(16, 20);
            ylabel.TabIndex = 5;
            ylabel.Text = "y";
            ylabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // zlabel
            // 
            zlabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            zlabel.AutoSize = true;
            zlabel.Location = new Point(72, 71);
            zlabel.Name = "zlabel";
            zlabel.Size = new Size(16, 20);
            zlabel.TabIndex = 6;
            zlabel.Text = "z";
            zlabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // wval
            // 
            wval.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            wval.DecimalPlaces = 3;
            wval.Location = new Point(94, 102);
            wval.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            wval.Name = "wval";
            wval.Size = new Size(103, 27);
            wval.TabIndex = 7;
            // 
            // wlabel
            // 
            wlabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            wlabel.AutoSize = true;
            wlabel.Location = new Point(72, 104);
            wlabel.Name = "wlabel";
            wlabel.Size = new Size(20, 20);
            wlabel.TabIndex = 8;
            wlabel.Text = "w";
            wlabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // EditorVector4
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(wlabel);
            Controls.Add(wval);
            Controls.Add(zlabel);
            Controls.Add(ylabel);
            Controls.Add(xlabel);
            Controls.Add(zval);
            Controls.Add(yval);
            Controls.Add(xval);
            Name = "EditorVector4";
            Size = new Size(200, 132);
            ((System.ComponentModel.ISupportInitialize)xval).EndInit();
            ((System.ComponentModel.ISupportInitialize)yval).EndInit();
            ((System.ComponentModel.ISupportInitialize)zval).EndInit();
            ((System.ComponentModel.ISupportInitialize)wval).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown xval;
        private NumericUpDown yval;
        private NumericUpDown zval;
        private Label xlabel;
        private Label ylabel;
        private Label zlabel;
        private NumericUpDown wval;
        private Label wlabel;
    }
}

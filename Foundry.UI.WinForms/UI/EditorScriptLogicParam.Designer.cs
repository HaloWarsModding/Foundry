namespace Chef.Win.UI
{
    partial class EditorScriptLogicParam
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
            varlabel = new Label();
            vallabel = new Label();
            varname = new TextBox();
            SuspendLayout();
            // 
            // varlabel
            // 
            varlabel.AutoSize = true;
            varlabel.Location = new Point(3, 3);
            varlabel.Margin = new Padding(3, 3, 3, 0);
            varlabel.Name = "varlabel";
            varlabel.Size = new Size(66, 20);
            varlabel.TabIndex = 0;
            varlabel.Text = "Variable:";
            // 
            // vallabel
            // 
            vallabel.AutoSize = true;
            vallabel.Location = new Point(3, 60);
            vallabel.Margin = new Padding(3, 3, 3, 0);
            vallabel.Name = "vallabel";
            vallabel.Size = new Size(48, 20);
            vallabel.TabIndex = 1;
            vallabel.Text = "Value:";
            // 
            // varname
            // 
            varname.Location = new Point(3, 26);
            varname.Name = "varname";
            varname.Size = new Size(200, 27);
            varname.TabIndex = 2;
            // 
            // EditorScriptLogicParam
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(varname);
            Controls.Add(vallabel);
            Controls.Add(varlabel);
            Margin = new Padding(6);
            Name = "EditorScriptLogicParam";
            Size = new Size(206, 80);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label varlabel;
        private Label vallabel;
        private TextBox varname;
    }
}

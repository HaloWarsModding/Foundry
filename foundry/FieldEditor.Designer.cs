namespace Foundry
{
    partial class FieldEditor
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
            splitContainer3 = new SplitContainer();
            buttonCancel = new Button();
            buttonApply = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer3
            // 
            splitContainer3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            splitContainer3.IsSplitterFixed = true;
            splitContainer3.Location = new Point(214, 469);
            splitContainer3.Margin = new Padding(0);
            splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(buttonCancel);
            splitContainer3.Panel1.RightToLeft = RightToLeft.No;
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(buttonApply);
            splitContainer3.Panel2.RightToLeft = RightToLeft.No;
            splitContainer3.Size = new Size(286, 31);
            splitContainer3.SplitterDistance = 139;
            splitContainer3.SplitterWidth = 5;
            splitContainer3.TabIndex = 2;
            // 
            // buttonCancel
            // 
            buttonCancel.Dock = DockStyle.Fill;
            buttonCancel.ImeMode = ImeMode.NoControl;
            buttonCancel.Location = new Point(0, 0);
            buttonCancel.Margin = new Padding(3, 4, 3, 4);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(139, 31);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonApply
            // 
            buttonApply.Dock = DockStyle.Fill;
            buttonApply.ImeMode = ImeMode.NoControl;
            buttonApply.Location = new Point(0, 0);
            buttonApply.Margin = new Padding(3, 4, 3, 4);
            buttonApply.Name = "buttonApply";
            buttonApply.Size = new Size(142, 31);
            buttonApply.TabIndex = 3;
            buttonApply.Text = "Apply";
            buttonApply.UseVisualStyleBackColor = true;
            // 
            // FieldEditorControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer3);
            MinimumSize = new Size(500, 500);
            Name = "FieldEditorControl";
            Size = new Size(500, 500);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer3;
        private Button buttonCancel;
        private Button buttonApply;
    }
}

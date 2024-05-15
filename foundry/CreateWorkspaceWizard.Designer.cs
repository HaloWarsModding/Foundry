namespace Foundry
{
    partial class CreateWorkspaceWizard
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
            buttonCancel = new Button();
            buttonCreate = new Button();
            textboxName = new TextBox();
            buttonOpenFileBrowser = new Button();
            textboxLocation = new TextBox();
            optionUnpackBaseEras = new CheckBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            checkBox1 = new CheckBox();
            splitContainer1 = new SplitContainer();
            splitContainer3 = new SplitContainer();
            groupBox2 = new GroupBox();
            groupBox3 = new GroupBox();
            groupBox4 = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Dock = DockStyle.Fill;
            buttonCancel.Location = new Point(0, 0);
            buttonCancel.Margin = new Padding(3, 4, 3, 4);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(139, 31);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonCreate
            // 
            buttonCreate.DialogResult = DialogResult.OK;
            buttonCreate.Dock = DockStyle.Fill;
            buttonCreate.Location = new Point(0, 0);
            buttonCreate.Margin = new Padding(3, 4, 3, 4);
            buttonCreate.Name = "buttonCreate";
            buttonCreate.Size = new Size(142, 31);
            buttonCreate.TabIndex = 3;
            buttonCreate.Text = "Create";
            buttonCreate.UseVisualStyleBackColor = true;
            // 
            // textboxName
            // 
            textboxName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textboxName.Location = new Point(7, 25);
            textboxName.Margin = new Padding(7, 0, 7, 0);
            textboxName.Name = "textboxName";
            textboxName.Size = new Size(564, 27);
            textboxName.TabIndex = 4;
            textboxName.Text = "NewWorkspace1";
            // 
            // buttonOpenFileBrowser
            // 
            buttonOpenFileBrowser.Dock = DockStyle.Fill;
            buttonOpenFileBrowser.Location = new Point(0, 0);
            buttonOpenFileBrowser.Margin = new Padding(3, 4, 3, 4);
            buttonOpenFileBrowser.Name = "buttonOpenFileBrowser";
            buttonOpenFileBrowser.Size = new Size(57, 31);
            buttonOpenFileBrowser.TabIndex = 1;
            buttonOpenFileBrowser.Text = "...";
            buttonOpenFileBrowser.UseVisualStyleBackColor = true;
            buttonOpenFileBrowser.Click += buttonOpenFileBrowser_Click;
            // 
            // textboxLocation
            // 
            textboxLocation.Dock = DockStyle.Fill;
            textboxLocation.Location = new Point(0, 0);
            textboxLocation.Margin = new Padding(7, 4, 3, 4);
            textboxLocation.Name = "textboxLocation";
            textboxLocation.Size = new Size(503, 27);
            textboxLocation.TabIndex = 0;
            // 
            // optionUnpackBaseEras
            // 
            optionUnpackBaseEras.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            optionUnpackBaseEras.Checked = true;
            optionUnpackBaseEras.CheckState = CheckState.Checked;
            optionUnpackBaseEras.Location = new Point(11, 25);
            optionUnpackBaseEras.Margin = new Padding(7, 4, 3, 4);
            optionUnpackBaseEras.Name = "optionUnpackBaseEras";
            optionUnpackBaseEras.Size = new Size(530, 31);
            optionUnpackBaseEras.TabIndex = 8;
            optionUnpackBaseEras.Text = "Unpack default archives";
            optionUnpackBaseEras.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Location = new Point(0, 0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.Size = new Size(200, 100);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(3, 3);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(152, 19);
            checkBox1.TabIndex = 8;
            checkBox1.Text = "Unpack default archives";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.FixedPanel = FixedPanel.Panel2;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(7, 25);
            splitContainer1.Margin = new Padding(3, 0, 3, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(textboxLocation);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(buttonOpenFileBrowser);
            splitContainer1.Size = new Size(565, 31);
            splitContainer1.SplitterDistance = 503;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 0;
            // 
            // splitContainer3
            // 
            splitContainer3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            splitContainer3.IsSplitterFixed = true;
            splitContainer3.Location = new Point(310, 233);
            splitContainer3.Margin = new Padding(0);
            splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(buttonCancel);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(buttonCreate);
            splitContainer3.Size = new Size(286, 31);
            splitContainer3.SplitterDistance = 139;
            splitContainer3.SplitterWidth = 5;
            splitContainer3.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(textboxName);
            groupBox2.Location = new Point(14, 16);
            groupBox2.Margin = new Padding(3, 4, 3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(3, 4, 3, 4);
            groupBox2.Size = new Size(578, 64);
            groupBox2.TabIndex = 15;
            groupBox2.TabStop = false;
            groupBox2.Text = "Workspace name";
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(splitContainer1);
            groupBox3.Location = new Point(14, 88);
            groupBox3.Margin = new Padding(3, 4, 3, 4);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(3, 4, 3, 4);
            groupBox3.Size = new Size(578, 64);
            groupBox3.TabIndex = 16;
            groupBox3.TabStop = false;
            groupBox3.Text = "Location";
            // 
            // groupBox4
            // 
            groupBox4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox4.Controls.Add(optionUnpackBaseEras);
            groupBox4.Location = new Point(14, 160);
            groupBox4.Margin = new Padding(3, 4, 3, 4);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(3, 4, 3, 4);
            groupBox4.Size = new Size(578, 64);
            groupBox4.TabIndex = 17;
            groupBox4.TabStop = false;
            groupBox4.Text = "Options";
            // 
            // CreateWorkspaceWizard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(606, 276);
            Controls.Add(splitContainer3);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreateWorkspaceWizard";
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "Create new workspace...";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button buttonCancel;
        private Button buttonCreate;
        private TextBox textboxName;
        private Button buttonOpenFileBrowser;
        private TextBox textboxLocation;
        private CheckBox optionUnpackBaseEras;
        private TableLayoutPanel tableLayoutPanel3;
        private CheckBox checkBox1;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer3;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
    }
}
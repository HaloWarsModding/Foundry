using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Foundry
{
    partial class FoundryInstance
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FoundryInstance));
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.logStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.versionReadout = new System.Windows.Forms.ToolStripStatusLabel();
            this.discordLink = new System.Windows.Forms.ToolStripStatusLabel();
            this.memoryReadout = new System.Windows.Forms.ToolStripStatusLabel();
            this.perfReadout = new System.Windows.Forms.ToolStripStatusLabel();
            this.dockpanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logStatus,
            this.versionReadout,
            this.discordLink,
            this.memoryReadout,
            this.perfReadout});
            this.statusBar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusBar.Location = new System.Drawing.Point(0, 761);
            this.statusBar.Name = "statusBar";
            this.statusBar.Padding = new System.Windows.Forms.Padding(1, 0, 12, 0);
            this.statusBar.Size = new System.Drawing.Size(1475, 25);
            this.statusBar.SizingGrip = false;
            this.statusBar.TabIndex = 1;
            this.statusBar.Text = "statusBar";
            // 
            // logStatus
            // 
            this.logStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.logStatus.Name = "logStatus";
            this.logStatus.Size = new System.Drawing.Size(42, 20);
            this.logStatus.Spring = true;
            this.logStatus.Text = "Ready.";
            // 
            // versionReadout
            // 
            this.versionReadout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.versionReadout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.versionReadout.ForeColor = System.Drawing.Color.Black;
            this.versionReadout.Name = "versionReadout";
            this.versionReadout.Size = new System.Drawing.Size(45, 20);
            this.versionReadout.Text = "version";
            // 
            // discordLink
            // 
            this.discordLink.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.discordLink.AutoToolTip = true;
            this.discordLink.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.discordLink.Image = ((System.Drawing.Image)(resources.GetObject("discordLink.Image")));
            this.discordLink.Name = "discordLink";
            this.discordLink.Size = new System.Drawing.Size(20, 20);
            this.discordLink.ToolTipText = "Join our discord!";
            this.discordLink.Click += new System.EventHandler(this.Footer_DiscordImageClicked);
            // 
            // memoryReadout
            // 
            this.memoryReadout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.memoryReadout.Name = "memoryReadout";
            this.memoryReadout.Size = new System.Drawing.Size(31, 20);
            this.memoryReadout.Text = "0mb";
            // 
            // perfReadout
            // 
            this.perfReadout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.perfReadout.Name = "perfReadout";
            this.perfReadout.Size = new System.Drawing.Size(0, 20);
            // 
            // dockpanel
            // 
            this.dockpanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockpanel.DockBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(242)))));
            this.dockpanel.Location = new System.Drawing.Point(0, 24);
            this.dockpanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dockpanel.Name = "dockpanel";
            this.dockpanel.ShowAutoHideContentOnHover = false;
            this.dockpanel.Size = new System.Drawing.Size(1475, 737);
            this.dockpanel.TabIndex = 3;
            // 
            // menuStrip
            // 
            this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(1475, 24);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "menuStrip1";
            // 
            // FoundryInstance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1475, 786);
            this.Controls.Add(this.dockpanel);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FoundryInstance";
            this.Text = "Foundry";
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel logStatus;
        private System.Windows.Forms.ToolStripStatusLabel versionReadout;
        private DockPanel dockpanel;
        private System.Windows.Forms.MenuStrip menuStrip;
        private ToolStripStatusLabel discordLink;
        private ToolStripStatusLabel memoryReadout;
        public ToolStripStatusLabel perfReadout;
    }
}
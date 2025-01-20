using Chef.HW1;
using Chef.HW1.Map;
using Chef.HW1.Script;
using Chef.HW1.Workspace;
using Chef.Win.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using static WeifenLuo.WinFormsUI.Docking.DockPanelExtender;

namespace Chef.Win.UI
{
    internal class WorkspaceFloatWindow : FloatWindow
    {
        public WorkspaceFloatWindow(DockPanel panel, DockPane pane) : base(panel, pane)
        {
            //FormBorderStyle = FormBorderStyle.Sizable;
            //ShowInTaskbar = true;
            //Icon = Owner.Icon;
            //Owner = null;
            //DoubleClickTitleBarToDock = false;
        }
        public WorkspaceFloatWindow(DockPanel panel, DockPane pane, Rectangle bounds) : base(panel, pane, bounds)
        {
            //FormBorderStyle = FormBorderStyle.Sizable;
            //ShowInTaskbar = true;
            //Icon = Owner.Icon;
            //Owner = null;
            //DoubleClickTitleBarToDock = false;
        }
    }
    internal class WorkspaceFloatWindowFactory : IFloatWindowFactory
    {
        public FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane)
        {
            return new WorkspaceFloatWindow(dockPanel, pane);
        }
        public FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds)
        {
            return new WorkspaceFloatWindow(dockPanel, pane, bounds);
        }
    }

    public class MainWindow : Form
    {
        public AssetCache Assets { get;private set; }
        public GpuCache GpuAssets { get; private set; }
        public DockPanel DockPanel { get; private set; }
        public BrowserWindow Browser { get; private set; }

        public MainWindow() : base()
        {
            Assets = new AssetCache();
            GpuAssets = new GpuCache();

            //window docking
            DockPanel = new DockPanel();
            DockPanel.Dock = DockStyle.Fill;
            DockPanel.Theme = new VS2015LightTheme();
            DockPanel.DefaultFloatWindowSize = new Size(500, 250);
            DockPanel.Theme.Extender.FloatWindowFactory = new WorkspaceFloatWindowFactory();

            //browser
            Browser = new BrowserWindow();
            Browser.Show(DockPanel, DockState.DockLeft);

            //top menu
            MainMenuStrip = new MenuStrip();
            MainMenuStrip.Items.Add(
                new ToolStripMenuItem("File", null,
                [
                    new ToolStripMenuItem("Open Workspace...", null, (s, e) =>
                {
                    using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            AssetDatabase.Index(fbd.SelectedPath, Assets);
                            Browser.Update(Assets, GpuAssets, DockPanel);
                        }
                    }
                }),
                    new ToolStripMenuItem("Close Workspace", null, (s, e) =>
                    {
                        Assets = new AssetCache();
                        GpuAssets = new GpuCache();
                        Browser.Update(Assets, GpuAssets, DockPanel);
                    })
                ]));

            //this
            ClientSize = new Size(942, 493);
            Text = Properties.Resources.Title;
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Controls.Add(DockPanel);
            Controls.Add(MainMenuStrip);
        }
    }
}

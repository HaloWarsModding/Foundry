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
        public WorkspaceFile Root { get; private set; }
        public DockPanel DockPanel { get; private set; }
        public BrowserWindow Browser { get; private set; }

        public MainWindow() : base()
        {
            Assets = new AssetCache();
            GpuAssets = new GpuCache();
            EditorWindows = new Dictionary<WorkspaceFile, DockContent>();

            //window docking
            DockPanel = new DockPanel();
            DockPanel.Dock = DockStyle.Fill;
            DockPanel.Theme = new VS2015LightTheme();
            DockPanel.DefaultFloatWindowSize = new Size(500, 250);
            DockPanel.Theme.Extender.FloatWindowFactory = new WorkspaceFloatWindowFactory();

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
                            Root = new WorkspaceFile(fbd.SelectedPath);
                            Browser.Update(Assets, GpuAssets, DockPanel);
                            //Browser.RootItems.Clear();
                            ////Browser.RootItems.Add(WorkspaceBrowser.ArtItem(Root));
                            ////Browser.RootItems.Add(WorkspaceBrowser.DataItem(Root));
                            ////Browser.RootItems.Add(WorkspaceBrowser.ScenarioItem(Root));
                            //Browser.UpdateView();
                        }
                    }
                }),
                    new ToolStripMenuItem("Close Workspace", null, (s, e) =>
                    {
                        Browser.Update(Assets, GpuAssets, DockPanel);
                        Root = null;
                    })
                ]));

            //this
            ClientSize = new Size(942, 493);
            //Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            Text = Properties.Resources.Title;
            Controls.Add(DockPanel);
            Controls.Add(MainMenuStrip);

            //browser
            Browser = new BrowserWindow();
            Browser.Show(DockPanel, DockState.DockLeft);
            //Browser.BrowserNodeDoubleClicked += (s, e) =>
            {
                //if (e.Item is WorkspaceBrowserPath)
                //{
                //    var path = e.Item as WorkspaceBrowserPath;
                //    var item = path.Item;

                //    if (!EditorWindows.ContainsKey(file.)
                //    {
                //        EditorWindows.Add(file. CreateFileEditor(Workspace, GpuAssets, file.);
                //    }
                //    EditorWindows[file..Show(DockPanel, DockState.Document);
                //    EditorWindows[file..Activate();

                //    EditorWindows[file..FormClosed += (s, e) =>
                //    {
                //        //remove the form when it closes.
                //        EditorWindows.Remove(file.;
                //        GC.Collect(); //force gc because the user probably will expect that [closing page -> less memory used].
                //    };
                //}
            };
#if DEBUG
            //Browser.RootItems.Clear();
            //Browser.RootItems.Add(WorkspaceBrowser.ArtItem(Root));
            //Browser.RootItems.Add(WorkspaceBrowser.DataItem(Root));
            //Browser.RootItems.Add(WorkspaceBrowser.ScenarioItem(Root));
            //Browser.UpdateView();
#endif
        }
        private Dictionary<WorkspaceFile, DockContent> EditorWindows;

        public static DockContent CreateFileEditor(WorkspaceFile root, GpuDatabase gpudb, WorkspaceFile file)
        {
            //switch (file.Extension)
            //{
            //    case ".triggerscript":
            //        var ts = new TriggerscriptWindow();
            //        ts.Text = file.Name;
            //        ts.Name = file.Name;
            //        Triggerscript script = null;
            //        if (script == null) return null;
            //        ts.TriggerscriptRef = new WeakReference<Triggerscript>(script);
            //        return ts;

            //    case ".scn":
            //        var sc = new ScenarioWindow();
            //        sc.Text = file.Name;
            //        sc.Name = file.Name;
            //        using (Stream xtd = File.OpenRead(file.Parent.ChildFiles.Where(f => f.Extension == ".xtd").First().Path))
            //        {
            //            var tvis = TerrainIO.ReadXtd(xtd);
            //            if (tvis == null) return null;
            //            sc.Visual = tvis;
            //            sc.VisualAABBs = TerrainCollision.CalcAABBs(tvis);
            //            sc.VisualMesh = TerrainRenderer.UploadVisualMesh(tvis);
            //        }
            //        using (Stream scn = File.OpenRead(file.Path))
            //        {
            //            var xml = ScenarioIO.ReadXml(scn);
            //            if (xml == null) return null;
            //            sc.Scenario = xml;
            //        }
            //        return sc;
            //}

            return null;
        }
    }
}

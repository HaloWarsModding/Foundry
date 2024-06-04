using Foundry.HW1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using static WeifenLuo.WinFormsUI.Docking.DockPanelExtender;

namespace Foundry.UI.WinForms
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
        public Workspace Workspace { get; private set; }
        public DockPanel DockPanel { get; private set; }
        public Browser Browser { get; private set; }

        public MainWindow() : base()
        {

            Workspace = new Workspace();

            //window docking
            DockPanel = new DockPanel();
            DockPanel.Dock = DockStyle.Fill;
            DockPanel.Theme = new VS2015LightTheme();
            DockPanel.DefaultFloatWindowSize = new Size(500, 250);
            DockPanel.Theme.Extender.FloatWindowFactory = new WorkspaceFloatWindowFactory();

            //top menu
            MainMenuStrip = new MenuStrip();
            MainMenuStrip.Items.Add(FileItem(Workspace));

            //this
            ClientSize = new Size(942, 493);
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            Text = Properties.Resources.Title;
            Controls.Add(DockPanel);
            Controls.Add(MainMenuStrip);

            //browser
            Browser = new Browser();
            Browser.Show(DockPanel, DockState.DockLeft);
            Browser.BrowserNodeDoubleClicked += (s, e) =>
            {
                if (e.Item is WorkspaceBrowserPath)
                {
                    var path = e.Item as WorkspaceBrowserPath;
                    var item = path.Item;
                    var editor = CreateFileEditor(Workspace, item);
                    if (editor != null)
                    {
                        editor.Show(DockPanel, DockState.Document);
                    }
                }
            };
            //lets make sure the browser updates when we open or close a workspace.
            Workspace.Opened += (s, e) =>
            {
                Browser.RootItems.Clear();
                Browser.RootItems.Add(WorkspaceBrowser.ArtItem(Workspace));
                Browser.RootItems.Add(WorkspaceBrowser.DataItem(Workspace));
                Browser.RootItems.Add(WorkspaceBrowser.ScenarioItem(Workspace));
                Browser.UpdateView();
            };
            Workspace.Closed += (s, e) =>
            {
                Browser.RootItems.Clear();
                Browser.UpdateView();
            };


#if DEBUG
            Workspace.Open("D:\\repos\\Foundry\\_resources\\workspace");
#endif
        }

        public static DockContent CreateFileEditor(Workspace workspace, WorkspaceItem item)
        {
            switch (item.Extension)
            {
                case ".triggerscript":
                    var editor = new TriggerscriptWindow();
                    editor.Text = item.Name;
                    editor.Name = item.Name;
                    var script = workspace.GetTriggerscriptRef(item);
                    if (script == null) return null;
                    editor.TriggerscriptRef = script;
                    return editor;

                case ".xtd":
                    break;
            }

            return null;
        }

        //Note: These items bind the workspace argument to themselves.
        public static ToolStripItem FileItem(Workspace workspace)
        {
            var rootItem = new ToolStripMenuItem("File");
            rootItem.DropDownItems.Add(OpenWorkspaceItem(workspace));
            rootItem.DropDownItems.Add(CloseWorkspaceItem(workspace));
            return rootItem;
        }
        public static ToolStripItem OpenWorkspaceItem(Workspace workspace)
        {
            var openItem = new ToolStripMenuItem("Open Workspace", null, (s, e) =>
            {
                FolderBrowserDialog ofd = new FolderBrowserDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (!workspace.Open(ofd.SelectedPath))
                    {
                        MessageBox.Show("There was an error opening the selected directory.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            });
            return openItem;
        }
        public static ToolStripItem CloseWorkspaceItem(Workspace workspace)
        {
            //Close workspace
            var closeItem = new ToolStripMenuItem("Close Workspace", null, (s, e) =>
            {
                if (workspace.IsOpen)
                {
                    workspace.Close();
                }
            });
            //lets only activate the close button when there is something to close, solely for UX.
            workspace.Opened += (s, e) =>
            {
                closeItem.Enabled = true;
            };
            workspace.Closed += (s, e) =>
            {
                closeItem.Enabled = false;
            };
            return closeItem;
        }
    }
}

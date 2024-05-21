using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Foundry.HW1
{
    public class WorkspaceBrowserGroup : IBrowserViewable
    {
        public IEnumerable<IBrowserViewable> BrowserChildren { get; set; } = new List<IBrowserViewable>();
        public Image Icon { get { return Properties.Resources.folder; } }
        public string Name { get; set; } = "";
    }
    public class WorkspaceBrowserPath : IBrowserViewable
    {
        public WorkspaceItem Item { get; set; }
        public IEnumerable<IBrowserViewable> BrowserChildren
        {
            get
            {
                foreach (WorkspaceItem i in Item.ChildItems)
                {
                    yield return new WorkspaceBrowserPath()
                    {
                        Item = i
                    };
                }
            }
        }
        public Image Icon
        {
            get
            {
                if (_Icon != null) return _Icon;
                if (Item.IsDirectory) return Properties.Resources.folder;
                else switch (Item.Extension)
                    {
                        case ".triggerscript":
                            return Properties.Resources.script;
                        case ".vis":
                            return Properties.Resources.bricks;
                        case ".ugx":
                            return Properties.Resources.brick;
                        case ".uax":
                            return Properties.Resources.chart_line;
                        default:
                            return Properties.Resources.folder_page_white;
                    }
            }
            set
            {
                _Icon = value;
            }
        }
        public string Name
        {
            get
            {
                if (ShowExt) return Item.Name;
                else return Item.NameNoExt;
            }
        }

        private Image _Icon { get; set; }
        public bool ShowExt { get; set; } = true;
    }

    public static class WorkspaceBrowser
    {
        public static IBrowserViewable DataItem(Workspace workspace)
        {
            return new WorkspaceBrowserGroup()
            {
                Name = "data",
                BrowserChildren = new IBrowserViewable[]
                {
                    TriggerscriptsItem(workspace),
                    UserTablesItem(workspace),
                    ObjectsItem(workspace),
                    SquadsItem(workspace)
                }
            };
        }
        public static IBrowserViewable TriggerscriptsItem(Workspace workspace)
        {
            return new WorkspaceBrowserGroup()
            {
                Name = "triggerscripts",
                BrowserChildren = workspace.TriggerscriptFiles.Select(t => new WorkspaceBrowserPath() { Item = t })
            };
        }
        public static IBrowserViewable UserTablesItem(Workspace workspace)
        {
            return new WorkspaceBrowserGroup()
            {
                Name = "tables",
                BrowserChildren = workspace.UserTableFiles.Where(i => i.Extension != ".xmb").Select(t => new WorkspaceBrowserPath()
                {
                    Item = t,
                    Icon = Properties.Resources.table,
                })
            };
        }
        public static IBrowserViewable ObjectsItem(Workspace workspace)
        {
            return new WorkspaceBrowserGroup()
            {
                Name = "objects"
            };
        }
        public static IBrowserViewable SquadsItem(Workspace workspace)
        {
            return new WorkspaceBrowserGroup()
            {
                Name = "squads"
            };
        }

        public static IBrowserViewable ArtItem(Workspace workspace)
        {
            return new WorkspaceBrowserGroup()
            {
                Name = "art",
                BrowserChildren = workspace.Art.ChildItems.Select(i => new WorkspaceBrowserPath() { Item = i })
            };
        }
    }
}
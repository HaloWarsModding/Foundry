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
        public Image Icon { get; set; } = Properties.Resources.folder;
        public string Name { get; set; } = "";
    }
    public class WorkspaceBrowserPath : IBrowserViewable
    {
        public WorkspaceItem Item { get; set; }
        public IEnumerable<IBrowserViewable> BrowserChildren
        {
            get
            {
                if (Item == null) yield break;

                foreach (WorkspaceItem i in Item.ChildDirectories
                    .OrderBy(i => i.Extension + i.Name))
                {
                    yield return new WorkspaceBrowserPath() { Item = i };
                }
                foreach (WorkspaceItem i in Item.ChildFiles
                    .Where(i => i.Extension != ".xmb")
                    .OrderBy(i => i.Extension + i.Name)) //sort by extension first, then name
                {
                    yield return new WorkspaceBrowserPath() { Item = i };
                }
            }
        }
        public Image Icon
        {
            get
            {
                if (Item == null) return null;

                //folder
                if (Item.IsDirectory) return Properties.Resources.folder;

                switch (Item.Extension)
                {
                    //data
                    case ".triggerscript":
                        return Properties.Resources.script_s;
                    case ".ai":
                    case ".table":
                        return Properties.Resources.table;

                    //art
                    case ".ddx":
                    case ".dds":
                        return Properties.Resources.picture;
                    case ".vis":
                        return Properties.Resources.bricks;
                    case ".ugx":
                        return Properties.Resources.brick_green_s;
                    case ".uax":
                        return Properties.Resources.chart_line;
                    case ".dmg":
                        return Properties.Resources.damage;

                    //scenario
                    case ".scn":
                        return Properties.Resources.script_s;

                    //misc
                    case ".xml":
                        return Properties.Resources.page_white_code_red;
                    default:
                        return Properties.Resources.page_white;
                }
            }
        }
        public string Name
        {
            get
            {
                if (Item == null) return "";
                if (ShowExt) return Item.Name;
                else return Item.NameNoExt;
            }
        }

        public bool ShowExt { get; set; } = true;
    }

    public static class WorkspaceBrowser
    {
        public static IBrowserViewable ArtItem(Workspace workspace)
        {
            return new WorkspaceBrowserPath()
            {
                Item = workspace.Art
            };
        }

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
                BrowserChildren = workspace.UserTableFiles.Select(t => new WorkspaceBrowserPath() { Item = t })
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

        public static IBrowserViewable ScenarioItem(Workspace workspace)
        {
            return new WorkspaceBrowserGroup()
            {
                Name = "scenario",
                BrowserChildren = workspace.TerrainFolders.Select(v =>
                {
                    return new WorkspaceBrowserGroup()
                    {
                        Name = v.NameNoExt,
                        Icon = Properties.Resources.map_s,
                        BrowserChildren = v.ChildFiles.Where(f => f.Extension == ".scn").Select(scn => new WorkspaceBrowserPath()
                        {
                            Item = scn,
                            ShowExt = false
                        })
                    };
                })
            };
        }
    }
}
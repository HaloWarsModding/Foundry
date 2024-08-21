using Chef.HW1.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Chef.Win.UI
{
    public class WorkspaceBrowserGroup : IBrowserViewable
    {
        public IEnumerable<IBrowserViewable> BrowserChildren { get; set; } = new List<IBrowserViewable>();
        public Image Icon { get; set; } = null; //Properties.Resources.folder;
        public string Name { get; set; } = "";
    }
    public class WorkspaceBrowserPath : IBrowserViewable
    {
        public WorkspaceDirectory Item { get; set; }
        public IEnumerable<IBrowserViewable> BrowserChildren
        {
            get
            {
                if (Item == null) yield break;

                foreach (WorkspaceDirectory i in Item.ChildDirectories)
                {
                    yield return new WorkspaceBrowserPath() { Item = i };
                }
                foreach (WorkspaceFile i in Item.ChildFiles
                    .Where(i => i.Extension != ".xmb")
                    .OrderBy(i => i.Extension + i.Name)) //sort by extension first, then name
                {
                    yield return new WorkspaceBrowserFile() { Item = i };
                }
            }
        }
        public Image Icon
        {
            get
            {
                return null;// Properties.Resources.folder;
            }
        }
        public string Name
        {
            get
            {
                if (Item == null) return "";
                else return Item.Name;
            }
        }
    }
    public class WorkspaceBrowserFile : IBrowserViewable
    {
        public WorkspaceFile Item { get; set; }
        public IEnumerable<IBrowserViewable> BrowserChildren { get { yield break; } }
        public Image Icon
        {
            get
            {
                if (Item == null) return null;

                switch (Item.Extension)
                {
                    //data
                    case ".triggerscript":
                        return null; // Properties.Resources.script_s;
                    case ".ai":
                    case ".table":
                        return null; //Properties.Resources.table;

                    //art
                    case ".ddx":
                    case ".dds":
                        return null; //Properties.Resources.picture;
                    case ".vis":
                        return null; //Properties.Resources.bricks;
                    case ".ugx":
                        return null; //Properties.Resources.brick_green_s;
                    case ".uax":
                        return null; //Properties.Resources.chart_line;
                    case ".dmg":
                        return null; //Properties.Resources.damage;

                    //scenario
                    case ".scn":
                        return null; //Properties.Resources.script_s;

                    //misc
                    case ".xml":
                        return null; //Properties.Resources.page_white_code_red;
                    default:
                        return null; //Properties.Resources.page_white;
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
        public bool ShowExt { get; set; }
    }

    public static class WorkspaceBrowser
    {
        public static IBrowserViewable ArtItem(WorkspaceRoot workspace)
        {
            return new WorkspaceBrowserPath()
            {
                Item = workspace.Art
            };
        }

        public static IBrowserViewable DataItem(WorkspaceRoot workspace)
        {
            return new WorkspaceBrowserGroup()
            {
                Name = "data",
                BrowserChildren = new IBrowserViewable[]
                {
                    TriggerscriptsItem(workspace.Data),
                    UserTablesItem(workspace.Data),
                    ObjectsItem(workspace.Data),
                    SquadsItem(workspace.Data)
                }
            };
        }
        public static IBrowserViewable TriggerscriptsItem(DataRoot workspace)
        {
            return new WorkspaceBrowserGroup()
            {
                Name = "triggerscripts",
                BrowserChildren = workspace.Triggerscripts.Select(t => new WorkspaceBrowserFile() { Item = t })
            };
        }
        public static IBrowserViewable UserTablesItem(DataRoot workspace)
        {
            return new WorkspaceBrowserGroup()
            {
                Name = "tables",
                BrowserChildren = workspace.DataTables.Select(t => new WorkspaceBrowserFile() { Item = t })
            };
        }
        public static IBrowserViewable ObjectsItem(DataRoot workspace)
        {
            return new WorkspaceBrowserGroup()
            {
                Name = "objects"
            };
        }
        public static IBrowserViewable SquadsItem(DataRoot workspace)
        {
            return new WorkspaceBrowserGroup()
            {
                Name = "squads"
            };
        }

        public static IBrowserViewable ScenarioItem(WorkspaceRoot workspace)
        {
            return new WorkspaceBrowserGroup()
            {
                Name = "scenario",
                BrowserChildren = workspace.Scenario.Missions.Select(v =>
                {
                    return new WorkspaceBrowserGroup()
                    {
                        Name = v.Name,
                        //Icon = Properties.Resources.map_s,
                        BrowserChildren = v.Scenarios.Select(scn => new WorkspaceBrowserFile()
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
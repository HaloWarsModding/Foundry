using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Data.Scenario
{
    public class RootBrowserItem : FolderBrowserItem
    {
        public string Name { get { return "Scenarios"; } }
        public new Image Icon { get { return Properties.Resources.box; } }

        public RootBrowserItem(WorkspaceItem folder) : base(folder)
        {

        }
    }

    public class FolderBrowserItem : IBrowserViewable
    {
        public string Name { get { return Folder.Name; } }
        public Image Icon { get { return Properties.Resources.folder; } }

        private WorkspaceItem Folder { get; set; }

        public FolderBrowserItem(WorkspaceItem folder)
        {
            Folder = folder;
        }

        public IEnumerable<IBrowserViewable> BrowserChildren
        {
            get
            {
                foreach(var item in Folder.ChildDirectories)
                {
                    if (item.ChildFiles.Where(i => i.Extension == ".scn").Any())
                    {
                        yield return new MapBrowserItem(item);
                    }
                    else
                    {
                        yield return new FolderBrowserItem(item);
                    }
                }
            }
        }
    }

    public class MapBrowserItem : IBrowserViewable
    {
        public string Name { get { return Dir.NameNoExt; } }
        public Image Icon { get { return Properties.Resources.map; } }

        private WorkspaceItem Dir { get; set; }

        public MapBrowserItem(WorkspaceItem dir)
        {
            Dir = dir;
        }

        public IEnumerable<IBrowserViewable> BrowserChildren
        {
            get
            {
                if (Dir.IsDirectory)
                {
                    foreach (var file in Dir.ChildFiles.Where(i => i.Extension == ".scn"))
                    {
                        yield return new ScriptBrowserItem(file);
                    }
                }
            }
        }
    }

    public class ScriptBrowserItem : IBrowserViewable
    {
        public string Name { get { return File.NameNoExt; } }
        public Image Icon { get { return Properties.Resources.script_link; } }

        private WorkspaceItem File { get; set; }

        public ScriptBrowserItem(WorkspaceItem file)
        {
            File = file;
        }

        public IEnumerable<IBrowserViewable> BrowserChildren
        {
            get
            {
                return new List<IBrowserViewable>();
            }
        }
    }
}
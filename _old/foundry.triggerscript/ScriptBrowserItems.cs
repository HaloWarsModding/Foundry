using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Triggerscript
{
    public class RootBrowserItem : IBrowserViewable
    {
        public string Name { get { return "Scripts"; } }
        public Image Icon { get { return Foundry.Properties.Resources.box; } }

        private WorkspaceItem Folder { get; set; }

        public RootBrowserItem(WorkspaceItem folder)
        {
            Folder = folder;
        }

        public IEnumerable<IBrowserViewable> BrowserChildren
        {
            get
            {
                foreach (var item in Folder.ChildFiles.Where(i => i.Extension == ".triggerscript"))
                {
                    yield return new ScriptBrowserItem(item);
                }
            }
        }
    }

    public class ScriptBrowserItem : IBrowserViewable
    {
        public string Name { get { return Dir.NameNoExt; } }
        public Image Icon { get { return Foundry.Properties.Resources.script; } }

        private WorkspaceItem Dir { get; set; }

        public ScriptBrowserItem(WorkspaceItem dir)
        {
            Dir = dir;
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
using HelixToolkit.SharpDX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Asset
{
    public class VisBrowserItem : IBrowserViewable
    {
        public AssetModule Owner { get; private set; }
        public WorkspaceItem File { get; private set; }
        public string Name { get { return File.Name; } }
        public Image Icon { get { return Properties.Resources.bricks; } }

        public VisBrowserItem(AssetModule owner, WorkspaceItem file)
        {
        }

        public IEnumerable<IBrowserViewable> BrowserChildren
        {
            get
            {
                return new List<IBrowserViewable>();
            }
        }
    }

    public class GroupBrowserItem : IBrowserViewable
    {
        public AssetModule Owner { get; private set; }
        public List<GroupBrowserItem> ChildGroups { get; private set; }
        public List<VisBrowserItem> ChildVisFiles { get; private set; }
        public string Name { get; set; }
        public Image Icon { get { return Properties.Resources.folder; } }

        public GroupBrowserItem(AssetModule owner)
        {
            Owner = owner;
            ChildGroups = new List<GroupBrowserItem>();
            ChildVisFiles = new List<VisBrowserItem>();
        }

        public IEnumerable<IBrowserViewable> BrowserChildren
        {
            get
            {
                foreach (var i in ChildGroups)
                {
                    yield return i;
                }
                foreach (var i in ChildVisFiles)
                {
                    yield return i;
                }
            }
        }
    }
    public class RootGroupBrowserItem : GroupBrowserItem
    {
        public new string Name { get { return "Art"; } }
        public new Image Icon { get { return Properties.Resources.package; } }

        public RootGroupBrowserItem(AssetModule owner) : base(owner)
        {
        }
    }
}
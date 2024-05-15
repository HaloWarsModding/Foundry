using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Art
{
    public class ArtBrowserRootItem : ArtBrowserItem
    {
        public new Image Icon { get { return Properties.Resources.package; } }
        public new string Name { get { return "Art"; } }

        public ArtBrowserRootItem(ArtModule owner, WorkspaceItem item) : base(owner, item)
        {
        }
    }

    public class ArtBrowserItem : IBrowserViewable
    {
        public event EventHandler Clicked;
        public event EventHandler DoubleClicked;

        public ArtModule Owner { get; private set; }
        public WorkspaceItem Item { get; private set; }
        public Image Icon
        {
            get
            {
                if (Item.IsDirectory)
                {
                    return Properties.Resources.folder;
                }
                else
                {
                    if (Owner.ExtensionIcons.ContainsKey(Item.Extension))
                    {
                        return Owner.ExtensionIcons[Item.Extension];
                    }
                }
                return null;
            }
        }
        public string Name { get { return Item.Name; } }

        public ArtBrowserItem(ArtModule owner, WorkspaceItem item)
        {
            Owner = owner;
            Item = item;
        }

        public IEnumerable<IBrowserViewable> BrowserChildren
        {
            get
            {
                foreach (var i in Item.ChildDirectories)
                {
                    yield return new ArtBrowserItem(Owner, i);
                }
                foreach (var i in Item.ChildFiles.Where(i => Owner.ExtensionFilter.Contains(i.Extension)))
                {
                    yield return new ArtBrowserItem(Owner, i);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Art
{
    public class ArtModule : BaseModule
    {
        public event EventHandler<WorkspaceItemChangedArgs> ArtItemChanged;

        public BrowserView Browser { get; private set; }
        public ArtBrowserRootItem RootArtBrowserItem { get; private set; }

        public List<string> ExtensionFilter { get; private set; }
        public Dictionary<string, Image> ExtensionIcons { get; private set; }

        public ArtModule()
        {
            ExtensionFilter = new List<string>();
            ExtensionIcons = new Dictionary<string, Image>();
            Browser = new BrowserView(Instance);
            Browser.Form.Text = "Art";
        }

        protected override void OnPostInit()
        {
            Browser.Show(Instance, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
        }
        protected override void OnWorkspaceOpened()
        {
            RootArtBrowserItem = new ArtBrowserRootItem(this, Instance.GetNamedWorkspaceDir(FoundryInstance.NamedWorkspaceDirNames.Art));
            Instance.WorkspaceItemChanged += OnWorkspaceItemChanged;
            Browser.RootItems.Add(RootArtBrowserItem);
            Browser.UpdateView();
        }
        protected override void OnWorkspaceClosed()
        {
            Instance.WorkspaceItemChanged -= OnWorkspaceItemChanged;
            Browser.RootItems.Remove(RootArtBrowserItem);
            Browser.Close(true);
            RootArtBrowserItem = null;
        }

        private void OnWorkspaceItemChanged(object o, WorkspaceItemChangedArgs args)
        {
            if(args.Item.IsRelativeTo(Instance.GetNamedWorkspaceDir(FoundryInstance.NamedWorkspaceDirNames.Art)))
            {
                ArtItemChanged?.Invoke(this, args);
            }
        }
    }
}

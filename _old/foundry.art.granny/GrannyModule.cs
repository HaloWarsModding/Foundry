namespace Foundry.Art.Granny
{
    public class GrannyModule : BaseModule
    {
        public ArtModule ArtModuleInstance { get; private set; }

        public GrannyModule()
        {
        }

        protected override void OnPostInit()
        {
            ArtModule art = null;
            Instance.GetModuleByType(out art);
            ArtModuleInstance = art;

            ArtModuleInstance.ExtensionFilter.Add(".ugx");
            ArtModuleInstance.ExtensionIcons.Add(".ugx", Properties.Resources.bricks);
            ArtModuleInstance.ExtensionFilter.Add(".uax");
            ArtModuleInstance.ExtensionIcons.Add(".uax", Properties.Resources.chart_line);

        }
        protected override void OnWorkspaceOpened()
        {
            //OnArtDirChanged(null, new WorkspaceItemChangedArgs() { Item = ArtModuleInstance.RootArtBrowserItem.Folder, Type = WorkspaceItemChangedType.FileAdded });
            Instance.Browser.BrowserNodeClicked += OnNodeClicked;
        }
        protected override void OnWorkspaceClosed()
        {
            Instance.Browser.BrowserNodeClicked -= OnNodeClicked;
        }

        private void OnNodeClicked(object o, BrowserNodeClickedArgs args)
        {
            if (args.Button == MouseButtons.Right
                && args.Item is ArtBrowserItem)
            {
                ArtBrowserItem abi = (ArtBrowserItem)args.Item;

                ContextMenuStrip menu = new ContextMenuStrip();

                if (abi.Item.Extension == ".ugx")
                {
                    menu.Items.Add("View Mesh", null, (s, e) =>
                    {
                        UgxViewerPage page = new UgxViewerPage(this, abi.Item);
                        page.Show(Instance, WeifenLuo.WinFormsUI.Docking.DockState.Document);
                    });
                }
            }
        }
    }
}

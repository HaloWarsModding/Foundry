namespace Foundry.Data
{
    public class DataModule : BaseModule
    {
        public event EventHandler<WorkspaceItemChangedArgs> ArtItemChanged;

        public BrowserView Browser { get; private set; }
        private Dictionary<WorkspaceItem, OperatorRegistrantToolstrip> ArtFileOperatorRegistrants { get; set; }

        protected override void OnInit()
        {
            ArtFileOperatorRegistrants = new Dictionary<WorkspaceItem, OperatorRegistrantToolstrip>();

            Browser = new BrowserView(Instance);
            Browser.Form.Text = "Data";
            //Browser.BrowserNodeClicked += (s, e) =>
            //{
            //    if (e.Item is null) return;
            //    if (e.Item is not WorkspaceItem) return;

            //    WorkspaceItem item = (WorkspaceItem)e.Item;

            //    SelectedArtItem = item;

            //    if (e.Button == MouseButtons.Right && ArtFileOperatorRegistrants.ContainsKey(item))
            //    {
            //        ContextMenuStrip cms = new ContextMenuStrip();
            //        cms.Items.AddRange(ArtFileOperatorRegistrants[item].GetRootMenuItems().ToArray());
            //        cms.Show(Browser.Form, e.Location);
            //    }
            //};
        }
        protected override void OnPostInit()
        {
        }
        protected override void OnWorkspaceOpened()
        {
            Instance.WorkspaceItemChanged += OnWorkspaceItemChanged;
            Browser.Show(Instance, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
        }
        protected override void OnWorkspaceClosed()
        {
            Instance.WorkspaceItemChanged -= OnWorkspaceItemChanged;
            Browser.RootItems.Clear();
            Browser.Close(true);
        }

        public OperatorRegistrantToolstrip GetArtFileOperatorRegistrant(WorkspaceItem item)
        {
            if (!ArtFileOperatorRegistrants.ContainsKey(item))
            {
                ArtFileOperatorRegistrants.Add(item, new OperatorRegistrantToolstrip());
            }

            return ArtFileOperatorRegistrants[item];
        }

        private void OnWorkspaceItemChanged(object o, WorkspaceItemChangedArgs args)
        {
            if (args.Item.IsRelativeTo(Instance.GetNamedWorkspaceDir(FoundryInstance.NamedWorkspaceDirNames.Art)))
            {
                ArtItemChanged?.Invoke(this, args);
            }
        }
    }
}
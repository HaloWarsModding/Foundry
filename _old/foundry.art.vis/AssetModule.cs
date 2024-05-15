using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
using WeifenLuo.WinFormsUI.Docking;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace Foundry.Asset
{
    public class AssetModule : BaseModule
    {
        public WorkspaceItem ArtRoot { get; private set; }
        public List<WorkspaceItem> VisFiles { get; private set; }
        public List<WorkspaceItem> UgxFiles { get; private set; }
        public List<WorkspaceItem> UaxFiles { get; private set; }

        private Dictionary<WorkspaceItem, RefManager<VisXmlData>> VisData { get; set; }
        private Dictionary<WorkspaceItem, RefManager<UgxBinData>> UgxData { get; set; }

        private RootGroupBrowserItem BrowserRoot { get; set; }

        protected override void OnPostInit()
        {
            VisFiles = new List<WorkspaceItem>();
            VisData = new Dictionary<WorkspaceItem, RefManager<VisXmlData>>();
            UgxFiles = new List<WorkspaceItem>();
            UgxData = new Dictionary<WorkspaceItem, RefManager<UgxBinData>>();
            UaxFiles = new List<WorkspaceItem>();
            BrowserRoot = new RootGroupBrowserItem(this);
            Instance.Browser.RootItems.Add(BrowserRoot);
        }
        protected override void OnWorkspaceOpened()
        {
            ArtRoot = new WorkspaceItem(Instance.GetNamedWorkspaceDir(FoundryInstance.NamedWorkspaceDirNames.WorkspaceFolder).FullPath + "art/");
            UpdateIndex();
            UpdateView();
        }
        protected override void OnWorkspaceClosed()
        {
            ArtRoot = null;
            UpdateIndex();
            UpdateView();
        }

        public RefWrapper<VisXmlData> GetVisData(WorkspaceItem file)
        {
            if(!VisData.ContainsKey(file))
            {
                VisXmlData data = new YAXSerializer<VisXmlData>().DeserializeFromFile(file.FullPath);
                VisData.Add(file, new RefManager<VisXmlData>(data));
            }
            return VisData[file].GetRef();
        }
        public RefWrapper<UgxBinData> GetUgxData(WorkspaceItem file)
        {
            if (!UgxData.ContainsKey(file))
            {
                UgxBinData data = UgxBinData.ImportUgxGeometry(file);
                UgxData.Add(file, new RefManager<UgxBinData>(data));
            }
            return UgxData[file].GetRef();
        }

        private void UpdateIndex()
        {
            VisFiles.Clear();
            UgxFiles.Clear();
            UaxFiles.Clear();
            PurgeData();

            if (ArtRoot == null) return;

            foreach (var file in ArtRoot.ChildFilesRecursive)
            {
                if (file.Extension == ".vis")
                {
                    VisFiles.Add(file);
                }
                if (file.Extension == ".ugx")
                {
                    UgxFiles.Add(file);
                }
                if (file.Extension == ".uax")
                {
                    UaxFiles.Add(file);
                }
            }
        }
        private void UpdateView()
        {
            BrowserRoot.ChildGroups.Clear();
            BrowserRoot.ChildVisFiles.Clear();

            Dictionary<WorkspaceItem, GroupBrowserItem> groups = new Dictionary<WorkspaceItem, GroupBrowserItem>();

            foreach (var file in VisFiles)
            {
                WorkspaceItem first = file.ParentDirectory;
                WorkspaceItem last = first;
                if (!groups.ContainsKey(first))
                {
                    groups.Add(first, new GroupBrowserItem(this) { Name = first.Name });
                }

                while (last.IsRelativeTo(ArtRoot) && last != ArtRoot)
                {
                    WorkspaceItem cur = last.ParentDirectory;

                    if(!groups.ContainsKey(cur))
                    {
                        groups.Add(cur, new GroupBrowserItem(this) { Name = cur.Name });
                    }

                    if (!groups[cur].ChildGroups.Contains(groups[last]))
                    {
                        groups[cur].ChildGroups.Add(groups[last]);
                    }

                    last = cur;
                }

                if (!BrowserRoot.ChildGroups.Contains(groups[last]))
                {
                    BrowserRoot.ChildGroups.Add(groups[last]);
                }
                
                groups[first].ChildVisFiles.Add(new VisBrowserItem(this, file));
            }
        }
        private void PurgeData()
        {
            foreach(var item in VisData)
            {
                if (item.Value.RefCount == 0)
                {
                    VisData.Remove(item.Key);
                }
            }
        }
    }
}
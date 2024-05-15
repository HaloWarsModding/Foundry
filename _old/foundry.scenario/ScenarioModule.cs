using Foundry;
using YAXLib;

namespace Foundry.Data.Scenario
{
    public class ScenarioScriptItem
    {
        public ScenarioDirectoryItem Owner { get; private set; }
        public ScenarioScriptItem(ScenarioDirectoryItem owner, string name)
        {
            Owner = owner;
            Name = name;
        }

        public string Name
        {
            get;
            private set;
        }
        public Image Icon
        {
            get { return Properties.Resources.script; }
        }

        public WorkspaceItem ScnFile
        {
            get
            {
                var found = Owner.ChildFiles.Where(i => i.Name == Name + ".scn");
                return found.Count() > 0 ? found.First() : null;
            }
        }
        public WorkspaceItem Sc2File
        {
            get
            {
                var found = Owner.ChildFiles.Where(i => i.Name == Name + ".sc2");
                return found.Count() > 0 ? found.First() : null;
            }
        }
        public WorkspaceItem Sc3File
        {
            get
            {
                var found = Owner.ChildFiles.Where(i => i.Name == Name + ".sc3");
                return found.Count() > 0 ? found.First() : null;
            }
        }
    }
    public class ScenarioDirectoryItem : WorkspaceItem
    {
        public ScenarioDirectoryItem(string path) : base(path)
        {
        }

        public new Image Icon
        {
            get
            {
                return Properties.Resources.map;
            }
        }

        public IEnumerable<ScenarioScriptItem> ScriptItems
        {
            get
            {
                List<ScenarioScriptItem> scripts = new List<ScenarioScriptItem>();
                foreach (WorkspaceItem script in ChildFiles.Where(i => i.Extension == ".scn"))
                {
                    scripts.Add(new ScenarioScriptItem(this, script.NameNoExt));
                }
                return scripts;
            }
        }
        public WorkspaceItem XtdFile
        {
            get
            {
                var found = ChildFiles.Where(i => i.Extension == ".xtd");
                return found.Count() > 0 ? found.First() : null;
            }
        }
        public WorkspaceItem XsdFile
        {
            get
            {
                var found = ChildFiles.Where(i => i.Extension == ".xsd");
                return found.Count() > 0 ? found.First() : null;
            }
        }
        public WorkspaceItem XthFile
        {
            get
            {
                var found = ChildFiles.Where(i => i.Extension == ".xth");
                return found.Count() > 0 ? found.First() : null;
            }
        }
        public WorkspaceItem XttFile
        {
            get
            {
                var found = ChildFiles.Where(i => i.Extension == ".xtt");
                return found.Count() > 0 ? found.First() : null;
            }
        }
    }

    public class ScenarioModule : BaseModule
    {
        public event EventHandler<WorkspaceItemChangedArgs> ScenarioItemChanged;
        
        private Dictionary<ScenarioDirectoryItem, OperatorRegistrantToolstrip> ScenarioDirOperatorRegistrants { get; set; }

        protected override void OnPostInit()
        {
            ScenarioDirOperatorRegistrants = new Dictionary<ScenarioDirectoryItem, OperatorRegistrantToolstrip>();

            Instance.WorkspaceItemChanged += OnWorkspaceItemChanged;
        }
        protected override void OnWorkspaceOpened()
        {
            Instance.Browser.RootItems.Add(
                new RootBrowserItem(Instance.GetNamedWorkspaceDir(FoundryInstance.NamedWorkspaceDirNames.Scenarios))
                );
            Instance.Browser.UpdateView();
        }
        protected override void OnWorkspaceClosed()
        {
        }

        public OperatorRegistrantToolstrip GetScenarioDirOperatorRegistrant(ScenarioDirectoryItem item)
        {
            if (!ScenarioDirOperatorRegistrants.ContainsKey(item))
            {
                ScenarioDirOperatorRegistrants.Add(item, new OperatorRegistrantToolstrip());
            }

            return ScenarioDirOperatorRegistrants[item];
        }

        private void OnWorkspaceItemChanged(object o, WorkspaceItemChangedArgs args)
        {
            if (args.Item.IsRelativeTo(Instance.GetNamedWorkspaceDir(FoundryInstance.NamedWorkspaceDirNames.Scenarios)))
            {
                //if (args.Item.Extension == ".ugx")
                //{
                //    if (args.Type == WorkspaceItemChangedType.FileAdded)
                //    {
                //        Operator op = new Operator("View Mesh");
                //        op.OperatorActivated += (s, e) =>
                //        {
                //            ScenarioEditorPage page = new UgxViewerPage(this, args.Item);
                //            page.Text = args.Item.Name;
                //            page.TryShow(Instance, WeifenLuo.WinFormsUI.Docking.DockState.Document);
                //        };
                //        GrannyOps.Add(args.Item, op);
                //        GetScenarioDirOperatorRegistrant(args.Item).AddOperator(op);
                //    }
                //    else if (args.Type == WorkspaceItemChangedType.FileRemoved)
                //    {
                //        GetScenarioDirOperatorRegistrant(args.Item).RemoveOperator(GrannyOps[args.Item]);
                //    }
            }

            ScenarioItemChanged?.Invoke(this, args);
        }
    }
}
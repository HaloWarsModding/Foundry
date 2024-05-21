using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Foundry.HW1.WorkspaceMenus;
using Foundry.HW1.Unit;
using Foundry.HW1.Triggerscript;

namespace Foundry.HW1
{
    public enum WorkspaceItemChangedType
    {
        FileAdded,
        FileRemoved,
        FileChanged,
    }
    public class WorkspaceItemChangedArgs
    {
        public WorkspaceItem Item { get; set; }
        public WorkspaceItemChangedType Type { get; set; }
    }

    public class Workspace
    {
        public event EventHandler WorkspaceOpened;
        public event EventHandler WorkspaceClosed;
        public event EventHandler<WorkspaceItemChangedArgs> WorkspaceItemChanged;

        public bool IsOpen { get { return Root != null; } }

        public WorkspaceItem Root { get; private set; } = null;
        public WorkspaceItem Data
        {
            get
            {
                if (Root == null) return null;
                return Root.ChildDirectories
                    .Where(d => d.Name == "data")
                    .FirstOrDefault((WorkspaceItem)null);
            }
        }
        public WorkspaceItem DataTriggerscripts
        {
            get
            {
                if (Data == null) return null;
                return Data.ChildDirectories
                    .Where(d => d.Name == "triggerscripts")
                    .FirstOrDefault((WorkspaceItem)null);
            }
        }
        public WorkspaceItem DataUserTables
        {
            get
            {
                if (Data == null) return null;
                return Data.ChildDirectories
                    .Where(d => d.Name == "aidata")
                    .FirstOrDefault((WorkspaceItem)null);
            }
        }
        public WorkspaceItem DataTactics
        {
            get
            {
                if (Data == null) return null;
                return Data.ChildDirectories
                    .Where(d => d.Name == "aidata")
                    .FirstOrDefault((WorkspaceItem)null);
            }
        }
        public WorkspaceItem Scenario
        {
            get
            {
                if (Root == null) return null;
                return Root.ChildDirectories
                    .Where(d => d.Name == "scenario")
                    .FirstOrDefault((WorkspaceItem)null);
            }
        }
        public WorkspaceItem Art
        {
            get
            {
                if (Root == null) return null;
                return Root.ChildDirectories
                    .Where(d => d.Name == "art")
                    .FirstOrDefault((WorkspaceItem)null);
            }
        } 
        public IEnumerable<WorkspaceItem> TerrainFolders
        {
            get
            {
                if (Scenario == null) return new List<WorkspaceItem>();
                return Scenario.ChildDirectories.Where(
                        d => d.ChildFiles.Any(
                            f => f.Extension == ".xtd"));
            }
        }
        public IEnumerable<WorkspaceItem> TriggerscriptFiles
        {
            get
            {
                if (DataTriggerscripts == null) return new List<WorkspaceItem>();
                return DataTriggerscripts.ChildFiles.Where(f => f.Extension == ".triggerscript");
            }
        }
        public IEnumerable<WorkspaceItem> UserTableFiles
        {
            get
            {
                if (DataUserTables == null) return new List<WorkspaceItem>();
                return DataUserTables.ChildFiles;
            }
        }
        public IEnumerable<WorkspaceItem> ObjectFiles
        {
            get
            {
                if (Data == null) return new List<WorkspaceItem>();
                return Data.ChildFiles.Where(f => f.Name == "objects.xml" || f.Name == "objects_update.xml");
            }
        }
        public IEnumerable<WorkspaceItem> SquadFiles
        {
            get
            {
                if (Data == null) return new List<WorkspaceItem>();
                return Data.ChildFiles.Where(f => f.Name == "squads.xml" || f.Name == "squads_update.xml");
            }
        }
        public IEnumerable<WorkspaceItem> VisFiles
        {
            get
            {
                if (Art == null) return null;
                return Art.ChildFilesRecursive.Where(f => f.Extension == ".vis");
            }
        }

        private FileSystemWatcher WorkspaceWatcher { get; set; }

        public bool Open(string directory)
        {
            if (!Directory.Exists(directory)) return false;
            if (IsOpen && !Close()) return false;

            Root = new WorkspaceItem(directory);
            EnsureSubDirsExist();

            //fire an event for each currently present file.
            foreach (WorkspaceItem child in Root.ChildItemsRecursive)
            {
                WorkspaceItemChanged?.Invoke(this, new WorkspaceItemChangedArgs()
                {
                    Item = child,
                    Type = WorkspaceItemChangedType.FileAdded
                });
            }
            StartFileSystemWatcher();

            WorkspaceOpened?.Invoke(this, EventArgs.Empty);

            return true;
        }
        public bool Close()
        {
            if (!IsOpen) return true;

            StopFileSystemWatcher();

            //fire an event for each currently present file.
            foreach (WorkspaceItem child in Root.ChildItemsRecursive)
            {
                WorkspaceItemChanged?.Invoke(this, new WorkspaceItemChangedArgs()
                {
                    Item = child,
                    Type = WorkspaceItemChangedType.FileRemoved
                });
            }

            Root = null;

            WorkspaceClosed?.Invoke(this, EventArgs.Empty);

            return true;
        }

        private void StartFileSystemWatcher()
        {
            //start the file watcher.
            WorkspaceWatcher = new FileSystemWatcher();
            WorkspaceWatcher.Filter = "*.*";
            WorkspaceWatcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;
            WorkspaceWatcher.Path = Root.FullPath;
            WorkspaceWatcher.IncludeSubdirectories = true;
            WorkspaceWatcher.Created += (s, e) =>
            {
                WorkspaceItemChanged?.Invoke(this, new WorkspaceItemChangedArgs()
                {
                    Item = new WorkspaceItem(e.FullPath),
                    Type = WorkspaceItemChangedType.FileAdded
                });
            };
            WorkspaceWatcher.Deleted += (s, e) =>
            {
                WorkspaceItemChanged?.Invoke(this, new WorkspaceItemChangedArgs()
                {
                    Item = new WorkspaceItem(e.FullPath),
                    Type = WorkspaceItemChangedType.FileRemoved
                });
            };
            WorkspaceWatcher.Changed += (s, e) =>
            {
                WorkspaceItemChanged?.Invoke(this, new WorkspaceItemChangedArgs()
                {
                    Item = new WorkspaceItem(e.FullPath),
                    Type = WorkspaceItemChangedType.FileChanged
                });
            };

            WorkspaceWatcher.EnableRaisingEvents = true;
        }
        private void StopFileSystemWatcher()
        {
            WorkspaceWatcher = null;
        }
        private void EnsureSubDirsExist()
        {
            if (Data == null)
                Directory.CreateDirectory("data");

            if (Art == null)
                Directory.CreateDirectory("art");
        }
    }
}

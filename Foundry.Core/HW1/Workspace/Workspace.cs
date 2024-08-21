using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chef.HW1.Unit;
using Chef.HW1.Script;
using Chef.HW1.Map;
using YAXLib;
using YAXLib.Enums;
using YAXLib.Options;

namespace Chef.HW1.Workspace
{
    public class WorkspaceFile
    {
        public WorkspaceFile(string path)
        {
            Path = path;
        }
        public string Path { get; private set; }
        public string Name
        {
            get
            {
                return System.IO.Path.GetFileName(Path);
            }
        }
        public string Extension
        {
            get
            {
                return System.IO.Path.GetExtension(Path);
            }
        }
        public string NameNoExt
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(Path);
            }
        }
        public WorkspaceDirectory Parent
        {
            get
            {
                return new WorkspaceDirectory(Directory.GetParent(Path).Name);
            }
        }
    }
    public class WorkspaceDirectory
    {
        public WorkspaceDirectory(string path)
        {
            Path = path;
            Watcher = new FileSystemWatcher(Path);
            Watcher.IncludeSubdirectories = false;
            Watcher.EnableRaisingEvents = true;
            Watcher.Created += (s, e) =>
            {

            };
        }
        public string Path { get; private set; }
        public string Name
        {
            get
            {
                return System.IO.Path.GetFileName(Path);
            }
        }
        public WorkspaceDirectory Parent
        {
            get
            {
                return new WorkspaceDirectory(Directory.GetParent(Path).Name);
            }
        }
        public IEnumerable<WorkspaceDirectory> ChildDirectories
        {
            get
            {
                foreach (var dir in Directory.GetDirectories(Path))
                {
                    yield return new WorkspaceDirectory(dir);
                }
            }
        }
        public IEnumerable<WorkspaceDirectory> ChildDirectoriesRecursive
        {
            get
            {
                foreach (var dir in ChildDirectories)
                {
                    yield return dir;
                    foreach (var dirrec in dir.ChildDirectoriesRecursive)
                    {
                        yield return dirrec;
                    }
                }
            }
        }
        public IEnumerable<WorkspaceFile> ChildFiles
        {
            get
            {
                foreach (var file in Directory.GetFiles(Path))
                {
                    yield return new WorkspaceFile(file);
                }
            }
        }
        public IEnumerable<WorkspaceFile> ChildFilesRecursive
        {
            get
            {
                foreach (var file in ChildFiles)
                {
                    yield return file; //files in our dir
                }
                foreach (var dir in ChildDirectoriesRecursive) //files in all subdirs
                {
                    foreach (var file in dir.ChildFiles)
                    {
                        yield return file;
                    }
                }
            }
        }


        public event EventHandler<WorkspaceFile> FileAdded;
        public event EventHandler<WorkspaceFile> FileRemoved;
        private FileSystemWatcher Watcher { get; }
    }

    public class ArtVisualFile : WorkspaceFile
    {
        public ArtVisualFile(string path) : base(path)
        {
        }
    }
    public class ArtRoot : WorkspaceDirectory
    {
        public ArtRoot(string path) : base(path)
        {
        }

        public IEnumerable<WorkspaceFile> Visuals
        {
            get
            {
                return ChildFilesRecursive.Where(f => f.Extension == ".vis");
            }
        }
        public IEnumerable<WorkspaceFile> Damage
        {
            get
            {
                return ChildFilesRecursive.Where(f => f.Extension == ".dmg");
            }
        }
        public IEnumerable<WorkspaceFile> Models
        {
            get
            {
                return ChildFilesRecursive.Where(f => f.Extension == ".ugx");
            }
        }
        public IEnumerable<WorkspaceFile> Animations
        {
            get
            {
                return ChildFilesRecursive.Where(f => f.Extension == ".uax");
            }
        }
    }

    public class DataRoot : WorkspaceDirectory
    {
        public DataRoot(string path) : base(path)
        {
        }

        public IEnumerable<WorkspaceFile> Triggerscripts
        {
            get
            {
                WorkspaceDirectory subdir = ChildDirectories.Where(d => d.Name == "triggerscripts").FirstOrDefault((WorkspaceDirectory)null);
                if (subdir == null) return new WorkspaceFile[0];
                return subdir.ChildFilesRecursive.Where(f => f.Extension == ".triggerscript");
            }
        }
        public IEnumerable<WorkspaceFile> DataTables
        {
            get
            {
                WorkspaceDirectory subdir = ChildDirectories.Where(d => d.Name == "aidata").FirstOrDefault((WorkspaceDirectory)null);
                if (subdir == null) return new WorkspaceFile[0];
                return subdir.ChildFilesRecursive.Where(f => f.Extension == ".ai" || f.Extension == ".table");
            }
        }
        public IEnumerable<WorkspaceFile> Objects
        {
            get
            {
                return ChildFiles.Where(f => f.Name == "objects.xml" || f.Name == "objects_update.xml");
            }
        }
    }

    public class ScenarioDirectory : WorkspaceDirectory
    {
        public ScenarioDirectory(string path) : base(path)
        {
        }

        public WorkspaceFile TerrainVisual
        {
            get
            {
                return ChildFiles.Where(f => f.Extension == ".xtd").FirstOrDefault((WorkspaceFile)null);
            }
        }
        public WorkspaceFile TerrainSim
        {
            get
            {
                return ChildFiles.Where(f => f.Extension == ".xsd").FirstOrDefault((WorkspaceFile)null);
            }
        }
        public IEnumerable<WorkspaceFile> Scenarios
        {
            get
            {
                return ChildFiles.Where(f => f.Extension == ".scn");
            }
        }
    }
    public class ScenarioRoot : WorkspaceDirectory
    {
        public ScenarioRoot(string path) : base(path)
        {
        }
        public IEnumerable<ScenarioDirectory> Missions
        {
            get
            {
                foreach (var f in ChildDirectoriesRecursive)
                {
                    var m = new ScenarioDirectory(f.Path);
                    if (m.TerrainVisual != null &&
                        m.TerrainSim != null)
                        yield return m;
                }
            }
        }
    }

    public class WorkspaceRoot : WorkspaceDirectory
    {
        public WorkspaceRoot(string path) : base(path)
        {
        }
        public ArtRoot Art
        {
            get
            {
                var dir = ChildDirectories.Where(d => d.Name.ToLower() == "art").FirstOrDefault((WorkspaceDirectory)null);
                if (dir == null) return null;
                return new ArtRoot(dir.Path);
            }
        }
        public DataRoot Data
        {
            get
            {
                var dir = ChildDirectories.Where(d => d.Name.ToLower() == "data").FirstOrDefault((WorkspaceDirectory)null);
                if (dir == null) return null;
                return new DataRoot(dir.Path);
            }
        }
        public ScenarioRoot Scenario
        {
            get
            {
                var dir = ChildDirectories.Where(d => d.Name.ToLower() == "scenario").FirstOrDefault((WorkspaceDirectory)null);
                if (dir == null) return null;
                return new ScenarioRoot(dir.Path);
            }
        }
    }
}

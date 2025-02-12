using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chef.HW1.Workspace
{
    public class NodeFolder : WorkspaceNode
    {
        public NodeFolder(string path, bool isroot)
        {
            _Path = path;
            _IsRoot = isroot;
        }

        public override bool[] Bool(string name) { return null; }
        public override float[] Float(string name) { return null; }
        public override int[] Int(string name) { return null; }
        public override string[] String(string name) { return null; }

        protected override WorkspaceNode _Parent()
        {
            var dir = Directory.GetParent(_Path);
            if (!_IsRoot && dir != null && dir.Exists)
                return new NodeFolder(dir.FullName, false);
            return null;
        }
        protected override WorkspaceNode _Child(string name)
        {
            var entries = Directory.EnumerateFileSystemEntries(_Path);
            foreach(var e in entries)
            {
                if (Directory.Exists(e) && Path.GetDirectoryName(e) == name)
                    return new NodeFolder(e, false);
            }
            return null;
        }

        private string _Path { get; set; }
        private bool _IsRoot { get; set; }
    }
}

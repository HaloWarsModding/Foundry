using System;
using System.Numerics;

namespace Chef.HW1.Workspace
{
    public enum WorkspaceAttributeType
    {
        Float, Bool, Int, String
    }

    public class WorkspaceAttribute
    {
        public void SetFloat(string name, float[] value)
        {

        }

        public WorkspaceAttributeType Type { get; private set; }
    }

    public abstract class WorkspaceNode
    {
        public WorkspaceNode(WorkspaceNode parent)
        {
            _Cache = new Dictionary<string, WorkspaceNode>();
            _Parent = parent;
        }

        public WorkspaceNode Parent()
        {
            return _Parent();
        }
        public WorkspaceNode Root()
        {
            WorkspaceNode parent = this;
            while (parent != null)
            {
                parent = parent.Parent();
            }
            return parent;
        }

        public WorkspaceNode Child(string name)
        {
            if (_Cache.ContainsKey(name) && _Cache[name] != null)
                return _Cache[name];
            var child = _Child(name);
            _Cache[name] = child;
            return child;
        }
        public WorkspaceNode Child(string[] names)
        {
            WorkspaceNode cur = this;
            foreach (string name in names)
            {
                cur = cur.Child(name);
                if (cur == null) return null;
            }
            return cur;
        }

        //public float[] Float(string name);
        //public int[] Int(string name);
        //public bool[] Bool(string name);
        //public string[] String(string name);

        protected abstract WorkspaceNode _Child(string name);
        private Dictionary<string, WorkspaceNode> _Cache;
        private WorkspaceNode _Parent;
    }

    //public class WorkspaceNode
    //{
    //    public Dictionary<string, WorkspaceNode> Nodes;
    //}

    //public static class Workspace
    //{
    //    public static WorkspaceNode 
    //}
}

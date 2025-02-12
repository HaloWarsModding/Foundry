using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chef.HW1.Workspace
{
    public class NodeRootHw1 : WorkspaceNode
    {
        public override bool[] Bool(string name) { return null; }
        public override float[] Float(string name) { return null; }
        public override int[] Int(string name) { return null; }
        public override string[] String(string name) { return null; }
        protected override WorkspaceNode _Parent() { return null; }

        protected override WorkspaceNode _Child(string name)
        {
        }
    }
}

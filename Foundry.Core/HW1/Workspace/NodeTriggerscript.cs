using Chef.HW1.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chef.HW1.Workspace
{
    public class NodeTriggerscript : WorkspaceNode
    {
        public NodeTriggerscript()
        {

        }

        public override bool[] Bool(string name) { return null; }
        public override float[] Float(string name) { return null; }
        public override int[] Int(string name) { return null; }
        public override string[] String(string name) { return null; }
        protected override WorkspaceNode _Parent() { return null; }
        protected override WorkspaceNode _Child(string name)
        {
            VarType[] types = Enum.GetValues<VarType>().ToArray();
            for (int i = 0; i < types.Count(); i++)
            {
                if (name.ToLower() == types[i].ToString().ToLower())
                {

                }
            }
            return null;
        }

        Triggerscript _script;
    }
}

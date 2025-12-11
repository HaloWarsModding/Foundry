using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chef.Workspace.HW1.Root.Scenario
{
    public class ScenarioFolder : Node
    {
        protected override Node _Child(string name)
        {
            if (File.Exists(FullName ))

            return new ScenarioFolder();
        }

        protected override Stream _Seek(string name, Stream src)
        {
        }
    }
}

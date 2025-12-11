using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chef.Workspace.Basic
{
    public class Folder : Node
    {
        public Folder(string dir)
        {

        }

        public event Func<int> File
    }
}

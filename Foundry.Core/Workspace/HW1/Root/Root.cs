using Chef.Workspace.HW1.Root.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chef.Workspace.HW1.Root
{
    public class Root : Node
    {
        public string Directory { get; private set; }

        protected override Node _Child(string name)
        {
            switch(name)
            {
                case "objects":
                case "objects_update":
                    return new Objects.Objects();
            }

            return null;
        }

        protected override Stream _Seek(string name, Stream src)
        {
            switch(name)
            {
                case "objects":
                case "objects_update":
                    return File.Open(Directory + "/" + name + ".xml", FileMode.Open);

                case "scenarios":
                    break;
            }

            return null;
        }
    }
}

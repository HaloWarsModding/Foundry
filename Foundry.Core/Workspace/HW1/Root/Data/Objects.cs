using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Chef.Workspace.HW1.Root.Objects
{
    public class Objects : Node
    {
        protected override Node _Child(string name)
        {

        }
        protected override Stream _Seek(string name, Stream src)
        {
            using (XmlReader r = XmlReader.Create(src))
            {
                r.ReadToDescendant("Object");
                r.MoveToAttribute("name");

                while (r.Value != name)
                {
                    r.MoveToElement();
                    r.ReadToNextSibling("Object");
                    r.MoveToAttribute("name");
                }

                r.MoveToElement();

                return src;
            }
        }
    }
}

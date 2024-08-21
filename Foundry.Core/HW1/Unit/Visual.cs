using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chef.HW1.Unit
{
    public class VisualComponent
    {
        public string File { get; set; }
        public string DamageFile { get; set; }
    }
    public class VisualAnimation
    {

    }
    public class VisualModel
    {
        public string Name { get; set; }
        public VisualComponent Component { get; set; }
    }

    public class Visual
    {
        public VisualModel[] Models { get; set; }
    }
}

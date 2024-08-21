using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Chef.Util
{
    public struct AABB
    {
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }
    }
}

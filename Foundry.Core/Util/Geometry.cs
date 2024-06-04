using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Util
{
    public struct Vertex
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
    }
    public struct Triangle
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
    }
}

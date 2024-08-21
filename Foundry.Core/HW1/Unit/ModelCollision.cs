using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Chef.HW1.Unit
{
    public class ModelAABB
    {
        public ModelAABB() { }

        public Vector3 Position { get; set; }
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }
    }
    public class ModelCollisionItem
    {
        public Model Model { get; set; }
        public Vector3 Position { get; set; }
    }

    public static class ModelCollision
    {
        public static void CreateAABBTree(IEnumerable<ModelCollisionItem> items)
        {
            foreach(ModelCollisionItem item in items)
            {

            }
        }
    }
}

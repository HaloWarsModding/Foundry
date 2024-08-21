using Chef.HW1.Unit;
using Chef.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Chef.HW1.Map
{
    public static class ScenarioHelpers
    {
        public static int SelectAt(Scenario scn, Vector3 start, Vector3 direction, AssetCache assets)
        {
            Vector3 end = start + (direction * 10000);

            List<KeyValuePair<int, Vector3>> hits = new List<KeyValuePair<int, Vector3>>();

            foreach (var o in scn.Objects)
            {
                ProtoObject proto = AssetDatabase.GetOrLoadProtoObject(o.Unit, assets);
                if (proto == null) continue;

                string visName = AssetDatabase.ObjectVisualName(o.Unit, assets);
                if (visName == null) continue;

                Vector3 position = Misc.FromString(o.Position);
                Vector3 forward = Misc.FromString(o.Forward);
                Vector3 right = Misc.FromString(o.Right);
                Vector3 up = Vector3.Cross(forward, right);
                //we need a left handed matrix, but c# only offers this function in right-handedness, so we invert the forward vector.
                Matrix4x4 transform = Matrix4x4.CreateWorld(position, -forward, up);

                foreach (var modelName in AssetDatabase.VisualModelNames(visName, assets))
                {
                    Model model = AssetDatabase.GetOrLoadModel(modelName, assets);
                    if (model == null) continue;

                    Vector3 hit = Vector3.Zero;
                    if (CommonCollision.TestRayAABB(model.BoundsMin + position, model.BoundsMax + position, start, end, out hit))
                    {
                        foreach (var section in model.Sections)
                        {
                            if (CommonCollision.TestRayTriangles(transform, section.Vertices.Select(v => v.Position).ToArray(), section.Indices, start, end, out hit))
                            {
                                hits.Add(new KeyValuePair<int, Vector3>(o.ID, hit));
                            }
                        }
                    }
                }
            }

            int SelectedId = -1;
            float distance = 999999.0f;
            foreach (var (id, hit) in hits)
            {
                float d2 = Vector3.Distance(start, hit);
                if (d2 < distance)
                {
                    distance = d2;
                    SelectedId = id;
                }
            }

            return SelectedId;
        }
    }
}

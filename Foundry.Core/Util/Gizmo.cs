using Chef.HW1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Chef.Util
{
    [Flags()]
    public enum GizmoAxis
    {
        None = 0,
        X = 1 << 0,
        Y = 1 << 1, 
        Z = 1 << 2,
    }
    public static class Gizmo
    {
        public const float cMoveSize = 5.0f;
        public static bool TestRayTranslate(Vector3 start, Vector3 end, Vector3 position, Vector3 forward, Vector3 right, out GizmoAxis axis)
        {
            axis = GizmoAxis.None;
            Vector3 up = Vector3.Cross(forward, right);
            Vector3 hit;

            if (     CommonCollision.TestRayOBB(start, end, position + (forward * cMoveSize), forward, new Vector3(1, 1, cMoveSize), out hit))
            {
                axis = GizmoAxis.Z;
                return true;
            }
            else if (CommonCollision.TestRayOBB(start, end, position + (up      * cMoveSize), up,      new Vector3(1, 1, cMoveSize), out hit))
            {
                axis = GizmoAxis.Y;
                return true;
            }
            else if (CommonCollision.TestRayOBB(start, end, position + (right   * cMoveSize), right,   new Vector3(1, 1, cMoveSize), out hit))
            {
                axis = GizmoAxis.X;
                return true;
            }

            return false;
        }
        public static Vector3 RayAxisClosestPoint(Vector3 start, Vector3 end, Vector3 position, Vector3 forward, Vector3 right, GizmoAxis axis)
        {
            Vector3 up = Vector3.Cross(forward, right);

            Vector3 view_forward = Vector3.Normalize(end - start);
            Vector3 view_right = Vector3.Cross(view_forward, Vector3.UnitY);

            if (axis == GizmoAxis.X)
            {
                return CommonCollision.ClosestPointRayLine(start, end, position, right);
            }
            if (axis == GizmoAxis.Y)
            {
                return CommonCollision.ClosestPointRayLine(start, end, position, up);
            }
            if (axis == GizmoAxis.Z)
            {
                return CommonCollision.ClosestPointRayLine(start, end, position, forward);
            }

            return Vector3.Zero;
        }
    }
}

using Chef.Util;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Chef.Win.Render
{
    public static class GizmoRenderer
    {
        public static void DrawTransformGizmo(RenderTargetView target, DepthStencilView depth, Camera camera, Vector3 position, GizmoAxis highlight)
        {
            Matrix4x4 gizmoMat = Matrix4x4.CreateWorld(position, -Vector3.UnitZ, Vector3.UnitY);
            if (highlight == GizmoAxis.X)
                CommonRenderer.DrawLine(target, depth, camera, Vector3.Zero, Vector3.UnitX * 7, Color.Red, gizmoMat);
            else
                CommonRenderer.DrawLine(target, depth, camera, Vector3.Zero, Vector3.UnitX * 7, Color.DarkRed, gizmoMat);

            if (highlight == GizmoAxis.Y)
                CommonRenderer.DrawLine(target, depth, camera, Vector3.Zero, Vector3.UnitY * 7, Color.Green, gizmoMat);
            else
                CommonRenderer.DrawLine(target, depth, camera, Vector3.Zero, Vector3.UnitY * 7, Color.DarkGreen, gizmoMat);


            if (highlight == GizmoAxis.Z)
                CommonRenderer.DrawLine(target, depth, camera, Vector3.Zero, Vector3.UnitZ * 7, Color.Blue, gizmoMat);
            else
                CommonRenderer.DrawLine(target, depth, camera, Vector3.Zero, Vector3.UnitZ * 7, Color.DarkBlue, gizmoMat);

        }
    }
}

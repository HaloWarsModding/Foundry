using BrightIdeasSoftware;
using Chef.Win.Render;
using Chef.HW1.Unit;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Buffer = SharpDX.Direct3D11.Buffer;
using Chef.HW1;
using Chef.Util;
using Chef.HW1.Map;
using DirectXTexNet;
using Image = DirectXTexNet.Image;
using Aga.Controls;
using Device2 = SharpDX.Direct3D11.Device2;
using System.Drawing;

namespace Chef.Win.UI
{
    public enum TransformMode
    {
        Global,
        Local,
    }

    public class ScenarioWindow : DockContent
    {
        public string ScenarioName { get; set; }

        AssetCache Assets { get; set; }
        GpuCache GpuAssets { get; set; }

        private D3DViewport Viewport { get; set; }
        private Camera Camera { get; set; }

        private Point MouseLast { get; set; }
        private int SelectedId { get; set; } = -1;

        private TransformMode TransformMode { get; set; } = TransformMode.Global;
        private GizmoAxis TransformAxis { get; set; }

        public ScenarioWindow(AssetCache assets, GpuCache gassets)
        {
            Assets = assets;
            GpuAssets = gassets;

            Camera = new Camera();

            Viewport = new D3DViewport();
            Viewport.Dock = DockStyle.Fill;
            Controls.Add(Viewport);

            Viewport.Paint += OnPaint;
            Viewport.Resize += OnResize;
            Viewport.MouseMove += OnMouseMove;
            Viewport.MouseWheel += OnMouseScroll;
            Viewport.MouseClick += OnMouseClick;
        }

        private void OnMouseScroll(object sender, MouseEventArgs e)
        {
            float multiplier = 1 / 15.0f; //regular
            if (ModifierKeys == Keys.Control) //slow
                multiplier = 1 / 25.0f;
            if (ModifierKeys == Keys.Shift) //fast
                multiplier = 1 / 5.0f;

            Camera.Move(0, 0, -e.Delta * multiplier, 0, 0); //we want scrolling down (negative delta) to add distance, so flip the delta.
            Viewport.Invalidate();
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point mouseCur = e.Location;
            float delX = mouseCur.X - MouseLast.X;
            float delY = -(mouseCur.Y - MouseLast.Y); //in windows up == -y, while in d3d up == +y, so flip the delta y.
            if (e.Button == MouseButtons.Middle)
            {
                if (ModifierKeys == Keys.Shift)
                {
                    //pan
                    Camera.Move(0, 0, 0, delX * Camera.Distance / 150.0f, delY * Camera.Distance / 150.0f);
                }
                else
                {
                    //rotate
                    Camera.Move(delX / 2, delY / 2, 0, 0, 0);
                }
            }

            var scn = AssetDatabase.GetOrLoadScenario(ScenarioName, Assets);
            var where = scn.Objects.Where(o => o.ID == SelectedId && o.ID != -1);

            Vector3 ray_start = Camera.ScreenPointToWorldPos(e.Location.X, e.Location.Y);
            Vector3 ray_dir = Camera.ScreenPointToWorldDir(e.Location.X, e.Location.Y);
            Vector3 ray_end = ray_start + (ray_dir * 10000);

            if (where.Any())
            {
                Vector3 objPos = Misc.FromString(where.First().Position);
                Vector3 objForward = Misc.FromString(where.First().Forward);
                Vector3 objRight = Misc.FromString(where.First().Right);
                Vector3 objUp = Vector3.Cross(objForward, objRight);

                Vector3 gizmoForward;
                Vector3 gizmoRight;

                if (TransformMode == TransformMode.Global)
                {
                    gizmoForward = Vector3.UnitZ;
                    gizmoRight = Vector3.UnitX;
                }
                else
                {
                    gizmoForward = objForward;
                    gizmoRight = objRight;
                }


                if (e.Button == MouseButtons.Left && TransformAxis != GizmoAxis.None)
                {
                    Vector3 last_ray_start = Camera.ScreenPointToWorldPos(MouseLast.X, MouseLast.Y);
                    Vector3 last_ray_dir = Camera.ScreenPointToWorldDir(MouseLast.X, MouseLast.Y);
                    Vector3 last_ray_end = last_ray_start + (last_ray_dir * 10000);

                    Vector3 cur_axis_point = Gizmo.RayAxisClosestPoint(ray_start, ray_end, objPos, gizmoForward, gizmoRight, TransformAxis);
                    Vector3 last_axis_point = Gizmo.RayAxisClosestPoint(last_ray_start, last_ray_end, objPos, gizmoForward, gizmoRight, TransformAxis);
                    Vector3 axis_point_delta = cur_axis_point - last_axis_point;

                    where.First().Position = Misc.ToString(objPos + axis_point_delta);
                }
                else
                {
                    GizmoAxis axis;
                    if (Gizmo.TestRayTranslate(ray_start, ray_end, objPos, gizmoForward, gizmoRight, out axis))
                    {
                        TransformAxis = axis;
                    }
                    else
                    {
                        TransformAxis = GizmoAxis.None;
                    }
                }
            }

            MouseLast = mouseCur;
            Viewport.Invalidate();
        }
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            Vector3 start = Camera.ScreenPointToWorldPos(e.Location.X, e.Location.Y);
            Vector3 dir = Camera.ScreenPointToWorldDir(e.Location.X, e.Location.Y);

            var scn = AssetDatabase.GetOrLoadScenario(ScenarioName, Assets);
            int sel = ScenarioHelpers.SelectAt(scn, start, dir, Assets);
            SelectedId = sel;
        }
        private void OnPaint(object sender, PaintEventArgs e)
        {
            Viewport.Clear(Color.Tan);

            if (ScenarioName == null) return;

            var scn = AssetDatabase.GetOrLoadScenario(ScenarioName, Assets);
            ScenarioRenderer.DrawObjects(Viewport.Target, Viewport.Depth, Camera, scn, Assets, GpuAssets);

            //draw this LAST because of transparency.
            var terrain = AssetDatabase.ScenarioTerrainVisual(ScenarioName, Assets);
            var terrainGpu = GpuDatabase.GetOrUploadTerrainVisual(terrain, GpuAssets);
            TerrainRenderer.DrawVisualMesh(
                Viewport.Target, Viewport.Depth,
                terrainGpu,
                Camera,
                Atlas);

            if (SelectedId != -1)
            {
                var where = scn.Objects.Where(o => o.ID == SelectedId);
                if (where.Any())
                {
                    Vector3 objPos = Misc.FromString(where.First().Position);
                    Vector3 objForward = Misc.FromString(where.First().Forward);
                    Vector3 objRight = Misc.FromString(where.First().Right);
                    Vector3 objUp = Vector3.Cross(objForward, objRight);

                    Vector3 gizmoForward;
                    Vector3 gizmoRight;
                    Vector3 gizmoUp;

                    if (TransformMode == TransformMode.Global)
                    {
                        gizmoForward = Vector3.UnitZ;
                        gizmoRight = Vector3.UnitX;
                        gizmoUp = Vector3.UnitY;
                    }
                    else
                    {
                        gizmoForward = objForward;
                        gizmoRight = objRight;
                        gizmoUp = objUp;
                    }

                    CommonRenderer.DrawLine(Viewport.Target, null, Camera, objPos, objPos + (gizmoForward * 10), TransformAxis == GizmoAxis.Z ? Color.Blue : Color.Black, Matrix4x4.Identity);
                    CommonRenderer.DrawLine(Viewport.Target, null, Camera, objPos, objPos + (gizmoUp * 10),      TransformAxis == GizmoAxis.Y ? Color.Green : Color.Black, Matrix4x4.Identity);
                    CommonRenderer.DrawLine(Viewport.Target, null, Camera, objPos, objPos + (gizmoRight * 10),   TransformAxis == GizmoAxis.X ? Color.Red : Color.Black, Matrix4x4.Identity);
                }
            }

            Viewport.Present();
        }
        private void OnResize(object o, EventArgs e)
        {
            Camera.Width = Width;
            Camera.Height = Height;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        //TODO: This should be cached!!
        private ShaderResourceView Atlas { get; set; }
        //TODO: This should be cached!!
        public void RefreshAssets()
        {
            //TODO: This should be cached!!
            string[] atlasNames = AssetDatabase.TerrainTextures(ScenarioName, Assets).ToArray();
            Image[] atlasTextures = atlasNames.Select(t => AssetDatabase.GetOrLoadTexture(t, Assets)).ToArray();
            Atlas = new ShaderResourceView(D3DViewport.Device, CommonRenderer.UploadTerrainTextures(atlasTextures, false, false));
        }
    }
}
using BrightIdeasSoftware;
using Foundry.HW1.Scenario;
using Foundry.HW1.Serialization;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Foundry.UI.WinForms
{
    public class ScenarioWindow : DockContent
    {
        public TerrainVisual Visual { get; set; }
        public TerrainVisualMesh VisualMesh { get; set; }
        public TerrainVisualAABB[] VisualAABBs { get; set; }

        private D3DViewport Viewport { get; set; }

        private Camera Camera { get; set; } = new Camera();

        private Point MouseLocationLast { get; set; }
        private Vector3 MouseLocation3D
        {
            get
            {
                float mnx = (PointToClient(MousePosition).X - (Width / 2F)) / Width;
                float mny = (PointToClient(MousePosition).Y - (Height / 2F)) / Height;
                return Camera.Pos + (Camera.Right * mnx) + (Camera.Up * -mny);
            }
        }

        public ScenarioWindow()
        {
            Viewport = new D3DViewport();
            Viewport.Dock = DockStyle.Fill;
            Controls.Add(Viewport);

            using (Stream s = File.OpenRead("D:\\SteamLibrary\\steamapps\\common\\HaloWarsDE\\Extract\\scenario\\skirmish\\design\\chasms\\chasms.xtd"))
            {
                Visual = Terrain.ReadXtd(s);
            }
            VisualMesh = TerrainRenderer.UploadVisualMesh(Visual);
            VisualAABBs = TerrainCollision.CalcAABBs(Visual);

            Viewport.Paint += OnPaint;
            Viewport.Resize += OnResize;
            Viewport.MouseMove += OnMouseMove;
            Viewport.MouseWheel += OnMouseScroll;
        }

        private void OnMouseScroll(object sender, MouseEventArgs e)
        {
            float multiplier = 1 / 15.0f; //regular
            if (ModifierKeys == Keys.Control) //slow
                multiplier = 1 / 25.0f;
            if (ModifierKeys == Keys.Shift) //fast
                multiplier = 1 / 5.0f;

            MoveCamera(0, 0, -e.Delta * multiplier, 0, 0); //scrolling down (negative delta) adds distance, so flip the delta so that -delta == +distance.
            Viewport.Invalidate();
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point mouseCur = e.Location;
            if (e.Button == MouseButtons.Middle)
            {
                float delX = mouseCur.X - MouseLocationLast.X;
                float delY = mouseCur.Y - MouseLocationLast.Y;
                if (ModifierKeys == Keys.Shift)
                {
                    //pan
                    MoveCamera(0, 0, 0, delX * Camera.Distance / 150.0f, -delY * Camera.Distance / 150.0f); //in windows up == -y, while in d3d up == +y, so flip the delta y.
                }
                else
                {
                    //rotate
                    MoveCamera(delX / 2, -delY / 2, 0, 0, 0); //in windows up == -y, while in d3d up == +y, so flip the delta y.
                }
            }
            MouseLocationLast = mouseCur;
            Viewport.Invalidate();
        }
        private void OnPaint(object sender, PaintEventArgs e)
        {
            var first = TerrainCollision.FirstRayCollision(VisualAABBs, MouseLocation3D, MouseLocation3D + (Camera.Forward * 100000f));
            Vector3 hitpos = new Vector3(0, 0, 0);
            if (first != null)
            {
                var inds = TerrainCollision.CollidingIndices(first, Visual, MouseLocation3D, MouseLocation3D + (Camera.Forward * 100000f));
                if (inds.Any())
                {
                    hitpos = Visual.Positions[inds[0]];
                }
            }

            Viewport.Clear(Color.Tan);
            TerrainRenderer.DrawVisualMesh(
                Viewport.Target, Viewport.Depth,
                VisualMesh,
                Camera,
                hitpos);
            Viewport.Present();
        }
        private void OnResize(object o, EventArgs e)
        {
            Viewport.ResizeBackBuffer(Width, Height);
            Camera.Width = Width;
            Camera.Height = Height;
        }

        private void MoveCamera(float rotDegY, float rotDegZ, float distance, float panScreenX, float panScreenY)
        {
            Camera.Yaw += rotDegY;
            while (Camera.Yaw > 360) Camera.Yaw -= 360;

            Camera.Pitch += rotDegZ;
            Camera.Pitch = Math.Clamp(Camera.Pitch, -89.5f, 89.5f);

            Camera.Target += Camera.Right * panScreenX;
            Camera.Target += Camera.Up * panScreenY;

            float minHeight = -500;
            float maxHeight = 500;
            float minWidth = -Visual.Width * .5f;
            float maxWidth = Visual.Width * 1.5f;
            Camera.Target = Vector3.Clamp(
                Camera.Target,
                new Vector3(minWidth, minHeight, minWidth),
                new Vector3(maxWidth, maxHeight, maxWidth));

            float maxDistance = Visual.Width * 3;
            Camera.Distance += distance;
            Camera.Distance = Math.Clamp(Camera.Distance, 1, maxDistance);
        }
    }
}
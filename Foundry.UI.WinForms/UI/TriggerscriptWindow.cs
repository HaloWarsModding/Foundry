using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using static Chef.HW1.Script.TriggerscriptParams;
using static Chef.HW1.Script.TriggerscriptHelpers;
using static Chef.Win.UI.TriggerscriptMenus;
using static Chef.Win.Render.TriggerscriptRenderer;
using WeifenLuo.WinFormsUI.Docking;
using Chef.HW1.Script;
using Chef.HW1;
using Chef.Win.Render;
using System.Diagnostics.Eventing.Reader;

namespace Chef.Win.UI
{
    internal static class TriggerscriptEditorStatics
    {
        public static Matrix Inverted(this Matrix m)
        {
            Matrix ret = m.Clone();
            ret.Invert();
            return ret;
        }
        public static Point TransformPoint(this Matrix m, Point point)
        {
            Point[] p = new Point[1] { point };
            m.TransformPoints(p);
            return p[0];
        }
        public static PointF TransformPoint(this Matrix m, PointF point)
        {
            PointF[] p = new PointF[1] { point };
            m.TransformPoints(p);
            return p[0];
        }

    }

    public class TriggerscriptWindow : DockContent
    {
        public EventHandler RefChanged;
        
        public string ScriptName { get; set; }
        private AssetCache Assets { get; set; }
        private GpuCache GpuAssets { get; set; }
        private Triggerscript Triggerscript { get; set; }

        private PointF ViewPos { get; set; } = new PointF(0, 0);
        private float ViewScale { get; set; } = 1.0f;
        private float ViewScaleLOD = 0.75f;
        private Matrix ViewMatrix
        {
            get
            {
                Matrix matrix = new Matrix();
                matrix.Reset();
                matrix.Translate(Width / 2, Height / 2);
                matrix.Scale(ViewScale, ViewScale);
                matrix.Translate(ViewPos.X, ViewPos.Y);
                return matrix;
            }
        }

        private Point MouseLast { get; set; }

        private int selTrigger, selLogic, selVar;
        private LogicSlot selSlot;
        private int dropTrigger, dropLogic;
        private LogicSlot dropSlot;

        public TriggerscriptWindow(AssetCache assets, GpuCache gassets)
        {
            Assets = assets;
            GpuAssets = gassets;

            DoubleBuffered = true;
            Paint += OnPaint;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseMove += OnMouseMove;
            MouseWheel += OnMouseScroll;
            KeyDown += OnKeyDown;

            RefChanged += (s, e) =>
            {
                Rectangle bounds = BoundsScript(Triggerscript);
                ViewPos = new PointF(
                    bounds.X + bounds.Width / 2,
                    bounds.Y + bounds.Height / 2
                );
                ViewScale = 1.0f;
                Invalidate();
            };
        }

        private void OnMouseDown(object o, MouseEventArgs e)
        {
            Triggerscript = AssetDatabase.GetOrLoadTriggerscript(ScriptName, Assets);
            if (Triggerscript == null) return;

            Point ViewMouse = ViewMatrix.Inverted().TransformPoint(e.Location);

            SelectBoundsBody(Triggerscript, ViewMouse, out selTrigger, out selSlot, out selLogic);
            if (ViewScale > ViewScaleLOD)
            {
                SelectBoundsInsert(Triggerscript, ViewMouse, out dropTrigger, out dropSlot, out dropLogic);
                SelectBoundsParamValue(Triggerscript, ViewMouse, out selTrigger, out selSlot, out selLogic, out selVar);
            }
            else
            {
                selLogic = -1;
                dropLogic = -1;
                selVar = -1;
            }

            //what to do when a menu edits something in the script:
            EventHandler onEdit = (s, e) =>
            {
                AssetDatabase.TriggerscriptMarkEdited(ScriptName, Assets, true);
                Invalidate();
            };

            if (e.Button == MouseButtons.Right && selTrigger != -1)
            {
                Trigger t = Triggerscript.Triggers[selTrigger];
                if (selVar != -1)
                {
                    ShowSetVarMenu(Triggerscript, Logics(t, selSlot).ElementAt(selLogic), selVar, PointToScreen(e.Location), onEdit);
                }
                else if (dropLogic != -1)
                {
                        ShowLogicAddMenu(Triggerscript, dropTrigger, dropSlot, dropLogic, PointToScreen(e.Location), onEdit);
                }
                else if (selLogic != -1)
                {
                    if (selSlot == LogicSlot.Condition)
                        ShowConditionOptionsMenu((Condition)Logics(t, selSlot).ElementAt(selLogic), PointToScreen(e.Location), onEdit);
                    else
                        ShowEffectOptionsMenu((Effect)Logics(t, selSlot).ElementAt(selLogic), PointToScreen(e.Location), onEdit);
                }
                else
                {
                    ShowTriggerOptionsMenu(t, e.Location, onEdit);
                }
            }
            else if (e.Button == MouseButtons.Right && dropTrigger != -1 && dropLogic != -1)
            {
                ShowLogicAddMenu(Triggerscript, dropTrigger, dropSlot, dropLogic, PointToScreen(e.Location), onEdit);
            }
            else if (e.Button == MouseButtons.Right)
            {
                //ShowVarList(Triggerscript, PointToScreen(e.Location), onEdit);
            }

            MouseLast = e.Location;
            Invalidate();
        }
        private void OnMouseUp(object o, MouseEventArgs e)
        {
            Triggerscript = AssetDatabase.GetOrLoadTriggerscript(ScriptName, Assets);
            if (Triggerscript == null) return;

            //Point ViewMouse = ViewMatrix.Inverted().TransformPoint(e.Location);
            //BodyBoundsAtPoint(Triggerscript, ViewMouse, out selTrigger, out selSlot, out selLogic);
            
            //MouseLast = e.Location;
            //Invalidate();
        }
        private void OnMouseMove(object o, MouseEventArgs e)
        {
            Triggerscript = AssetDatabase.GetOrLoadTriggerscript(ScriptName, Assets);
            if (Triggerscript == null) return;

            Point ViewMouse = ViewMatrix.Inverted().TransformPoint(e.Location);
            SelectBoundsInsert(Triggerscript, ViewMouse, out dropTrigger, out dropSlot, out dropLogic);

            if (MouseButtons == MouseButtons.Middle)
            {
                ViewPos += new SizeF(
                    (e.Location.X - MouseLast.X) * (1 / ViewScale),
                    (e.Location.Y - MouseLast.Y) * (1 / ViewScale)
                    );
            }

            if ((MouseButtons & MouseButtons.Left) > 0)
            {
                if (selTrigger != -1 && selLogic == -1)
                {
                    Trigger selected = Triggerscript.Triggers[selTrigger];
                    if (selected != null)
                    {
                        selected.X += (e.Location.X - MouseLast.X) * (1 / ViewScale);
                        selected.Y += (e.Location.Y - MouseLast.Y) * (1 / ViewScale);
                        AssetDatabase.TriggerscriptMarkEdited(ScriptName, Assets, true);
                    }
                }
                if (selTrigger != -1 && selLogic != -1 && dropLogic != -1 && CanTransfer(selSlot, dropSlot))
                {
                    Trigger from = Triggerscript.Triggers[selTrigger];
                    Trigger to = Triggerscript.Triggers[dropTrigger];

                    if (from == to && selSlot == dropSlot)
                    {
                        if (dropLogic > selLogic)
                        {
                            dropLogic--;
                        }
                    }

                    TransferLogic(from, selSlot, selLogic, to, dropSlot, dropLogic);
                    selTrigger = dropTrigger;
                    selSlot = dropSlot;
                    selLogic = dropLogic;
                    AssetDatabase.TriggerscriptMarkEdited(ScriptName, Assets, true);
                }
            }

            MouseLast = e.Location;
            Invalidate();
        }
        private void OnMouseScroll(object o, MouseEventArgs e)
        {
            Triggerscript = AssetDatabase.GetOrLoadTriggerscript(ScriptName, Assets);
            if (Triggerscript == null) return;

            Point ViewMousePre = ViewMatrix.Inverted().TransformPoint(e.Location);
            ViewScale += e.Delta * (ViewScale / 1000);
            Point ViewMouse = ViewMatrix.Inverted().TransformPoint(e.Location);

            if ((MouseButtons & MouseButtons.Left) > 0
                && selTrigger != -1
                && selLogic == -1)
            {
                Trigger selected = Triggerscript.Triggers[selTrigger];
                SizeF offset = new SizeF(
                    selected.X - ViewMousePre.X,
                    selected.Y - ViewMousePre.Y
                    );
                if (selected != null)
                {
                    selected.X = ViewMouse.X + offset.Width; //(offset.Width * (1 / ViewZoom));
                    selected.Y = ViewMouse.Y + offset.Height; //(offset.Height * (1 / ViewZoom));
                    AssetDatabase.TriggerscriptMarkEdited(ScriptName, Assets, true);
                }
            }

            ClampView();
            Invalidate();
        }
        private void OnKeyDown(object o, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                AssetDatabase.SaveTriggerscript(ScriptName, Assets);
                AssetDatabase.TriggerscriptMarkEdited(ScriptName, Assets, false);
            }
            Invalidate();
        }
        private void OnPaint(object o, PaintEventArgs e)
        {
            Text = ScriptName;
            Triggerscript = AssetDatabase.GetOrLoadTriggerscript(ScriptName, Assets);
            if (Triggerscript == null) return;
            if (AssetDatabase.TriggerscriptIsEdited(ScriptName, Assets)) Text += "*";

            ClampView();

            //e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            //e.Graphics.InterpolationMode = InterpolationMode.High;
            //e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            e.Graphics.Clear(BackgroundColor);

            Rectangle viewClip = new Rectangle(
                ViewMatrix.Inverted().TransformPoint(e.ClipRectangle.Location).X,
                ViewMatrix.Inverted().TransformPoint(e.ClipRectangle.Location).Y,
                (int)(e.ClipRectangle.Size.Width * (1 / ViewScale)),
                (int)(e.ClipRectangle.Size.Height * (1 / ViewScale))
                );
            e.Graphics.Transform = ViewMatrix;

            bool lod = false;
            if (ViewScale > ViewScaleLOD)
            {
                lod = true;
            }

            DrawScript(e.Graphics, viewClip, Triggerscript,
                new Selection() { TriggerId = selTrigger, LogicType = selSlot, LogicIndex = selLogic },
                new Selection() { TriggerId = dropTrigger, LogicType = dropSlot, LogicIndex = dropLogic, InsertIndex = dropLogic },
                lod);
        }

        //TODO:
        private void ClampView()
        {
            Triggerscript = AssetDatabase.GetOrLoadTriggerscript(ScriptName, Assets);
            if (Triggerscript == null) return;

            ViewScale = Math.Clamp(ViewScale, ScaleViewMin, ScaleViewMax);

            Rectangle bounds = BoundsScript(Triggerscript);
            bounds.Location = ViewMatrix.Inverted().TransformPoint(bounds.Location);
            bounds.Size = ((SizeF)bounds.Size * ViewScale).ToSize();

            float dx = 0, dy = 0;

            ViewPos = ViewPos + new SizeF(dx, dy);
        }
    }
}
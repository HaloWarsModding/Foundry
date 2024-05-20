using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using Newtonsoft.Json;
using Foundry;
using System.Numerics;
using Foundry.Util;
using Foundry.util;
using System.Text.RegularExpressions;
using YAXLib;
using YAXLib.Enums;
using YAXLib.Options;
using System.Reflection;
using static Foundry.HW1.Triggerscript.EditorParams;
using static Foundry.HW1.Triggerscript.EditorHelpers;
using static Foundry.HW1.Triggerscript.EditorMenusWinForms;
using static Foundry.HW1.Triggerscript.EditorRendererWinForms;
using WeifenLuo.WinFormsUI.Docking;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Foundry.HW1.Triggerscript
{
    public class Editor : DockContent
    {
        public EventHandler<Triggerscript> FileChanged;

        public Triggerscript TriggerscriptFile
        {
            get { return _TriggerscriptFile; }
            set { _TriggerscriptFile = value; FileChanged?.Invoke(this, value); }
        }
        private Triggerscript _TriggerscriptFile;

        private PointF ViewPos { get; set; } = new PointF(0, 0);
        private float ViewScale { get; set; } = 1.0f;
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

        public Selection Selection { get; private set; }
        public Selection Hover { get; private set; }

        public Editor(FoundryInstance i)
        {
            DoubleBuffered = true;
            Paint += OnPaint;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseMove += OnMouseMove;
            MouseWheel += OnMouseScroll;

            KeyDown += (s, e) =>
            {
                if (e.KeyData == Keys.F4)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Triggerscript|*.triggerscript";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        TriggerscriptFile = new YAXSerializer<Triggerscript>(new SerializerOptions() { ExceptionBehavior = YAXExceptionTypes.Ignore })
                            .DeserializeFromFile(ofd.FileName);
                    };
                }

                if (e.KeyData == Keys.F5)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Triggerscript|*.triggerscript";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        new YAXSerializer<Triggerscript>(new SerializerOptions() { ExceptionBehavior = YAXExceptionTypes.Ignore })
                            .SerializeToFile(TriggerscriptFile, sfd.FileName);
                    };
                }
            };

            FileChanged += (s, e) =>
            {
                Selection = new Selection();
                Hover = new Selection();

                Rectangle bounds = ScriptBounds(e);
                ViewPos = new PointF(
                    bounds.X + (bounds.Width / 2),
                    bounds.Y + (bounds.Height / 2)
                );
                ViewScale = 1.0f;
                Invalidate();
            };
        }

        private void OnMouseDown(object o, MouseEventArgs e)
        {
            if (TriggerscriptFile == null) return;

            Point ViewMouse = ViewMatrix.Inverted().TransformPoint(e.Location);
            Selection = SelectAt(TriggerscriptFile, ViewMouse);

            if (e.Button == MouseButtons.Right)
            {
                ShowOptionsForSelection(TriggerscriptFile, Selection, PointToScreen(e.Location));
            }

            MouseLast = e.Location;
            Invalidate();
        }
        private void OnMouseUp(object o, MouseEventArgs e)
        {
            if (TriggerscriptFile == null) return;

            Point ViewMouse = ViewMatrix.Inverted().TransformPoint(e.Location);
            Selection = SelectAt(TriggerscriptFile, ViewMouse);

        }
        private void OnMouseMove(object o, MouseEventArgs e)
        {
            if (TriggerscriptFile == null) return;
            Point ViewMouse = ViewMatrix.Inverted().TransformPoint(e.Location);

            if (MouseButtons == MouseButtons.Middle)
            {
                ViewPos += new SizeF(
                    (e.Location.X - MouseLast.X) * (1 / ViewScale),
                    (e.Location.Y - MouseLast.Y) * (1 / ViewScale)
                    );
            }

            if ((MouseButtons & MouseButtons.Left) > 0)
            {
                if (Selection.TriggerId != -1 && Selection.LogicIndex == -1)
                {
                    Trigger selected = TriggerscriptFile.Triggers[Selection.TriggerId];
                    if (selected != null)
                    {
                        selected.X += (e.Location.X - MouseLast.X) * (1 / ViewScale);
                        selected.Y += (e.Location.Y - MouseLast.Y) * (1 / ViewScale);
                    }
                }
                if (Selection.TriggerId != -1 && Selection.LogicIndex != -1)
                {
                    TSDragDrop(ViewMouse);
                }
            }

            MouseLast = e.Location;

            ClampView();
            Invalidate();
        }
        private void OnMouseScroll(object o, MouseEventArgs e)
        {
            if (TriggerscriptFile == null) return;

            Point ViewMousePre = ViewMatrix.Inverted().TransformPoint(e.Location);
            ViewScale += e.Delta * (ViewScale / 1000);
            Point ViewMouse = ViewMatrix.Inverted().TransformPoint(e.Location);

            if ((MouseButtons & MouseButtons.Left) > 0
                && Selection.TriggerId != -1
                && Selection.LogicIndex == -1)
            {
                Trigger selected = TriggerscriptFile.Triggers[Selection.TriggerId];
                SizeF offset = new SizeF(
                    selected.X - ViewMousePre.X,
                    selected.Y - ViewMousePre.Y
                    );
                if (selected != null)
                {
                    selected.X = ViewMouse.X + offset.Width; //(offset.Width * (1 / ViewZoom));
                    selected.Y = ViewMouse.Y + offset.Height; //(offset.Height * (1 / ViewZoom));
                }
            }

            ClampView();
            Invalidate();
        }
        private void OnPaint(object o, PaintEventArgs e)
        {
            if (TriggerscriptFile == null) return;

            Rectangle viewClip = new Rectangle(
                ViewMatrix.Inverted().TransformPoint(e.ClipRectangle.Location).X,
                ViewMatrix.Inverted().TransformPoint(e.ClipRectangle.Location).Y,
                (int)(e.ClipRectangle.Size.Width * (1 / ViewScale)),
                (int)(e.ClipRectangle.Size.Height * (1 / ViewScale))
                );
            e.Graphics.Transform = ViewMatrix;

            bool detail, lod;
            if (ViewScale > .75)
            {
                //regular
                detail = false;
                lod = false;
                if (ViewScale > 2.5) detail = true;
            }
            else //ViewScale <= .75
            {
                //lod
                detail = true;
                lod = true;
                if (ViewScale < .2) detail = false;
            }
            DrawScript(e.Graphics, viewClip, TriggerscriptFile, Selection, Selection, detail, lod);
        }

        private void ClampView()
        {
            if (TriggerscriptFile == null) return;

            ViewScale = Math.Clamp(ViewScale, ScaleViewMin, ScaleViewMax);

            Rectangle bounds = ScriptBounds(TriggerscriptFile);
            // min == bottom right corner
            // max == top left corner
            ViewPos = new PointF(
               (int)Math.Clamp(
                    ViewPos.X,
                    bounds.X - bounds.Width,
                    bounds.X
                    ),
               (int)Math.Clamp(
                   ViewPos.Y,
                   bounds.Y - bounds.Height,
                   bounds.Y
                   )
                );
        }
        private void TSDragDrop(Point ViewMouse)
        {
            Hover = SelectAt(TriggerscriptFile, ViewMouse);
            if (Hover != Selection)
            {
                if (Hover.LogicIndex != -1 && CanTransfer(Selection.LogicType, Hover.LogicType))
                {
                    Trigger from = TriggerscriptFile.Triggers[Selection.TriggerId];
                    Trigger to = TriggerscriptFile.Triggers[Hover.TriggerId];

                    if (Logics(to, Hover.LogicType).Count() > 0)
                    {
                        Rectangle toBounds = LogicBounds(to, Hover.LogicType, Hover.LogicIndex);
                        int toIndex = Hover.LogicIndex;

                        if (Hover != Selection)
                        {
                            if (Hover.LogicIndex == Selection.LogicIndex + 1)
                            {
                                if (ViewMouse.X <= toBounds.X + (toBounds.Width / 2)) return;
                            }
                            if (Hover.LogicIndex == Selection.LogicIndex - 1)
                            {
                                if (ViewMouse.X > toBounds.X + (toBounds.Width / 2)) return;
                            }
                        }
                        if (from != to || Hover.LogicType != Selection.LogicType)
                        {
                            if (ViewMouse.X > toBounds.X + (toBounds.Width / 2)) toIndex += 1;
                        }

                        if (TransferLogic(from, Selection.LogicType, Selection.LogicIndex, to, Hover.LogicType, toIndex))
                        {
                            Selection = new Selection()
                            {
                                LogicIndex = toIndex,
                                LogicType = Hover.LogicType,
                                TriggerId = Hover.TriggerId
                            };
                        }
                    }
                    else
                    {
                        Rectangle toBounds = LogicBounds(to, Hover.LogicType);
                        int toIndex = 0;
                        if (!toBounds.Contains(ViewMouse)) return;

                        if (TransferLogic(from, Selection.LogicType, Selection.LogicIndex, to, Hover.LogicType, toIndex))
                        {
                            Selection = new Selection()
                            {
                                LogicIndex = toIndex,
                                LogicType = Hover.LogicType,
                                TriggerId = Hover.TriggerId
                            };
                        }
                    }
                }
            }
        }
    }
}
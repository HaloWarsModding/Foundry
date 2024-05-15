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

namespace Foundry.HW1.Triggerscript
{
    public class Editor : DockContent
    {
        public Triggerscript TriggerscriptFile { get; set; }

        private PointF ViewPos { get; set; } = new PointF(0, 0);
        private float ViewZoom { get; set; } = 1.0f;
        private Matrix ViewMatrix
        {
            get
            {
                Matrix matrix = new Matrix();
                matrix.Reset();
                matrix.Translate(Width / 2, Height / 2);
                matrix.Scale(ViewZoom, ViewZoom);
                matrix.Translate(ViewPos.X, ViewPos.Y);
                return matrix;
            }
        }

        private Point MouseLast { get; set; }

        public Selection Selection { get; private set; }
        public Selection Hover { get; private set; }


        public Editor(FoundryInstance i)
        {
            ////debug save
            //ViewTick += (sender, e) =>
            //{
            //    //if (e.KeyData == Keys.F5)
            //    if (GetKeyIsDown(Keys.F5) && !GetKeyWasDown(Keys.F5))
            //    {
            //        SaveFileDialog sfd = new SaveFileDialog();
            //        sfd.Filter = "Script(*.triggerscript)|*.triggerscript";
            //        if (sfd.ShowDialog(Instance) == DialogResult.OK)
            //        {
            //            new YAXSerializer<Triggerscript>().SerializeToFile(TriggerscriptFile, sfd.FileName);
            //        }
            //    }
            //};
            DoubleBuffered = true;
            Paint += OnPaint;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseMove += OnMouseMove;
            MouseWheel += OnMouseScroll;
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
                    (e.Location.X - MouseLast.X) * (1 / ViewZoom),
                    (e.Location.Y - MouseLast.Y) * (1 / ViewZoom)
                    );
            }

            if ((MouseButtons & MouseButtons.Left) > 0)
                {
                if (Selection.TriggerId != -1 && Selection.LogicIndex == -1)
                {
                    Trigger selected = TriggerscriptFile.Triggers[Selection.TriggerId];
                    if (selected != null)
                    {
                        selected.X += (e.Location.X - MouseLast.X) * (1 / ViewZoom);
                        selected.Y += (e.Location.Y - MouseLast.Y) * (1 / ViewZoom);
                    }
                }
                if (Selection.TriggerId != -1 && Selection.LogicIndex != -1)
                {
                    TSDragDrop(ViewMouse);
                }
            }

            MouseLast = e.Location;
            Invalidate();
        }
        private void OnMouseScroll(object o, MouseEventArgs e)
        {
            if (TriggerscriptFile == null) return;
            
            Point ViewMousePre = ViewMatrix.Inverted().TransformPoint(e.Location);
            ViewZoom += e.Delta * (ViewZoom / 1000);
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

            Invalidate();
        }
        private void OnPaint(object o, PaintEventArgs e)
        {
            if (TriggerscriptFile == null) return;
            e.Graphics.Transform = ViewMatrix;
            DrawScript(e.Graphics, e.ClipRectangle, TriggerscriptFile, Selection, Selection);
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
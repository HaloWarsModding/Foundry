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
using static Foundry.Triggerscript.ScriptModule;
using System.Text.RegularExpressions;
using YAXLib;
using YAXLib.Enums;
using YAXLib.Options;
using System.Reflection;
using static Foundry.Triggerscript.TriggerscriptEditorHelpers;
using static Foundry.Triggerscript.TriggerscriptEditorParameters;
using Microsoft.VisualBasic.Logging;
using Microsoft.VisualBasic.Devices;
using System.Diagnostics.Eventing.Reader;

namespace Foundry.Triggerscript
{
    public struct TriggerscriptSelection
    {
        public TriggerscriptSelection()
        {
            TriggerId = -1;
            LogicType = LogicType.Condition;
            LogicIndex = -1;
        }

        public int TriggerId { get; set; }
        public LogicType LogicType { get; set; }
        public int LogicIndex { get; set; }

        public static bool operator ==(TriggerscriptSelection lhs, TriggerscriptSelection rhs)
        {
            return (lhs.TriggerId == rhs.TriggerId
                && lhs.LogicType == rhs.LogicType
                && lhs.LogicIndex == rhs.LogicIndex);
        }
        public static bool operator !=(TriggerscriptSelection lhs, TriggerscriptSelection rhs)
        {
            return !(lhs == rhs);
        }
    }

    public static class TriggerscriptEditorParameters
    {
        public static int HeaderHeight { get; } = 10;

        public static int LogicSpacing { get; } = 3;
        public static int LogicSectionSpacing { get; } = 7;

        public static int VarNameHeight { get; } = 3;
        public static int VarValHeight { get; } = 3;
        public static int VarHeight { get { return VarValHeight + VarNameHeight; } }
        public static int VarSpacing { get; } = 3;

        public static int Margin { get; } = 1;

        public static Font TitleFont { get; } = new Font("Consolas", 2.5f, FontStyle.Regular);
        public static Font TextFont { get; } = new Font("Consolas", 2, FontStyle.Regular);

        public static Color TextColor { get; } = Color.White;
        public static Color BodyColor { get; } = Color.FromArgb(90, 90, 90);
        public static Color TrimColor { get; } = Color.FromArgb(70, 70, 70);
        public static Color TriggerHeaderColor { get; } = Color.CadetBlue;
        public static Color EffectHeaderColor { get; } = Color.RebeccaPurple;
        public static Color ConditionHeaderColor { get; } = Color.Crimson;
        public static Color ContainerColor { get; } = Color.Black;
    }

    public static class TriggerscriptEditorHelpers
    {
        static TriggerscriptEditorHelpers()
        {

        }

        public static int TriggerLogicCount(Trigger trigger, LogicType type)
        {
            if (type == LogicType.Condition) return trigger.Conditions.Count();
            if (type == LogicType.EffectTrue) return trigger.TriggerEffectsOnTrue.Count();
            if (type == LogicType.EffectFalse) return trigger.TriggerEffectsOnFalse.Count();
            return 0;
        }

        public static Rectangle TriggerBounds(Trigger trigger)
        {
            return new Rectangle(
                (int)trigger.X,
                (int)trigger.Y,
                55,
                50);
        }
        public static Rectangle UnitBounds(Trigger trigger)
        {
            Rectangle triggerBounds = TriggerBounds(trigger);
            Rectangle ret = triggerBounds;

            foreach (var type in Enum.GetValues<LogicType>())
            {
                for (int i = 0; i < TriggerLogicCount(trigger, type); i++)
                {
                    Rectangle logicBounds = LogicBoundsAt(trigger, type, i);
                    ret.Width = (logicBounds.X - triggerBounds.X) + logicBounds.Width;
                    ret.Height = Math.Max(ret.Height, logicBounds.Height);
                }
            }
            ret.Width += 50;

            return ret;
        }

        public static Size LogicSize(ILogic logic)
        {
            int varCount = 0;
            if (logic.Inputs != null) varCount += logic.Inputs.Count;
            if (logic.Outputs != null) varCount += logic.Outputs.Count;
            
            //SLOW!!!
            //int width = TextRenderer.MeasureText(logic.Type, TextFont).Width;
            //width = Math.Max(width, 40);
            int width = 50;

            return new Size(width, HeaderHeight + (varCount * (VarHeight + VarSpacing)) + VarHeight);
        }

        public static Rectangle LogicBoundsAll(Trigger trigger, LogicType type)
        {
            IEnumerable<ILogic> logics;
            Point loc;
            if (type == LogicType.Condition)
            {
                logics = trigger.Conditions;
                loc = TriggerBounds(trigger).Location;
                loc.X += TriggerBounds(trigger).Width;
                loc.X += LogicSectionSpacing;
            }
            else if (type == LogicType.EffectTrue)
            {
                logics = trigger.TriggerEffectsOnTrue;
                loc = LogicBoundsAll(trigger, LogicType.Condition).Location;
                foreach(var cnd in trigger.Conditions)
                {
                    loc.X += LogicSize(cnd).Width;
                    loc.X += LogicSpacing;
                }
                loc.X += LogicSectionSpacing;
            }
            else
            {
                logics = trigger.TriggerEffectsOnFalse;
                loc = LogicBoundsAll(trigger, LogicType.EffectTrue).Location;
                foreach (var eff in trigger.TriggerEffectsOnTrue)
                {
                    loc.X += LogicSize(eff).Width;
                    loc.X += LogicSpacing;
                }
                loc.X += LogicSectionSpacing;
            }

            Size size = new Size(LogicSectionSpacing, 25);
            foreach(var l in logics)
            {
                size.Height = Math.Max(size.Height, LogicSize(l).Height);
                size.Width += LogicSize(l).Width;
                size.Width += LogicSpacing;
            }

            return new Rectangle(loc, size);
        }
        public static Rectangle LogicBoundsAt(Trigger trigger, LogicType type, int index)
        {
            IEnumerable<ILogic> logics;
            if (type == LogicType.Condition)
                logics = trigger.Conditions;
            else if (type == LogicType.EffectTrue)
                logics = trigger.TriggerEffectsOnTrue;
            else if (type == LogicType.EffectFalse)
                logics = trigger.TriggerEffectsOnFalse;
            else
                return Rectangle.Empty;

            Point loc = LogicBoundsAll(trigger, type).Location;
            for (int i = 0; i < index; i++)
            {
                loc.X += LogicSize(logics.ElementAt(i)).Width;
                loc.X += LogicSpacing;
            }

            return new Rectangle(loc, LogicSize(logics.ElementAt(index)));
        }
        public static Rectangle LogicParamNameBoundsAt(Trigger trigger, LogicType type, int lindex, int pindex)
        {
            Rectangle logicBounds = LogicBoundsAt(trigger, type, lindex);
            Rectangle ret = new Rectangle(
                logicBounds.X + 1,
                logicBounds.Y + HeaderHeight + (pindex * VarSpacing) + (pindex * VarHeight) + (VarHeight/2),
                logicBounds.Width - 2,
                VarNameHeight);
            return ret;
        }
        public static Rectangle LogicParamValBoundsAt(Trigger trigger, LogicType type, int lindex, int pindex)
        {
            Rectangle logicBounds = LogicBoundsAt(trigger, type, lindex);
            Rectangle ret = new Rectangle(
                logicBounds.X + 1,
                logicBounds.Y + HeaderHeight + (pindex * VarSpacing) + (pindex * VarHeight) + (VarHeight/2) + VarNameHeight,
                logicBounds.Width - 2,
                VarValHeight);
            return ret;
        }
        public static int FirstLogicInPoint(Trigger trigger, LogicType type, Point point)
        {
            IEnumerable<ILogic> logics;
            if (type == LogicType.Condition)
                logics = trigger.Conditions;
            else if (type == LogicType.EffectTrue)
                logics = trigger.TriggerEffectsOnTrue;
            else
                logics = trigger.TriggerEffectsOnFalse;


            for (int i = 0; i < logics.Count(); i++)
            {
                if (LogicBoundsAt(trigger, type, i).Contains(point))
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool TransferLogic(Trigger fromTrigger, LogicType fromType, int fromIndex, Trigger toTrigger, LogicType toType, int toIndex)
        {
            if (!LogicTransferValid(fromType, toType)) return false;
            //if (fromIndex >= TriggerLogicCount(fromTrigger, fromType)) return false;
            //if (toIndex >= TriggerLogicCount(toTrigger, toType)) return false;

            if (fromType == LogicType.Condition && toType == LogicType.Condition)
            {
                var move = fromTrigger.Conditions[fromIndex];
                fromTrigger.Conditions.Remove(move);
                toTrigger.Conditions.Insert(toIndex, move);
                return true;
            }

            Effect eff = null;
            if (fromType == LogicType.EffectTrue)
            {
                eff = fromTrigger.TriggerEffectsOnTrue[fromIndex];
                fromTrigger.TriggerEffectsOnTrue.Remove(eff);
            }
            if (fromType == LogicType.EffectFalse)
            {
                eff = fromTrigger.TriggerEffectsOnFalse[fromIndex];
                fromTrigger.TriggerEffectsOnFalse.Remove(eff);
            }
            if (eff == null) return false;

            if (toType == LogicType.EffectTrue)
            {
                toTrigger.TriggerEffectsOnTrue.Insert(toIndex, eff);
                return true;
            }
            if (toType == LogicType.EffectFalse)
            {
                toTrigger.TriggerEffectsOnFalse.Insert(toIndex, eff);
                return true;
            }

            return false;
        }
        public static bool LogicTransferValid(LogicType from, LogicType to)
        {
            if (from == to) return true;
            if (from == LogicType.EffectTrue && to == LogicType.EffectFalse) return true;
            if (from == LogicType.EffectFalse && to == LogicType.EffectTrue) return true;
            return false;
        }

        public static TriggerscriptSelection TrySelectAtPoint(Triggerscript script, Point point)
        {
            TriggerscriptSelection ret = new TriggerscriptSelection();

            ret.TriggerId = -1;
            ret.LogicIndex = -1;
            foreach (var trigger in script.Triggers.Values)
            {
                if (UnitBounds(trigger).Contains(point))
                {
                    ret.TriggerId = trigger.ID;

                    foreach (var type in Enum.GetValues<LogicType>())
                    {
                        if (LogicBoundsAll(trigger, type).Contains(point))
                        {
                            ret.LogicType = type;
                            ret.LogicIndex = FirstLogicInPoint(trigger, type, point);
                            if (ret.LogicIndex != -1)
                            {
                                return ret;
                            }
                            if(TriggerLogicCount(trigger, type) == 0)
                            {
                                ret.LogicIndex = 0;
                                return ret;
                            }
                        }
                    }

                    if (TriggerBounds(trigger).Contains(point))
                    {
                        return ret;
                    }
                }
            }
            ret.TriggerId = -1;
            return ret;
        }
    }

    public static class TriggerscriptEditorRendererGDI
    {
        public static void DrawScript(Graphics g, Rectangle clip, Triggerscript script, TriggerscriptSelection sel, TriggerscriptSelection hover)
        {
            //draw the triggers in a reverse order to match the top-down selection logic.
            for (int i = script.Triggers.Count - 1; i >= 0; i--)
            {
                //TODO: clip culling
                Trigger trigger = script.Triggers.Values.ElementAt(i);
                DrawTrigger(g, trigger, sel);
                DrawLogicBases(g, script, trigger, LogicType.Condition, sel);
                DrawLogicBases(g, script, trigger, LogicType.EffectTrue, sel);
                DrawLogicBases(g, script, trigger, LogicType.EffectFalse, sel);
                DrawTriggerContainer(g, trigger, sel);
                
                //DrawTriggerDebug(g, trigger, sel);
            }
        }

        private static void DrawTrigger(Graphics g, Trigger trigger, TriggerscriptSelection sel)
        {
            Rectangle bounds = TriggerBounds(trigger);

            g.FillRectangle(new SolidBrush(BodyColor), bounds);
            g.FillRectangle(new SolidBrush(TriggerHeaderColor), bounds.X, bounds.Y, bounds.Width, HeaderHeight);

            if (trigger.ID == sel.TriggerId && sel.LogicIndex == -1)
            {
                g.DrawRectangle(new Pen(Color.White, .5f), bounds);
            }
            else
            {
                g.DrawRectangle(new Pen(TrimColor, .25f), bounds);
            }

            g.DrawString(
                trigger.Name,
                TitleFont,
                new SolidBrush(TextColor),
                new Rectangle(
                        bounds.X + Margin,
                        bounds.Y + Margin,
                        bounds.Width - (Margin * 2),
                        HeaderHeight - (Margin * 2))
                );

        }
        
        private static void DrawLogicBases(Graphics g, Triggerscript script, Trigger trigger, LogicType type, TriggerscriptSelection sel)
        {
            IEnumerable<ILogic> logics;
            Color headerColor = Color.Black;
            if (type == LogicType.Condition)
            {
                headerColor = ConditionHeaderColor;
                logics = trigger.Conditions;
            }
            else if (type == LogicType.EffectTrue)
            {
                headerColor = EffectHeaderColor;
                logics = trigger.TriggerEffectsOnTrue;
            }
            else if (type == LogicType.EffectFalse)
            {
                headerColor = EffectHeaderColor;
                logics = trigger.TriggerEffectsOnFalse;
            }
            else return;


            for (int i = 0; i < logics.Count(); i++)
            {
                Rectangle bounds = LogicBoundsAt(trigger, type, i);
                g.FillRectangle(new SolidBrush(BodyColor), bounds);
                g.FillRectangle(new SolidBrush(headerColor), bounds.X, bounds.Y, bounds.Width, HeaderHeight);

                List<LogicParam> pars = new List<LogicParam>();
                if (logics.ElementAt(i).Inputs != null)
                    pars.AddRange(logics.ElementAt(i).Inputs);
                if (logics.ElementAt(i).Outputs != null)
                    pars.AddRange(logics.ElementAt(i).Outputs);

                for (int j = 0; j < pars.Count; j++)
                {
                    Rectangle varNameBounds = LogicParamNameBoundsAt(trigger, type, i, j);
                    Rectangle varValBounds = LogicParamValBoundsAt(trigger, type, i, j);

                    g.DrawString(pars[j].Name, TextFont, new SolidBrush(TextColor), varNameBounds);

                    string varValStr = script.TriggerVars[pars[j].Value].Name;
                    if (varValStr == "") varValStr = "NO NAME";

                    g.FillRectangle(new SolidBrush(TrimColor), varValBounds);
                    g.DrawString(
                        string.Format("[{0}] - {1}", pars[j].Value, varValStr),
                        TextFont,
                        new SolidBrush(TextColor),
                        varValBounds);
                }

                string title = "";
                if (logics.ElementAt(i) is Effect)
                {
                    title = (logics.ElementAt(i) as Effect).Type.ToString();
                    if ((logics.ElementAt(i) as Effect).Version != -1)
                        title += " v" + (logics.ElementAt(i) as Effect).Version;
                }

                g.DrawString(
                    title,
                    TitleFont,
                    new SolidBrush(TextColor),
                    new Rectangle(
                        bounds.X + Margin,
                        bounds.Y + Margin,
                        bounds.Width - (Margin * 2),
                        HeaderHeight - (Margin * 2))
                    );

                if (trigger.ID == sel.TriggerId
                    && sel.LogicType == type
                    && sel.LogicIndex == i)
                {
                    g.DrawRectangle(new Pen(Color.White, .5f), bounds);
                }
                else
                {
                    g.DrawRectangle(new Pen(TrimColor, .25f), bounds);
                }
            }
        }
        private static void DrawConditions(Graphics g, Trigger trigger, TriggerscriptSelection sel)
        {
            for (int i = 0; i < trigger.Conditions.Count; i++)
            {
                Rectangle bounds = LogicBoundsAt(trigger, LogicType.Condition, i);
                g.FillRectangle(new SolidBrush(BodyColor), bounds);
                g.FillRectangle(new SolidBrush(ConditionHeaderColor), bounds.X, bounds.Y, bounds.Width, HeaderHeight);

                if (trigger.ID == sel.TriggerId && sel.LogicType == LogicType.Condition && sel.LogicIndex == i)
                {
                    g.DrawRectangle(new Pen(Color.White), bounds);
                }
            }

        }
        private static void DrawTrueEffects(Graphics g, Trigger trigger, TriggerscriptSelection sel)
        {
            for (int i = 0; i < trigger.TriggerEffectsOnTrue.Count; i++)
            {

            }
        }
        private static void DrawFalseEffects(Graphics g, Trigger trigger, TriggerscriptSelection sel)
        {
            for (int i = 0; i < trigger.TriggerEffectsOnFalse.Count; i++)
            {
                Rectangle bounds = LogicBoundsAt(trigger, LogicType.EffectFalse, i);
                g.FillRectangle(new SolidBrush(BodyColor), bounds);
                g.FillRectangle(new SolidBrush(EffectHeaderColor), bounds.X, bounds.Y, bounds.Width, HeaderHeight);

                if (trigger.ID == sel.TriggerId && sel.LogicType == LogicType.EffectFalse && sel.LogicIndex == i)
                {
                    g.DrawRectangle(new Pen(Color.White), bounds);
                }
            }

        }
        
        private static void DrawTriggerContainer(Graphics g, Trigger trigger, TriggerscriptSelection sel)
        {
            Point[] triangle = new Point[3]
            {
                new Point(1, 1),
                new Point(1, HeaderHeight - 1),
                new Point(LogicSectionSpacing - 1, HeaderHeight / 2)
            };

            //if (trigger.Conditions.Count > 0)
            {
                Point[] cndTri = new Point[3];
                Array.Copy(triangle, cndTri, 3);
                Point firstConditionLoc = LogicBoundsAll(trigger, LogicType.Condition).Location;
                for (int i = 0; i < triangle.Length; i++)
                {
                    cndTri[i].X += firstConditionLoc.X - LogicSectionSpacing;
                    cndTri[i].Y += firstConditionLoc.Y;
                }
                g.FillPolygon(new SolidBrush(Color.Black), cndTri);
            }

            //if (trigger.TriggerEffectsOnTrue.Count > 0)
            {
                Point[] teffTri = new Point[3];
                Array.Copy(triangle, teffTri, 3);
                Point firstTrueEffLoc = LogicBoundsAll(trigger, LogicType.EffectTrue).Location;
                for (int i = 0; i < triangle.Length; i++)
                {
                    teffTri[i].X += firstTrueEffLoc.X - LogicSectionSpacing;
                    teffTri[i].Y += firstTrueEffLoc.Y;
                }
                g.FillPolygon(new SolidBrush(Color.Black), teffTri);
            }

            //if (trigger.TriggerEffectsOnFalse.Count > 0)
            {
                Point[] feffTri = new Point[3];
                Array.Copy(triangle, feffTri, 3);
                Point firstFalseEffLoc = LogicBoundsAll(trigger, LogicType.EffectFalse).Location;
                for (int i = 0; i < triangle.Length; i++)
                {
                    feffTri[i].X += firstFalseEffLoc.X - LogicSectionSpacing;
                    feffTri[i].Y += firstFalseEffLoc.Y;
                }
                g.FillPolygon(new SolidBrush(Color.Black), feffTri);
            }
        }
        
        private static void DrawTriggerDebug(Graphics g, Trigger trigger, TriggerscriptSelection sel)
        {
            g.DrawRectangle(new Pen(Color.YellowGreen, 1), LogicBoundsAll(trigger, LogicType.Condition));
            g.DrawRectangle(new Pen(Color.Yellow, 1), LogicBoundsAll(trigger, LogicType.EffectTrue));
            g.DrawRectangle(new Pen(Color.Green, 1), LogicBoundsAll(trigger, LogicType.EffectFalse));
            g.DrawRectangle(new Pen(Color.Red, 1), UnitBounds(trigger));
        }
    }

    public class TriggerscriptEditor : NodeView
    {
        public Triggerscript TriggerscriptFile { get; set; }
        public TriggerscriptSelection Selection { get; private set; }
        public TriggerscriptSelection Hover { get; private set; }

        private OperatorRegistrantToolstrip OperatorRegistrant { get; set; }
        private PointF CapturedLocation { get; set; }

        public TriggerscriptEditor(FoundryInstance i) : base(i)
        {
            Form.ContextMenuStrip = new ContextMenuStrip();
            Form.ContextMenuStrip.Opened += (sender, e) =>
            {
                CapturedLocation = GetTransformedMousePos();
            };
            OperatorRegistrant = new OperatorRegistrantToolstrip();
            CapturedLocation = new PointF(0, 0);

            Operator opAddTrigger = new Operator("Add Trigger");
            opAddTrigger.OperatorActivated += (sender, e) =>
            {
                //((ScriptData)NodeData).AddTrigger(string.Format("NewTrigger{0}", Random.Shared.Next()), CapturedLocation);
            };
            OperatorRegistrant.Operators.Add(opAddTrigger);

            Dictionary<string, Operator> opConditionCategories = new Dictionary<string, Operator>();
            Operator opAddCondition = new Operator("Add Condition");
            foreach (var pair in ConditionItems.Values)
            {
                foreach (var versions in pair.Values)
                {
                    foreach (var version in pair.Values)
                    {
                        string category = "";//ConditionCategories[version.Name];
                        string running = "";
                        Operator last = opAddCondition;
                        foreach (string entry in category.Split("|"))
                        {
                            running += "|" + entry;
                            if (!opConditionCategories.ContainsKey(running))
                            {
                                Operator entryOp = new Operator(entry);
                                entryOp.Parent = last;
                                last = entryOp;
                                opConditionCategories.Add(running, entryOp);
                            }
                            else
                            {
                                last = opConditionCategories[running];
                            }
                        }

                        string ver = version.Version == -1 ? "" : " v" + version.Version.ToString();
                        Operator opEffect = new Operator(string.Format("{0}{1}", version.Name, ver));
                        opEffect.OperatorActivated += (sender, e) =>
                        {
                            //((ScriptData)NodeData).AddCondition(version.DBID, version.Version, CapturedLocation);
                        };
                        opEffect.Parent = opConditionCategories["|" + category];
                    }
                }
            }
            OperatorRegistrant.AddOperator(opAddCondition);

            Dictionary<string, Operator> opEffectCategories = new Dictionary<string, Operator>();
            Operator opAddEffect = new Operator("Add Effect");
            foreach (var pair in EffectItems.Values)
            {
                foreach (var version in pair.Values)
                {
                    string category = EffectCategories[version.Name];
                    string running = "";
                    Operator last = opAddEffect;
                    foreach (string entry in category.Split("|"))
                    {
                        running += "|" + entry;
                        if (!opEffectCategories.ContainsKey(running))
                        {
                            Operator entryOp = new Operator(entry);
                            entryOp.Parent = last;
                            last = entryOp;
                            opEffectCategories.Add(running, entryOp);
                        }
                        else
                        {
                            last = opEffectCategories[running];
                        }
                    }

                    string ver = version.Version == -1 ? "" : " v" + version.Version.ToString();
                    Operator opEffect = new Operator(string.Format("{0}{1}", version.Name, ver));
                    opEffect.OperatorActivated += (sender, e) =>
                    {
                        //((ScriptData)NodeData).AddEffect(version.DBID, version.Version, CapturedLocation);
                    };
                    opEffect.Parent = opEffectCategories["|" + category];
                }
            }
            OperatorRegistrant.AddOperator(opAddEffect);

            Operator opAddVar = new Operator("Add Variable");
            foreach (string type in Enum.GetNames<VarType>())
            {
                Operator opVar = new Operator(type);
                opVar.OperatorActivated += (sender, e) =>
                {
                    //((ScriptData)NodeData).AddVariable(Enum.Parse<ScriptVarType>(type), CapturedLocation);
                };
                opVar.Parent = opAddVar;
            }
            OperatorRegistrant.AddOperator(opAddVar);

            Form.ContextMenuStrip.Items.AddRange(OperatorRegistrant.GetRootMenuItems().ToArray());

            //debug save
            ViewTick += (sender, e) =>
            {
                if (GetKeyIsDown(Keys.F5) && !GetKeyWasDown(Keys.F5))
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Script(*.triggerscript)|*.triggerscript";
                    if (sfd.ShowDialog(Instance) == DialogResult.OK)
                    {
                        var ser = new YAXSerializer<Triggerscript>(new SerializerOptions() { ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow });
                        //ser.SerializeToFile(((ScriptData)NodeData).TriggerscriptData, sfd.FileName);
                    }
                }
            };

            ViewTick += (sender, e) =>
            {
                OnTick();
            };

            //ViewDraw += (sender, e) =>
            //{
            //    OnDraw(e.Graphics, e.ClipRectangle);
            //};
            Form.Paint += OnPaint;
        }

        private void OnTick()
        {
            if (TriggerscriptFile == null) return;
            MouseState mouse = GetMouseState();
            Point mouseP = GetTransformedMousePos();

            if (mouse.leftDown && !mouse.leftDownLast)
            {
                Selection = TrySelectAtPoint(TriggerscriptFile, mouseP);
            }

            if (mouse.leftDown && Selection.TriggerId != -1)
            {
                if (Selection.LogicIndex == -1)
                {
                    Trigger selected = TriggerscriptFile.Triggers[Selection.TriggerId];
                    if (selected != null)
                    {
                        selected.X += mouse.deltaX * (1 / currentViewZoom);
                        selected.Y += mouse.deltaY * (1 / currentViewZoom);
                    }
                }
            }

            if (mouse.leftDown && mouse.leftDownLast && Selection.LogicIndex != -1)
            {
                Hover = TrySelectAtPoint(TriggerscriptFile, mouseP);
                if (Hover != Selection)
                {
                    if (Hover.LogicIndex != -1 && LogicTransferValid(Selection.LogicType, Hover.LogicType))
                    {
                        Trigger from = TriggerscriptFile.Triggers[Selection.TriggerId];
                        Trigger to = TriggerscriptFile.Triggers[Hover.TriggerId];

                        if (TriggerLogicCount(to, Hover.LogicType) > 0)
                        {
                            Rectangle toBounds = LogicBoundsAt(to, Hover.LogicType, Hover.LogicIndex);
                            int toIndex = Hover.LogicIndex;

                            if (Hover != Selection)
                            {
                                if (Hover.LogicIndex == Selection.LogicIndex + 1)
                                {
                                    if (mouseP.X <= toBounds.X + (toBounds.Width / 2)) return;
                                }
                                if (Hover.LogicIndex == Selection.LogicIndex - 1)
                                {
                                    if (mouseP.X > toBounds.X + (toBounds.Width / 2)) return;
                                }
                            }
                            if (from != to || Hover.LogicType != Selection.LogicType)
                            {
                                if (mouseP.X > toBounds.X + (toBounds.Width / 2)) toIndex += 1;
                            }

                            if (TransferLogic(from, Selection.LogicType, Selection.LogicIndex, to, Hover.LogicType, toIndex))
                            {
                                Selection = new TriggerscriptSelection()
                                {
                                    LogicIndex = toIndex,
                                    LogicType = Hover.LogicType,
                                    TriggerId = Hover.TriggerId
                                };
                            }
                        }
                        else
                        {
                            Rectangle toBounds = LogicBoundsAll(to, Hover.LogicType);
                            int toIndex = 0;
                            if (!toBounds.Contains(mouseP)) return;

                            if (TransferLogic(from, Selection.LogicType, Selection.LogicIndex, to, Hover.LogicType, toIndex))
                            {
                                Selection = new TriggerscriptSelection()
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

        private void OnPaint(object o, PaintEventArgs e)
        {
            if (TriggerscriptFile == null) return;
            TriggerscriptEditorRendererGDI.DrawScript(e.Graphics, e.ClipRectangle, TriggerscriptFile, Selection, Selection);
        }
    }
}
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Foundry.HW1.Triggerscript.EditorHelpers;
using static Foundry.HW1.Triggerscript.EditorParams;
using static Foundry.HW1.Triggerscript.EditorRendererWinForms;

namespace Foundry.HW1.Triggerscript
{
    public static class EditorRendererWinForms
    {
        public static void DrawScript(Graphics g, Rectangle clip, Triggerscript script, Selection sel, Selection hover, bool drawDetail, bool drawLOD)
        {
            DrawBackground(g, clip, script);
            foreach (Trigger trigger in script.Triggers.Values)
            {
                Rectangle bounds = UnitBounds(trigger);
                if (!bounds.IntersectsWith(clip)) continue;

                if (!drawLOD)
                    DrawUnit(g, script, trigger, sel, drawDetail);
                else
                    DrawUnitProxy(g, bounds, trigger.Name, drawDetail);
            }
        }

        private static void DrawBackground(Graphics g, Rectangle clip, Triggerscript script)
        {
            Rectangle bounds = ScriptBounds(script);
            g.DrawRectangle(new Pen(Color.Red, 4), bounds);
        }

        private static void DrawUnit(Graphics g, Triggerscript script, Trigger trigger, Selection sel, bool drawDetail)
        {
            DrawTrigger(g, trigger, sel, drawDetail);
            DrawLogicBases(g, script, trigger, TriggerLogicSlot.Condition, sel, drawDetail);
            DrawLogicBases(g, script, trigger, TriggerLogicSlot.EffectTrue, sel, drawDetail);
            DrawLogicBases(g, script, trigger, TriggerLogicSlot.EffectFalse, sel, drawDetail);
            DrawTriggerContainer(g, trigger, sel, drawDetail);
        }
        private static void DrawUnitProxy(Graphics g, Rectangle bounds, string text, bool drawDetail)
        {
            bounds.Width = Math.Max(bounds.Width, 200);
            bounds.Height = Math.Max(bounds.Height, 65);

            g.FillRectangle(new SolidBrush(BodyColor), bounds);
            g.DrawRectangle(new Pen(TrimColor), bounds);

            if (drawDetail)
                g.DrawString(text, HugeFont, new SolidBrush(TextColor), bounds, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }

        private static void DrawTrigger(Graphics g, Trigger trigger, Selection sel, bool drawDetail)
        {
            Rectangle bounds = TriggerBounds(trigger);

            g.FillRectangle(new SolidBrush(BodyColor), bounds);
            g.FillRectangle(new SolidBrush(TriggerHeaderColor), bounds.X, bounds.Y, bounds.Width, HeaderHeight);

            if (trigger.ID == sel.TriggerId && sel.LogicIndex == -1)
            {
                g.DrawRectangle(new Pen(Color.White, .5f), bounds);
            }
            else if (drawDetail)
            {
                g.DrawRectangle(new Pen(TrimColor, .25f), bounds);
            }

            //draw name title
            if (drawDetail)
            {
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
        }
        private static void DrawLogicBases(Graphics g, Triggerscript script, Trigger trigger, TriggerLogicSlot type, Selection sel, bool drawDetail)
        {
            IEnumerable<Logic> logics = Logics(trigger, type);
            Color headerColor = type == TriggerLogicSlot.Condition ? ConditionHeaderColor : EffectHeaderColor;

            for (int i = 0; i < logics.Count(); i++)
            {
                Logic cur = logics.ElementAt(i);
                Rectangle bounds = LogicBounds(trigger, type, i);

                //Draw background
                g.FillRectangle(new SolidBrush(BodyColor), bounds);
                g.FillRectangle(new SolidBrush(headerColor), bounds.X, bounds.Y, bounds.Width, HeaderHeight);

                //Draw outline
                if (sel.TriggerId == trigger.ID
                    && sel.LogicType == type
                    && sel.LogicIndex == i)
                {
                    g.DrawRectangle(new Pen(Color.White, .5f), bounds);
                }
                else
                {
                    //draw outline when not selected only when quality is high.
                    if (drawDetail)
                    {
                        g.DrawRectangle(new Pen(TrimColor, .25f), bounds);
                    }
                }

                //Draw param slots, only if quality is set to high.
                if (drawDetail)
                {
                    int paramIndex = 0;
                    foreach (var (sigid, param) in cur.StaticParamInfo)
                    {
                        //name
                        Rectangle varNameBounds = ParamNameBounds(trigger, type, i, paramIndex);

                        g.DrawString(
                        param.Name + " [" + param.Type + "]",
                        TextFont, new SolidBrush(TextColor),
                        varNameBounds,
                        new StringFormat() { LineAlignment = StringAlignment.Center });

                        //value
                        int varId = cur.GetValueOfParam(sigid);

                        Rectangle varValBounds = ParamValBounds(trigger, type, i, paramIndex);
                        g.FillRectangle(new SolidBrush(TrimColor), varValBounds);
                        if (!script.TriggerVars.ContainsKey(varId))
                        {
                            paramIndex++;
                            continue;
                        }

                        //outline var value box when selected.
                        if (sel.TriggerId == trigger.ID
                            && sel.LogicType == type
                            && sel.LogicIndex == i
                            && sel.VarSigId == sigid)
                        {
                            g.DrawRectangle(new Pen(Color.White, .125f), varValBounds);
                        }

                        string varValStr = script.TriggerVars[varId].Name;
                        if (varValStr == "") varValStr = "NO NAME";

                        //shrink it by a margin for the string
                        varValBounds.Inflate(-Margin, 0);
                        g.DrawString(
                            varValStr,
                            TextFont,
                            new SolidBrush(TextColor),
                            varValBounds,
                            new StringFormat() { LineAlignment = StringAlignment.Center });
                        
                        paramIndex++;
                    }
                }

                //Draw logic title
                if (drawDetail)
                {
                    string title = logics.ElementAt(i).TypeName;
                    if (logics.ElementAt(i).Version != -1)
                        title += " [v" + logics.ElementAt(i).Version + "]";
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
                }

                //Draw comment
                if (logics.ElementAt(i).Comment != "")
                {
                    //backdrop
                    g.FillRectangle(new SolidBrush(BodyColor),
                        bounds.X,
                        bounds.Y - CommentHeight - LogicSpacing - FooterHeight,
                        bounds.Width,
                        CommentHeight);
                    g.FillRectangle(new SolidBrush(headerColor),
                        bounds.X,
                        bounds.Y - LogicSpacing - FooterHeight,
                        bounds.Width,
                        FooterHeight);

                    //draw text and outline if high quality only
                    if (drawDetail)
                    {
                        g.DrawRectangle(new Pen(TrimColor, .25f),
                        bounds.X,
                        bounds.Y - CommentHeight - LogicSpacing - FooterHeight,
                        bounds.Width,
                        CommentHeight + FooterHeight);

                        g.DrawString(logics.ElementAt(i).Comment, TextFont, new SolidBrush(TextColor),
                            new Rectangle(
                                bounds.X + Margin,
                                bounds.Y + Margin - CommentHeight - LogicSpacing - FooterHeight,
                                bounds.Width - (Margin * 2),
                                CommentHeight - (Margin * 2)));
                    }
                }


            }

        }
        private static void DrawTriggerContainer(Graphics g, Trigger trigger, Selection sel, bool drawDetail)
        {
            if (!drawDetail) return;

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
                Point firstConditionLoc = LogicBounds(trigger, TriggerLogicSlot.Condition).Location;
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
                Point firstTrueEffLoc = LogicBounds(trigger, TriggerLogicSlot.EffectTrue).Location;
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
                Point firstFalseEffLoc = LogicBounds(trigger, TriggerLogicSlot.EffectFalse).Location;
                for (int i = 0; i < triangle.Length; i++)
                {
                    feffTri[i].X += firstFalseEffLoc.X - LogicSectionSpacing;
                    feffTri[i].Y += firstFalseEffLoc.Y;
                }
                g.FillPolygon(new SolidBrush(Color.Black), feffTri);
            }
        }
        private static void DrawTriggerDebug(Graphics g, Trigger trigger, Selection sel)
        {
            g.DrawRectangle(new Pen(Color.YellowGreen, 1), LogicBounds(trigger, TriggerLogicSlot.Condition));
            g.DrawRectangle(new Pen(Color.Yellow, 1), LogicBounds(trigger, TriggerLogicSlot.EffectTrue));
            g.DrawRectangle(new Pen(Color.Green, 1), LogicBounds(trigger, TriggerLogicSlot.EffectFalse));
            g.DrawRectangle(new Pen(Color.Red, 1), UnitBounds(trigger));
        }
    }
}

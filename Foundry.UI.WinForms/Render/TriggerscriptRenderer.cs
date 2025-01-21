﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Drawing;
using static Chef.HW1.Script.TriggerscriptHelpers;
using static Chef.HW1.Script.TriggerscriptParams;
using Chef.HW1.Script;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Chef.Win.Render
{
    public static class TriggerscriptRenderer
    {
        public static Font TitleFont { get; } = new Font("Consolas", 2.4f, FontStyle.Regular);
        public static Font SubtitleFont { get; } = new Font("Consolas", 2.3f, FontStyle.Regular);
        public static Font TextFont { get; } = new Font("Consolas", 2.2f, FontStyle.Regular);
        public static Font HugeFont { get; } = new Font("Consolas", 20.0f, FontStyle.Regular);

        public static void DrawScript(Graphics g, Rectangle clip, Triggerscript script, Selection sel, Selection hover, bool drawDetail, bool drawLOD)
        {
            DrawBackground(g, clip, script);
            foreach (Trigger trigger in script.Triggers.Values)
            {
                Rectangle bounds = BoundsTrigger(trigger);
                if (!bounds.IntersectsWith(clip)) continue;

                if (!drawLOD)
                    DrawUnit(g, script, trigger, sel, hover, drawDetail);
                else
                    DrawUnitProxy(g, bounds, trigger.Name, drawDetail, trigger.Active);
            }
        }

        private static void DrawBackground(Graphics g, Rectangle clip, Triggerscript script)
        {
            Rectangle bounds = BoundsScript(script);
            g.DrawRectangle(new Pen(Color.Red, 4), bounds);
        }

        private static void DrawUnit(Graphics g, Triggerscript script, Trigger trigger, Selection sel, Selection hover, bool drawDetail)
        {
            DrawBackdrop(g, trigger, sel.TriggerId, sel.LogicIndex, drawDetail);
            DrawLogicHeaders(g, script, trigger, drawDetail);
            DrawLogicBases(g, script, trigger, TriggerLogicSlot.Condition, sel, drawDetail);
            DrawLogicBases(g, script, trigger, TriggerLogicSlot.EffectTrue, sel, drawDetail);
            DrawLogicBases(g, script, trigger, TriggerLogicSlot.EffectFalse, sel, drawDetail);
        }
        private static void DrawUnitProxy(Graphics g, Rectangle bounds, string text, bool drawDetail, bool active)
        {
            bounds.Width = Math.Max(bounds.Width, 200);
            bounds.Height = Math.Max(bounds.Height, 65);

            g.FillRectangle(new SolidBrush(BodyColor), bounds);
            g.DrawRectangle(new Pen(TrimColor, 4), bounds);
            if (active)
            {
                Rectangle boundsActive = bounds;
                boundsActive.Inflate(-4, -4);
                g.DrawRectangle(new Pen(TriggerActiveColor, 4), boundsActive);
            }

            if (drawDetail)
                g.DrawString(text, HugeFont, new SolidBrush(TextColor), bounds, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }

        private static void DrawBackdrop(Graphics g, Trigger trigger, int selectedTrigger, int selectedLogic, bool detail)
        {
            Rectangle bounds = BoundsTrigger(trigger);
            bounds.Inflate(Margin * 5, Margin * 5);
            g.FillRectangle(new SolidBrush(UnitColor), bounds);

            g.DrawRectangle(new Pen(trigger.Active ? TriggerActiveColor : TrimColor), bounds);

            if (selectedTrigger == trigger.ID && selectedLogic == -1)
            {
                Rectangle sel = bounds;
                sel.Inflate(1, 1);
                g.DrawRectangle(new Pen(Color.White), sel);
            }
        }
        private static void DrawLogicBases(Graphics g, Triggerscript script, Trigger trigger, TriggerLogicSlot type, Selection sel, bool drawDetail)
        {
            IEnumerable<Logic> logics = Logics(trigger, type);
            Color headerColor = type == TriggerLogicSlot.Condition ? ConditionHeaderColor : EffectHeaderColor;

            for (int i = 0; i < logics.Count(); i++)
            {
                Logic cur = logics.ElementAt(i);
                Rectangle bounds = BoundsLogicBody(trigger, type, i);

                //Draw background
                g.FillRectangle(new SolidBrush(BodyColor), bounds);
                g.FillRectangle(new SolidBrush(headerColor), bounds.X, bounds.Y, bounds.Width, HeaderHeight);

                //Draw outline
                //only draw trim outlines if quality is set to high.
                if (drawDetail)
                {
                    //header-body divider
                    g.DrawLine(new Pen(TrimColor, Margin), 
                        bounds.X,
                        bounds.Y + HeaderHeight,
                        bounds.X + bounds.Width,
                        bounds.Y + HeaderHeight);
                    //full outline
                    g.DrawRectangle(new Pen(TrimColor, Margin), bounds);
                }
                //always draw selection outline.
                if (sel.TriggerId == trigger.ID
                    && sel.LogicType == type
                    && sel.LogicIndex == i)
                {
                    g.DrawRectangle(new Pen(Color.White, Margin), bounds);
                }

                //Draw param slots, only if quality is set to high.
                if (drawDetail)
                {
                    int paramIndex = 0;
                    foreach (var (sigid, param) in cur.StaticParamInfo)
                    {
                        //name
                        Rectangle varNameBounds = BoundsParamName(trigger, type, i, paramIndex);

                        g.DrawString(
                            param.Name + " [" + param.Type + "]",
                            TextFont, new SolidBrush(TextColor),
                            varNameBounds,
                            new StringFormat() { LineAlignment = StringAlignment.Center }
                        );

                        //value
                        int varId = cur.GetValueOfParam(sigid);

                        Rectangle varValBounds = BoundsParamValue(trigger, type, i, paramIndex);
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
                            bounds.Width - Margin * 2,
                            HeaderHeight - Margin * 2),
                        new StringFormat()
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        }
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
                                bounds.Width - Margin * 2,
                                CommentHeight - Margin * 2));
                    }
                }


            }

        }
        private static void DrawLogicHeaders(Graphics g, Triggerscript script, Trigger trigger, bool detail)
        {
            Rectangle bounds = BoundsTrigger(trigger);

            bounds.Height = HeaderHeight;
            g.FillRectangle(new SolidBrush(TriggerHeaderColor), bounds);
            g.DrawRectangle(new Pen(TrimColor, Margin), bounds);
            g.DrawString(trigger.Name, TitleFont, new SolidBrush(TextColor), bounds, new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
            });

            bounds.Y += HeaderHeight;
            Rectangle lbounds;

            lbounds = BoundsLogicSlot(trigger, TriggerLogicSlot.Condition);
            lbounds.Height = bounds.Height;
            lbounds.Y = bounds.Y;
            lbounds.Inflate(0, -2);
            g.FillRectangle(new SolidBrush(ConditionHeaderColor), lbounds);
            g.DrawRectangle(new Pen(TrimColor, Margin), lbounds);
            string cndAndStr = trigger.ConditionsAreAND ? "All" : "Any";
            string cndStr = trigger.ConditionalTrigger ? cndAndStr + " Pass" : "Await " + cndAndStr;
            lbounds.Inflate(-3, 0);
            if (detail) g.DrawString(cndStr, TextFont, new SolidBrush(TextColor), lbounds, new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
            });

            lbounds = BoundsLogicSlot(trigger, TriggerLogicSlot.EffectTrue);
            lbounds.Height = bounds.Height;
            lbounds.Y = bounds.Y;
            lbounds.Inflate(0, -2);
            g.FillRectangle(new SolidBrush(EffectHeaderColor), lbounds);
            g.DrawRectangle(new Pen(TrimColor, Margin), lbounds);
            string eftStr = trigger.ConditionalTrigger ? "Pass" : "Do";
            lbounds.Inflate(-3, 0);
            if (detail) g.DrawString(eftStr, TextFont, new SolidBrush(TextColor), lbounds, new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
            });

            lbounds = BoundsLogicSlot(trigger, TriggerLogicSlot.EffectFalse);
            lbounds.Height = bounds.Height;
            lbounds.Y = bounds.Y;
            lbounds.Inflate(0, -2);
            g.FillRectangle(new SolidBrush(EffectHeaderColor), lbounds);
            g.DrawRectangle(new Pen(TrimColor, Margin), lbounds);
            string effStr = trigger.ConditionalTrigger ? "Fail" : "-----";
            lbounds.Inflate(-3, 0);
            if (detail) g.DrawString(effStr, TextFont, new SolidBrush(TextColor), lbounds, new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
            });
        }
        private static void DrawLogicContainer(Graphics g, Triggerscript script, Trigger trigger, TriggerLogicSlot slot, int hoverIndex, bool detail)
        {
            Rectangle bounds = BoundsLogicSlot(trigger, slot);
            bounds.Height = HeaderHeight - LogicSpacing;

            string text = "";
            Color textColor = TextColor;
            Color color = Color.Black;
            bool center = false;

            if (slot == TriggerLogicSlot.Condition)
            {
                text = trigger.ConditionalTrigger ? "Test" : "Await";
                color = ConditionHeaderColor;
            }
            if (slot == TriggerLogicSlot.EffectTrue)
            {
                text = trigger.ConditionsAreAND ? "If All" : "If Any";
                color = EffectHeaderColor;
            }
            if (slot == TriggerLogicSlot.EffectFalse)
            {
                if (trigger.ConditionalTrigger)
                {
                    text = "Else";
                    color = EffectHeaderColor;
                }
                else if (trigger.TriggerEffectsOnFalse.Count > 0)
                {
                    text = "UNREACHABLE";
                    textColor = Color.Red;
                    color = Color.Black;
                }
                else
                {
                    return; //if there are no nodes in this non-conditional trigger, dont draw the section.
                }
                
                color = trigger.ConditionalTrigger ? EffectHeaderColor : Color.Black;
            }

            bounds.Y += HeaderHeight;
            g.FillRectangle(new SolidBrush(TrimColor), bounds.Left - LogicSectionSpacing, bounds.Top, LogicSectionSpacing, bounds.Height - 2);
            g.FillRectangle(new SolidBrush(color), bounds);
            g.DrawRectangle(new Pen(TrimColor, Margin), bounds);

            if (hoverIndex != -1)
            {
#if DEBUG
                g.DrawRectangle(new Pen(Color.Green, 1), BoundsLogicInsert(trigger, slot, hoverIndex));
#endif
            }

            bounds.Inflate(-2, 0);
            g.DrawString(text, TitleFont, new SolidBrush(textColor), bounds, new StringFormat()
            {
                Alignment = center ? StringAlignment.Center : StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
            });
        }
        private static void DrawVarList(Graphics g, Triggerscript script, bool detail)
        {
        }

        private static void DrawTriggerDebug(Graphics g, Trigger trigger, Selection sel)
        {
            g.DrawRectangle(new Pen(Color.YellowGreen, 1), BoundsLogicSlot(trigger, TriggerLogicSlot.Condition));
            g.DrawRectangle(new Pen(Color.Yellow, 1), BoundsLogicSlot(trigger, TriggerLogicSlot.EffectTrue));
            g.DrawRectangle(new Pen(Color.Green, 1), BoundsLogicSlot(trigger, TriggerLogicSlot.EffectFalse));
            g.DrawRectangle(new Pen(Color.Red, 1), BoundsTrigger(trigger));
        }
    }
}

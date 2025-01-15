using Microsoft.VisualBasic;
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
                Rectangle bounds = BoundsTriggerUnit(trigger);
                if (!bounds.IntersectsWith(clip)) continue;

                if (!drawLOD)
                    DrawUnit(g, script, trigger, sel, hover, drawDetail);
                else
                    DrawUnitProxy(g, bounds, trigger.Name, drawDetail, trigger.Active);
            }
        }

        private static void DrawBackground(Graphics g, Rectangle clip, Triggerscript script)
        {
            Rectangle bounds = ScriptBounds(script);
            g.DrawRectangle(new Pen(Color.Red, 4), bounds);
        }

        private static void DrawUnit(Graphics g, Triggerscript script, Trigger trigger, Selection sel, Selection hover, bool drawDetail)
        {
            DrawTrigger(g, trigger, sel, drawDetail);
            DrawLogicBases(g, script, trigger, TriggerLogicSlot.Condition, sel, drawDetail);
            DrawLogicBases(g, script, trigger, TriggerLogicSlot.EffectTrue, sel, drawDetail);
            DrawLogicBases(g, script, trigger, TriggerLogicSlot.EffectFalse, sel, drawDetail);
            DrawLogicContainer(g, script, trigger, TriggerLogicSlot.Condition, hover.LogicType == TriggerLogicSlot.Condition ? hover.LogicIndex : -1, drawDetail);
            DrawLogicContainer(g, script, trigger, TriggerLogicSlot.EffectTrue, hover.LogicType == TriggerLogicSlot.EffectTrue ? hover.LogicIndex : -1, drawDetail);
            DrawLogicContainer(g, script, trigger, TriggerLogicSlot.EffectFalse, hover.LogicType == TriggerLogicSlot.EffectFalse ? hover.LogicIndex : -1, drawDetail);
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

        private static void DrawTrigger(Graphics g, Trigger trigger, Selection sel, bool drawDetail)
        {
            Rectangle bounds = BoundsTriggerNode(trigger);

            g.FillRectangle(new SolidBrush(BodyColor), bounds);
            g.FillRectangle(new SolidBrush(TriggerHeaderColor), bounds.X, bounds.Y, bounds.Width, HeaderHeight);

            if (trigger.Active)
            {
                Rectangle activeBounds = bounds;
                activeBounds.Inflate(-Margin, -Margin);
                g.DrawRectangle(new Pen(TriggerActiveColor, Margin), activeBounds);
            }

            if (trigger.ID == sel.TriggerId && sel.LogicIndex == -1)
            {
                g.DrawRectangle(new Pen(Color.White, Margin), bounds);
            }
            else if (drawDetail)
            {
                g.DrawRectangle(new Pen(TrimColor, Margin), bounds);
            }

            //draw name title
            if (drawDetail)
            {
                g.DrawString(
                    trigger.Name,
                    TitleFont,
                    new SolidBrush(TextColor),
                    new Rectangle(
                        bounds.X + Margin * 2, //double the margin for the active outline.
                        bounds.Y + Margin * 2,
                         bounds.Width - Margin * 4,
                        HeaderHeight - Margin * 4),
                    new StringFormat()
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    }
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
                Rectangle bounds = BoundsLogicNode(trigger, type, i);

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
                        Rectangle varNameBounds = ParamNameBounds(trigger, type, i, paramIndex);

                        g.DrawString(
                            param.Name + " [" + param.Type + "]",
                            TextFont, new SolidBrush(TextColor),
                            varNameBounds,
                            new StringFormat() { LineAlignment = StringAlignment.Center }
                        );

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
        private static void DrawLogicContainer(Graphics g, Triggerscript script, Trigger trigger, TriggerLogicSlot slot, int hoverIndex, bool detail)
        {
            Rectangle bounds = BoundsLogicUnit(trigger, slot);
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
                text = trigger.ConditionsAreAND ? "All Pass" : "Any Pass";
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

            g.FillRectangle(new SolidBrush(TrimColor), bounds.Left - LogicSectionSpacing, bounds.Top + 1, LogicSectionSpacing, bounds.Height - 2);
            g.FillRectangle(new SolidBrush(color), bounds);
            g.DrawRectangle(new Pen(TrimColor, Margin), bounds);

            if (hoverIndex != -1)
            {
                //g.DrawRectangle(new Pen(Color.RebeccaPurple, 1), BoundsLogicDrop(trigger, slot, hoverIndex));
            }

            bounds.Inflate(-2, 0);
            g.DrawString(text, TitleFont, new SolidBrush(textColor), bounds, new StringFormat()
            {
                Alignment = center ? StringAlignment.Center : StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
            });
        }

        private static void DrawTriggerDebug(Graphics g, Trigger trigger, Selection sel)
        {
            g.DrawRectangle(new Pen(Color.YellowGreen, 1), BoundsLogicUnit(trigger, TriggerLogicSlot.Condition));
            g.DrawRectangle(new Pen(Color.Yellow, 1), BoundsLogicUnit(trigger, TriggerLogicSlot.EffectTrue));
            g.DrawRectangle(new Pen(Color.Green, 1), BoundsLogicUnit(trigger, TriggerLogicSlot.EffectFalse));
            g.DrawRectangle(new Pen(Color.Red, 1), BoundsTriggerUnit(trigger));
        }
    }
}

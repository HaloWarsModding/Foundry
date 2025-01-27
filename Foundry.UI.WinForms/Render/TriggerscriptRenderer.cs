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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Chef.Win.Render
{
    public static class TriggerscriptRenderer
    {
        public static Font TitleFont { get; } = new Font("Consolas", 2.4f, FontStyle.Regular);
        public static Font SubtitleFont { get; } = new Font("Consolas", 2.3f, FontStyle.Regular);
        public static Font TextFont { get; } = new Font("Consolas", 2.2f, FontStyle.Regular);
        public static Font HugeFont { get; } = new Font("Consolas", 20.0f, FontStyle.Regular);

        public static void DrawScript(Graphics g, Rectangle clip, Triggerscript script, Selection sel, Selection hover, bool lod)
        {
            DrawBackground(g, clip, script);
            foreach (Trigger trigger in script.Triggers.Values)
            {
                Rectangle bounds = BoundsTrigger(trigger);
                if (!bounds.IntersectsWith(clip)) continue;

                DrawUnit(g, script, trigger, sel, hover, lod);
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
            DrawLogicBases(g, script, trigger, LogicSlot.Condition, sel, drawDetail);
            DrawLogicBases(g, script, trigger, LogicSlot.EffectTrue, sel, drawDetail);
            DrawLogicBases(g, script, trigger, LogicSlot.EffectFalse, sel, drawDetail);
        }
        private static void DrawBackdrop(Graphics g, Trigger trigger, Trigger selectedTrigger, int selectedLogic, bool detail)
        {
            Rectangle bounds = BoundsTriggerMargin(trigger);
            g.FillRectangle(new SolidBrush(UnitColor), bounds);

            g.DrawRectangle(new Pen(trigger.Active ? TriggerActiveColor : TrimColor), bounds);

            if (selectedTrigger == trigger && selectedLogic == -1)
            {
                Rectangle sel = bounds;
                sel.Inflate(1, 1);
                g.DrawRectangle(new Pen(Color.White), sel);
            }

            if (!detail)
            {
                bounds.Inflate(50, 50);
                g.DrawString(trigger.Name, HugeFont, new SolidBrush(TextColor), bounds, new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox,
                    Trimming = StringTrimming.EllipsisWord,
                });
            }
        }
        private static void DrawLogicBases(Graphics g, Triggerscript script, Trigger trigger, LogicSlot slot, Selection sel, bool detail)
        {
            if (!detail) return;

            IEnumerable<Logic> logics = Logics(trigger, slot);
            Color headerColor = slot == LogicSlot.Condition ? ConditionHeaderColor : EffectHeaderColor;

            for (int i = 0; i < logics.Count(); i++)
            {
                Logic cur = logics.ElementAt(i);
                Rectangle bounds = BoundsLogicBody(trigger, slot, i);

                //Draw background
                g.FillRectangle(new SolidBrush(BodyColor), bounds);
                g.FillRectangle(new SolidBrush(headerColor), bounds.X, bounds.Y, bounds.Width, HeaderHeight);

                //Draw outline
                //only draw trim outlines if quality is set to high.
                if (detail)
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
                if (sel.TriggerId == trigger
                    && sel.LogicType == slot
                    && sel.LogicIndex == i)
                {
                    g.DrawRectangle(new Pen(Color.White, Margin), bounds);
                }

                //Draw param slots, only if quality is set to high.
                if (detail)
                {
                    int paramIndex = 0;
                    foreach (var (sigid, param) in LogicParamInfos(SlotType(slot), cur.DBID, cur.Version))
                    {
                        //name
                        Rectangle varNameBounds = BoundsParamName(trigger, slot, i, paramIndex);

                        g.DrawString(
                            param.Name + " [" + param.Type + "]",
                            TextFont, new SolidBrush(TextColor),
                            varNameBounds,
                            new StringFormat() { LineAlignment = StringAlignment.Center }
                        );

                        //value
                        var var = cur.Params[sigid];

                        Rectangle varValBounds = BoundsParamValue(trigger, slot, i, paramIndex);
                        g.FillRectangle(new SolidBrush(TrimColor), varValBounds);

                        if (var == null)
                        {
                            paramIndex++;
                            continue;
                        }

                        //outline var value box when selected.
                        if (sel.TriggerId == trigger
                            && sel.LogicType == slot
                            && sel.LogicIndex == i
                            && sel.VarSigId == sigid)
                        {
                            g.DrawRectangle(new Pen(Color.White, .125f), varValBounds);
                        }

                        string varValStr = var.Name;
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
                if (detail)
                {
                    string title = LogicName(cur.Type, cur.DBID);
                    if (cur.Version != -1)
                    {
                        title += " [v" + cur.Version + "]";
                    }
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
                    if (detail)
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
            if (!detail) return;
            //TODO: offset nightmare. yeesh...

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

            lbounds = BoundsLogicSlot(trigger, LogicSlot.Condition);
            lbounds.Height = HeaderHeight;
            lbounds.Y = bounds.Y;
            lbounds.Inflate(0, -2);
            g.FillRectangle(new SolidBrush(ConditionHeaderColor), lbounds);
            g.DrawRectangle(new Pen(TrimColor, Margin), lbounds);
            string cndAndStr = trigger.ConditionsAreAND ? "All" : "Any";
            string cndStr = trigger.ConditionalTrigger ? "If " + cndAndStr : "Await " + cndAndStr;
            lbounds.Inflate(-3, 0);
            if (detail) g.DrawString(cndStr, TextFont, new SolidBrush(TextColor), lbounds, new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
            });

            lbounds = BoundsLogicSlot(trigger, LogicSlot.EffectTrue);
            lbounds.Height = HeaderHeight;
            lbounds.Y = bounds.Y;
            lbounds.Inflate(0, -2);
            g.FillRectangle(new SolidBrush(EffectHeaderColor), lbounds);
            g.DrawRectangle(new Pen(TrimColor, Margin), lbounds);
            string eftStr = trigger.ConditionalTrigger ? "Then" : "Do";
            lbounds.Inflate(-3, 0);
            if (detail) g.DrawString(eftStr, TextFont, new SolidBrush(TextColor), lbounds, new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
            });

            lbounds = BoundsLogicSlot(trigger, LogicSlot.EffectFalse);
            lbounds.Height = HeaderHeight;
            lbounds.Y = bounds.Y;
            lbounds.Inflate(0, -2);
            g.FillRectangle(new SolidBrush(EffectHeaderColor), lbounds);
            g.DrawRectangle(new Pen(TrimColor, Margin), lbounds);
            string effStr = trigger.ConditionalTrigger ? "Else" : "UNREACHABLE";
            lbounds.Inflate(-3, 0);
            if (detail) g.DrawString(effStr, TextFont, new SolidBrush(TextColor), lbounds, new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
            });
        }
    }
}

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

namespace Foundry.HW1.Triggerscript
{
    public static class EditorRendererWinForms
    {
        public static void DrawScript(Graphics g, Rectangle clip, Triggerscript script, Selection sel, Selection hover)
        {
            DrawBackground(g, clip, script);

            //draw the triggers in a reverse order to match the top-down selection logic.
            for (int i = script.Triggers.Count - 1; i >= 0; i--)
            {
                //TODO: clip culling
                Trigger trigger = script.Triggers.Values.ElementAt(i);
                DrawTrigger(g, trigger, sel);
                DrawLogicBases(g, script, trigger, TriggerLogicSlot.Condition, sel);
                DrawLogicBases(g, script, trigger, TriggerLogicSlot.EffectTrue, sel);
                DrawLogicBases(g, script, trigger, TriggerLogicSlot.EffectFalse, sel);
                DrawTriggerContainer(g, trigger, sel);

                //DrawTriggerDebug(g, trigger, sel);
            }
        }

        public static void DrawBackground(Graphics g, Rectangle clip, Triggerscript script)
        {
            Rectangle bounds = ScriptBounds(script);
            bounds.Inflate(100, 100);
            g.DrawRectangle(new Pen(Color.Red, 4), bounds);
        }

        private static void DrawTrigger(Graphics g, Trigger trigger, Selection sel)
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

        private static void DrawLogicBases(Graphics g, Triggerscript script, Trigger trigger, TriggerLogicSlot type, Selection sel)
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

                //Draw param slots
                int paramIndex = 0;
                foreach (var (sigid, param) in cur.StaticParamInfo)
                {
                    //name
                    Rectangle varNameBounds = ParamNameBounds(trigger, type, i, paramIndex);

                    g.DrawString(
                        param.Name + " [" + param.Type + "]",
                        TextFont, new SolidBrush(TextColor),
                        varNameBounds,
                        new StringFormat() { LineAlignment = StringAlignment.Center});

                    //value
                    int varId = cur.GetValueOfParam(sigid);
                    string varValStr = script.TriggerVars[varId].Name;
                    if (varValStr == "") varValStr = "NO NAME";

                    Rectangle varValBounds = ParamValBounds(trigger, type, i, paramIndex);
                    g.FillRectangle(new SolidBrush(TrimColor), varValBounds);
                    //shrink it by a margin for the string
                    varValBounds.Inflate(-Margin, 0);
                    g.DrawString(
                        varValStr,
                        TextFont,
                        new SolidBrush(TextColor),
                        varValBounds,
                        new StringFormat() { LineAlignment = StringAlignment.Center});

                    paramIndex++;
                }

                //Draw logic title
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
                
                //Draw comment
                if (logics.ElementAt(i).Comment != "")
                {
                    g.FillRectangle(new SolidBrush(BodyColor),
                        bounds.X,
                        bounds.Y - CommentHeight - LogicSpacing - FooterHeight,
                        bounds.Width,
                        CommentHeight);

                    g.DrawString(logics.ElementAt(i).Comment, TextFont, new SolidBrush(TextColor), 
                        new Rectangle(
                            bounds.X + Margin,
                            bounds.Y + Margin - CommentHeight - LogicSpacing - FooterHeight,
                            bounds.Width - (Margin * 2),
                            CommentHeight - (Margin * 2)));

                    g.FillRectangle(new SolidBrush(headerColor),
                        bounds.X,
                        bounds.Y - LogicSpacing - FooterHeight,
                        bounds.Width,
                        FooterHeight);

                    g.DrawRectangle(new Pen(TrimColor, .25f),
                        bounds.X,
                        bounds.Y - CommentHeight - LogicSpacing - FooterHeight,
                        bounds.Width,
                        CommentHeight + FooterHeight);

                }



            }

        }

        private static void DrawTriggerContainer(Graphics g, Trigger trigger, Selection sel)
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

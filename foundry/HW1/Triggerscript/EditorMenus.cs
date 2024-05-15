using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Foundry.HW1.Triggerscript
{
    public static class EditorMenusWinForms
    {
        public static void ShowOptionsForSelection(Triggerscript script, Selection selection, Point point)
        {
            if (selection.TriggerId != -1)
            {
                if (selection.LogicIndex == -1)
                {
                    ShowTriggerOptionsMenu(script.Triggers[selection.TriggerId], point);
                }
                else
                {
                    if (selection.LogicType == TriggerLogicSlot.Condition)
                    {
                        ShowConditionOptionsMenu(script.Triggers[selection.TriggerId].Conditions.ElementAt(selection.LogicIndex), point);
                    }
                    else if (selection.LogicType == TriggerLogicSlot.EffectTrue)
                    {
                        ShowEffectOptionsMenu(script.Triggers[selection.TriggerId].TriggerEffectsOnTrue.ElementAt(selection.LogicIndex), point);
                    }
                    else if (selection.LogicType == TriggerLogicSlot.EffectFalse)
                    {
                        ShowEffectOptionsMenu(script.Triggers[selection.TriggerId].TriggerEffectsOnFalse.ElementAt(selection.LogicIndex), point);
                    }
                }
            }
        }
        public static void ShowTriggerOptionsMenu(Trigger trigger, Point point)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Closing += (s, e) =>
            {
                //keep the menu open if something was clicked.
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                {
                    e.Cancel = true;
                }
            };


            //Info
            ToolStripLabel info = new ToolStripLabel("Trigger");
            menu.Items.Add(info);
            ToolStripSeparator infoSep = new ToolStripSeparator();
            menu.Items.Add(infoSep);


            //Name text box
            ToolStripLabel nameLabel = new ToolStripLabel("Name:");
            menu.Items.Add(nameLabel);
            ToolStripTextBox name = new ToolStripTextBox();
            name.Text = trigger.Name;
            name.AutoSize = false;
            name.Multiline = true;
            name.BorderStyle = BorderStyle.FixedSingle;
            name.Size = new Size(200, 45);
            name.AcceptsReturn = false;
            name.AcceptsTab = false;
            name.TextChanged += (s, e) =>
            {
                trigger.Name = name.Text;
            };
            menu.Items.Add(name);
            ToolStripSeparator nameSeparator = new ToolStripSeparator();
            menu.Items.Add(nameSeparator);

            //Active button
            Image activeOnImg = Properties.Resources.bullet_green;
            Image activeOffImg = Properties.Resources.bullet_black;

            ToolStripMenuItem active = new ToolStripMenuItem();
            active.Text = "Active";
            active.Image = trigger.Active ? activeOnImg : activeOffImg;
            active.Click += (s, e) =>
            {
                trigger.Active = !trigger.Active;
                active.Image = trigger.Active ? activeOnImg : activeOffImg;
            };
            menu.Items.Add(active);


            //Conditional button
            Image conditionalOffImg = Properties.Resources.bullet_black;
            Image conditionalOnImg = Properties.Resources.bullet_orange;

            ToolStripMenuItem conditional = new ToolStripMenuItem();
            conditional.Text = "Conditional";
            conditional.Image = trigger.ConditionalTrigger ? conditionalOnImg : conditionalOffImg;
            conditional.Click += (s, e) =>
            {
                trigger.ConditionalTrigger = !trigger.ConditionalTrigger;
                conditional.Image = trigger.ConditionalTrigger ? conditionalOnImg : conditionalOffImg;
            };
            menu.Items.Add(conditional);


            //Show
            menu.Show(point);
        }
        public static void ShowConditionOptionsMenu(Condition condition, Point point)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Closing += (s, e) =>
            {
                //keep the menu open if something was clicked.
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                {
                    e.Cancel = true;
                }
            };


            //Info
            ToolStripLabel conditionLabel = new ToolStripLabel("Condition");
            menu.Items.Add(conditionLabel);
            ToolStripLabel info = new ToolStripLabel(string.Format("{0} [v{1}]", condition.TypeName, condition.Version)/*, Properties.Resources.information*/);
            menu.Items.Add(info);
            ToolStripSeparator infoSep = new ToolStripSeparator();
            menu.Items.Add(infoSep);


            //Invert
            Image invertOnImg = Properties.Resources.bullet_green;
            Image invertOffImg = Properties.Resources.bullet_black;

            ToolStripMenuItem invert = new ToolStripMenuItem();
            invert.Text = "Inverted";
            invert.Image = condition.Invert ? invertOnImg : invertOffImg;
            invert.Click += (s, e) =>
            {
                condition.Invert = !condition.Invert;
                invert.Image = condition.Invert ? invertOnImg : invertOffImg;
            };
            menu.Items.Add(invert);


            //Comment
            ToolStripSeparator commentSeparator = new ToolStripSeparator();
            menu.Items.Add(commentSeparator);
            ToolStripLabel commentLabel = new ToolStripLabel("Comment:");
            menu.Items.Add(commentLabel);
            ToolStripTextBox comment = new ToolStripTextBox();
            comment.Text = condition.Comment;
            comment.AutoSize = false;
            comment.Multiline = true;
            comment.BorderStyle = BorderStyle.FixedSingle;
            comment.Size = new Size(200, 70);
            comment.AcceptsReturn = false;
            comment.AcceptsTab = false;
            comment.TextChanged += (s, e) =>
            {
                condition.Comment = comment.Text;
            };
            menu.Items.Add(comment);


            menu.Show(point);
        }
        public static void ShowEffectOptionsMenu(Effect effect, Point point)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Closing += (s, e) =>
            {
                //keep the menu open if something was clicked.
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                {
                    e.Cancel = true;
                }
            };


            //Info
            ToolStripLabel effLabel = new ToolStripLabel("Effect");
            menu.Items.Add(effLabel);
            ToolStripLabel info = new ToolStripLabel(string.Format("{0} [v{1}]", effect.TypeName, effect.Version)/*, Properties.Resources.information*/);
            menu.Items.Add(info);


            //Comment
            ToolStripSeparator commentSeparator = new ToolStripSeparator();
            menu.Items.Add(commentSeparator);
            ToolStripLabel commentLabel = new ToolStripLabel("Comment:");
            menu.Items.Add(commentLabel);
            ToolStripTextBox comment = new ToolStripTextBox();
            comment.Text = effect.Comment;
            comment.AutoSize = false;
            comment.Multiline = true;
            comment.BorderStyle = BorderStyle.FixedSingle;
            comment.Size = new Size(200, 70);
            comment.AcceptsReturn = false;
            comment.AcceptsTab = false;
            comment.TextChanged += (s, e) =>
            {
                effect.Comment = comment.Text;
            };
            menu.Items.Add(comment);


            menu.Show(point);
        }
    }
}

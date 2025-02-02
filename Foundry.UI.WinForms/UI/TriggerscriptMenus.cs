using Chef.HW1.Script;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Chef.HW1.Script.TriggerscriptHelpers;

namespace Chef.Win.UI
{
    public static class TriggerscriptMenus
    {
        //Menus
        public static void ShowTriggerOptionsMenu(Trigger trigger, Point point, EventHandler onEdit = null)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Closing += (s, e) =>
            {
                //keep the menu open if something was clicked.
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                {
                    //e.Cancel = true;
                }
            };
            menu.MouseHover += (s, e) => { menu.Focus(); };
            menu.Items.AddRange(TriggerOptionItems(trigger, onEdit).ToArray());
            menu.Show(point);
        }
        public static void ShowConditionOptionsMenu(Condition condition, Point point, EventHandler onEdit = null)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Closing += (s, e) =>
            {
                //keep the menu open if something was clicked.
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                {
                    //e.Cancel = true;
                }
            };
            menu.MouseHover += (s, e) => { menu.Focus(); };
            menu.Items.AddRange(ConditionOptionItems(condition, onEdit).ToArray());
            menu.Show(point);
        }
        public static void ShowEffectOptionsMenu(Effect effect, Point point, EventHandler onEdit = null)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Closing += (s, e) =>
            {
                //keep the menu open if something was clicked.
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                {
                    //e.Cancel = true;
                }
            };
            menu.MouseHover += (s, e) => { menu.Focus(); };
            menu.Items.AddRange(EffectOptionItems(effect, onEdit).ToArray());
            menu.Show(point);
        }
        public static void ShowVarOptionsMenu(Triggerscript script, Logic logic, int sigid, Point point, EventHandler onEdit = null)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Closing += (s, e) =>
            {
                //keep the menu open if something was clicked.
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                {
                    //e.Cancel = true;
                }
            };

            menu.Items.AddRange(VarOptionItems(script, logic, sigid, onEdit).ToArray());

            menu.Show(point);
        }
        public static void ShowLogicAddMenu(Trigger trigger, LogicSlot slot, int logicIndex, Point point, EventHandler onEdit = null)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Closing += (s, e) =>
            {
                //keep the menu open if something was clicked.
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                {
                    //e.Cancel = true;
                }
            };

            menu.MouseHover += (s, e) => { menu.Focus(); };
            menu.Items.AddRange(LogicAddItems(trigger, slot, logicIndex, onEdit).ToArray());
            menu.Show(point);
        }

        //Items
        public static IEnumerable<ToolStripItem> TriggerOptionItems(Trigger trigger, EventHandler onEdit = null)
        {
            List<ToolStripItem> items = new List<ToolStripItem>();

            //Info
            ToolStripLabel info = new ToolStripLabel("Trigger");
            items.Add(info);
            ToolStripSeparator infoSep = new ToolStripSeparator();
            items.Add(infoSep);

            //Name text box
            ToolStripLabel nameLabel = new ToolStripLabel("Name:");
            items.Add(nameLabel);
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
                onEdit?.Invoke(name, EventArgs.Empty);
            };
            items.Add(name);
            ToolStripSeparator nameSeparator = new ToolStripSeparator();
            items.Add(nameSeparator);

            //Active button
            Image activeOnImg = null;//Properties.Resources.bullet_green;
            Image activeOffImg = null;//Properties.Resources.bullet_black;

            ToolStripMenuItem active = new ToolStripMenuItem();
            active.Text = "Active";
            active.Image = trigger.Active ? activeOnImg : activeOffImg;
            active.Click += (s, e) =>
            {
                trigger.Active = !trigger.Active;
                active.Image = trigger.Active ? activeOnImg : activeOffImg;
                onEdit?.Invoke(active, EventArgs.Empty);
            };
            items.Add(active);

            //Conditional button
            Image conditionalOffImg = null;// Properties.Resources.bullet_black;
            Image conditionalOnImg = null;//Properties.Resources.bullet_orange;

            ToolStripMenuItem conditional = new ToolStripMenuItem();
            conditional.Text = "Conditional";
            conditional.Image = trigger.ConditionalTrigger ? conditionalOnImg : conditionalOffImg;
            conditional.Click += (s, e) =>
            {
                trigger.ConditionalTrigger = !trigger.ConditionalTrigger;
                conditional.Image = trigger.ConditionalTrigger ? conditionalOnImg : conditionalOffImg;
                onEdit?.Invoke(conditional, EventArgs.Empty);
            };
            items.Add(conditional);

            return items;
        }
        public static IEnumerable<ToolStripItem> ConditionOptionItems(Condition condition, EventHandler onEdit = null)
        {
            List<ToolStripItem> items = new List<ToolStripItem>();

            //Info
            ToolStripLabel conditionLabel = new ToolStripLabel("Condition");
            items.Add(conditionLabel);
            ToolStripLabel info = new ToolStripLabel(
                string.Format("{0} [v{1}]", LogicName(LogicType.Condition ,condition.DBID), condition.Version)/*, Properties.Resources.information*/);
            items.Add(info);
            ToolStripSeparator infoSep = new ToolStripSeparator();
            items.Add(infoSep);


            //Invert
            Image invertOnImg = null;// Properties.Resources.bullet_green;
            Image invertOffImg = null;//Properties.Resources.bullet_black;

            ToolStripMenuItem invert = new ToolStripMenuItem();
            invert.Text = "Inverted";
            invert.Image = condition.Invert ? invertOnImg : invertOffImg;
            invert.Click += (s, e) =>
            {
                condition.Invert = !condition.Invert;
                invert.Image = condition.Invert ? invertOnImg : invertOffImg;
                onEdit?.Invoke(invert, EventArgs.Empty);
            };
            items.Add(invert);


            //Comment
            ToolStripSeparator commentSeparator = new ToolStripSeparator();
            items.Add(commentSeparator);
            ToolStripLabel commentLabel = new ToolStripLabel("Comment:");
            items.Add(commentLabel);
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
                onEdit?.Invoke(condition, EventArgs.Empty);
            };
            items.Add(comment);

            return items;
        }
        public static IEnumerable<ToolStripItem> EffectOptionItems(Effect effect, EventHandler onEdit = null)
        {
            List<ToolStripItem> items = new List<ToolStripItem>();

            //Info
            ToolStripLabel effLabel = new ToolStripLabel("Effect");
            items.Add(effLabel);
            ToolStripLabel info = new ToolStripLabel(
                string.Format("{0} [v{1}]", LogicName(LogicType.Effect, effect.DBID), effect.Version)/*, Properties.Resources.information*/);
            items.Add(info);

            //Comment
            ToolStripSeparator commentSeparator = new ToolStripSeparator();
            items.Add(commentSeparator);
            ToolStripLabel commentLabel = new ToolStripLabel("Comment:");
            items.Add(commentLabel);
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
                onEdit?.Invoke(effect, EventArgs.Empty);
            };
            items.Add(comment);
            items.Add(commentSeparator);

            return items;
        }
        public static IEnumerable<ToolStripItem> VarOptionItems(Triggerscript script, Logic logic, int sigid, EventHandler onEdit = null)
        {
            List<ToolStripItem> items = new List<ToolStripItem>();

            var paramInfo = LogicParamInfos(logic.Type, logic.DBID, logic.Version);
            if (!paramInfo.ContainsKey(sigid))
                return items;

            ////Info
            items.Add(new ToolStripLabel(paramInfo[sigid].Name + " [" + paramInfo[sigid].Type + "]"));
            items.Add(new ToolStripSeparator());

            ////Name
            var nameLabel = new ToolStripLabel("Name:");
            ToolStripTextBox name = new ToolStripTextBox();
            var valueLabel = new ToolStripLabel("Value:");
            ToolStripTextBox value = new ToolStripTextBox();
            name.BorderStyle = BorderStyle.FixedSingle;
            name.AutoSize = true;
            name.Size = new Size(160, 0);
            name.TextChanged += (s, e) =>
            {
                logic.Params[sigid] = name.Text;
                //does this name have a value? if so, update our value text box to reflect it.
                if (script.Constants.ContainsKey(paramInfo[sigid].Type)
                    && script.Constants[paramInfo[sigid].Type].ContainsKey(name.Text))
                {
                    //value.ReadOnly = false;
                    value.Text = script.Constants[paramInfo[sigid].Type][name.Text];
                }
                //if not, clear value box.
                else
                {
                    //value.ReadOnly = true;
                    value.Text = "";
                }
                onEdit?.Invoke(name, EventArgs.Empty);
            };

            value.BorderStyle = BorderStyle.FixedSingle;
            value.AutoSize = true;
            value.Size = new Size(160, 0);
            value.TextChanged += (s, e) =>
            {
                //does this name have a value? if so, update the value in the script.
                if (script.Constants[paramInfo[sigid].Type].ContainsKey(name.Text))
                {
                    script.Constants[paramInfo[sigid].Type][name.Text] = value.Text;
                }
                onEdit?.Invoke(name, EventArgs.Empty);
            };

            //set name text last so value can update properly.
            if (!logic.Params.ContainsKey(sigid))
                name.Text = "";
            else
                name.Text = logic.Params[sigid];

            items.Add(nameLabel);
            items.Add(name);
            items.Add(valueLabel);
            items.Add(value);

            ////Value
            //items.Add(new ToolStripLabel("Value:"));
            //ToolStripTextBox val = new ToolStripTextBox();
            ////val.Text = var.Value;
            //val.BorderStyle = BorderStyle.FixedSingle;
            //val.TextChanged += (s, e) =>
            //{
            //    //var.Value = val.Text;
            //    onEdit?.Invoke(name, EventArgs.Empty);
            //};
            //items.Add(val);

            return items;
        }
        public static IEnumerable<ToolStripItem> LogicAddItems(Trigger trigger, LogicSlot slot, int index, EventHandler onEdit = null)
        {
            List<ToolStripItem> items = new List<ToolStripItem>();

            ToolStripLabel label = new ToolStripLabel("Add...");
            items.Add(new ToolStripLabel("Add..."));
            items.Add(new ToolStripSeparator());

            LogicType t = slot == LogicSlot.Condition ? LogicType.Condition : LogicType.Effect;

            Dictionary<string, ToolStripMenuItem> categories = new Dictionary<string, ToolStripMenuItem>();

            foreach (var i in LogicIds(t))
            {
                ToolStripMenuItem b = new ToolStripMenuItem(LogicName(t, i));
                b.Click += (s, e) =>
                {
                    if (slot == LogicSlot.Condition)
                    {
                        Condition cnd = new Condition()
                        {
                            DBID = i,
                            Version = LogicVersions(t, i).First()
                        };
                        trigger.Conditions.Insert(index, cnd);
                    }
                    else
                    {
                        Effect eff = new Effect()
                        {
                            DBID = i,
                            Version = LogicVersions(t, i).First()
                        };
                        if (slot == LogicSlot.EffectTrue)
                            trigger.TriggerEffectsOnTrue.Insert(index, eff);
                        if (slot == LogicSlot.EffectFalse)
                            trigger.TriggerEffectsOnFalse.Insert(index, eff);
                    }

                    onEdit?.Invoke(b, EventArgs.Empty);
                };

                //category menu items
                string cat = "";
                ToolStripMenuItem last = null;
                foreach (string c in LogicCategory(t, i).Split("|"))
                {
                    if (c == "") break;

                    cat += c;
                    if (!categories.ContainsKey(cat))
                    {
                        categories.Add(cat, new ToolStripMenuItem(c));

                        if (last != null)
                            last.DropDownItems.Add(categories[cat]);
                        else
                            items.Add(categories[cat]);
                    }

                    last = categories[cat];
                }

                if (last != null)
                    last.DropDownItems.Add(b);
                else
                    items.Add(b);
            }
            return items;
        }
    }
}

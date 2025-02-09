using Aga.Controls;
using BrightIdeasSoftware;
using Chef.HW1;
using Chef.HW1.Script;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
        public static void ShowVarOptionsMenu(Triggerscript script, Logic logic, int sigid, Point point, AssetCache cache, EventHandler onEdit = null)
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

            menu.Items.Add(new ToolStripControlHost(new EditorScriptLogicParam(script, logic, sigid, cache)));

            //menu.Items.AddRange(VarOptionItems(script, logic, sigid, values, onEdit).ToArray());

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
        public static IEnumerable<ToolStripItem> VarOptionItems(Triggerscript script, Logic logic, int sigid, IEnumerable<string> values, EventHandler onEdit = null)
        {
            List<ToolStripItem> items = new List<ToolStripItem>();

            var pinfos = LogicParamInfos(logic.Type, logic.DBID, logic.Version);
            if (!pinfos.ContainsKey(sigid))
                return items;
            var pinfo = pinfos[sigid];
            var ptype = pinfo.Type;

            if (!script.Constants.ContainsKey(ptype))
                script.Constants[ptype] = new Dictionary<string, string>();
            var pvars = script.Constants[ptype];
            var pname = logic.Params[sigid];

            //if (pname != null && pvars.ContainsKey(pname))

            items.Add(new ToolStripLabel(pinfo.Name + " [" + pinfo.Type + "]"));
            items.Add(new ToolStripSeparator());

            items.Add(new ToolStripLabel("Variable:"));
            var varbox = new ToolStripComboBox();
            varbox.Items.AddRange(pvars.Keys.ToArray());
            varbox.ComboBox.SelectedItem = pname;
            varbox.DropDownStyle = ComboBoxStyle.DropDownList;
            varbox.FlatStyle = FlatStyle.Standard;
            varbox.AutoSize = false;
            varbox.Size = new Size(200, 0);
            varbox.Margin = new Padding(2, 2, 2, 2);
            items.Add(varbox);

            items.Add(new ToolStripLabel("Value:") { Margin = new Padding(2, 6, 2, 0)});
            var value = VarValueItem(script, logic, sigid, values, onEdit);
            value.Margin = new Padding(2, 2, 2, 2);
            items.Add(value);

            varbox.TextChanged += (s, e) =>
            {
                logic.Params[sigid] = varbox.Text;
                items.Remove(value);
                value = VarValueItem(script, logic, sigid, values, onEdit);
                value.Margin = new Padding(2, 2, 2, 2);
                items.Add(value);
                onEdit?.Invoke(varbox, EventArgs.Empty);
            };

            return items;
        }

        private static IEnumerable<string> VarTypeHeaders(VarType type)
        {
            switch(type)
            {
                case VarType.Vector:
                    return ["x", "y", "z"];
                default:
                    return [type.ToString()];
            }
        }
        private static char VarTypeColumnSeparator(VarType type)
        {
            switch (type)
            {
                case VarType.Vector:
                    return ',';
                default:
                    return '\0';
            }
        }
        private static char VarTypeListSeparator(VarType type)
        {
            switch (type)
            {
                case VarType.Vector:
                    return '|';
                default:
                    return ',';
            }
        }
        private static string VarTypeDefault(VarType type)
        {
            switch(type)
            {
                case VarType.Vector:
                    return "0.0,0.0,0.0";
                default:
                    return "";
            }
        }

        public static ToolStripItem VarValueItem(Triggerscript script, Logic logic, int sigid, IEnumerable<string> values, EventHandler onEdit = null)
        {
            var pi = LogicParamInfos(logic.Type, logic.DBID, logic.Version);
            if (!pi.ContainsKey(sigid))
                return new ToolStripMenuItem();

            if (!logic.Params.ContainsKey(sigid))
                return new ToolStripMenuItem();

            VarType ptype = pi[sigid].Type;

            if (!script.Constants.ContainsKey(ptype))
                script.Constants[ptype] = new Dictionary<string, string>();

            string pname = logic.Params[sigid];
            if (pname == null) return new ToolStripMenuItem();

            string pval = VarTypeDefault(ptype);
            if (script.Constants[ptype].ContainsKey(pname))
                pval = script.Constants[ptype][pname];

            if (pval == "") pval = VarTypeDefault(ptype);

            var pheaders = VarTypeHeaders(ptype);
            var pcolsep = VarTypeColumnSeparator(ptype);
            var prowsep = VarTypeListSeparator(ptype);

            bool islist = VarTypeIsList(ptype);


            string[] rows = islist ? pval.Split(prowsep) : [pval];
            int curIndex = 0;
            string[] cols = rows[curIndex].Split(pcolsep);

            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Height = 4;
            panel.ColumnStyles.Add(new ColumnStyle() { SizeType = SizeType.Absolute, Width = 80 });
            panel.ColumnStyles.Add(new ColumnStyle() { SizeType = SizeType.Absolute, Width = 120 });
            for (int i = 0; i < pheaders.Count(); i++)
            {
                panel.RowStyles.Add(new RowStyle() { SizeType = SizeType.Absolute, Height = 24 });

                //Control valueField;
                //switch(VarTypeFormat(ptype))
                //{
                //    case VarFormat.Float:
                //        valueField = new NumericUpDown();
                //        ((NumericUpDown)valueField).DecimalPlaces = 3;
                //        float fval = 0;
                //        float.TryParse(cols[i], out fval);
                //        ((NumericUpDown)valueField).Value = (decimal)fval;
                //        break;
                //    case VarFormat.Integer:
                //        valueField = new NumericUpDown();
                //        ((NumericUpDown)valueField).DecimalPlaces = 0;
                //        int ival = 0;
                //        int.TryParse(cols[i], out ival);
                //        ((NumericUpDown)valueField).Value = ival;
                //        break;
                //    case VarFormat.Enum:
                //        valueField = new ComboBox();
                //        ((ComboBox)valueField).Items.AddRange(values.ToArray());
                //        if (values.Contains(cols[i]))
                //            ((ComboBox)valueField).SelectedItem = cols[i];
                //        else
                //            ((ComboBox)valueField).Items.Add(cols[i]);
                //            ((ComboBox)valueField).SelectedItem = cols[i];
                //        break;
                //    default:
                //        return new ToolStripMenuItem();
                //}
                Label valueField = new Label();
                valueField.AutoSize = false;
                valueField.Text = cols[i];
                valueField.TextAlign = ContentAlignment.MiddleLeft;
                valueField.Dock = DockStyle.Fill;
                panel.Controls.Add(valueField, 1, i);

                var label = new Label();
                label.AutoSize = false;
                label.Text = pheaders.ElementAt(i);
                label.TextAlign = ContentAlignment.MiddleRight;
                label.Dock = DockStyle.Fill;
                panel.Controls.Add(label, 0, i);

                panel.Height += 24;
            }

            panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            var ret = new ToolStripControlHost(panel);
            ret.AutoSize = false;
            return ret;
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

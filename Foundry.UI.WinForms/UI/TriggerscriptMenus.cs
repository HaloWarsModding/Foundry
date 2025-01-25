using Chef.HW1.Script;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
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
        public static void ShowVarOptionsMenu(Var var, Point point, EventHandler onEdit = null)
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
            menu.Items.AddRange(VarOptionItems(var, 
                (s, e) =>
            {
                onEdit?.Invoke(menu, EventArgs.Empty);
            },
                (s, e) =>
            {
                onEdit?.Invoke(menu, EventArgs.Empty);
            }
            ).ToArray());
            menu.Show(point);
        }
        public static void ShowSetVarMenu(Triggerscript script, Logic logic, int sigid, Point point, EventHandler onEdit = null)
        {
            var spi = LogicParamInfos(logic.Type, logic.DBID, logic.Version);

            if (!spi.ContainsKey(sigid)) return;

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.MouseHover += (s, e) => { menu.Focus(); };
            var paramInfo = spi[sigid];

            //Info
            menu.Items.Add(new ToolStripLabel(paramInfo.Name + " [" + paramInfo.Type + "]"));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(VarNewItem(script, logic, sigid, (s, e) =>
            {
                onEdit?.Invoke(menu, EventArgs.Empty);

            }));
            menu.Items.Add(VarSetItem(script, logic, sigid, (s, e) =>
            {
                onEdit?.Invoke(menu, EventArgs.Empty);
            }));

            menu.Show(point);
        }
        public static void ShowVarList(Triggerscript script, Point point, EventHandler onEdit = null)
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            menu.Items.AddRange(VarListItems(script, onEdit).ToArray());

            menu.Show(point);
        }
        public static void ShowLogicAddMenu(Triggerscript script, int triggerId, TriggerLogicSlot slot, int logicIndex, Point point, EventHandler onEdit = null)
        {
            Trigger trigger = script.Triggers[triggerId];
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
            menu.Items.Add(LogicAddItem(trigger, slot, logicIndex, onEdit));
            menu.Show(point);
        }

        //Items
        public static ToolStripItem VarSetItem(Triggerscript script, Logic logic, int sigid, EventHandler onEdit = null)
        {
            var paramInfos = LogicParamInfos(logic.Type, logic.DBID, logic.Version);
            var paramInfo = paramInfos[sigid];
            List<Var> selectionSet = Variables(script, paramInfo.Type).ToList();

            int curIndex = 0;
            if (logic.Params.ContainsKey(sigid) && logic.Params[sigid] != null)
            {
                curIndex = selectionSet.IndexOf(logic.Params[sigid]) + 1; //+1 because NULL is index 0.
            }

            ToolStripComboBox cb = new ToolStripComboBox();
            cb.Items.Add("NULL");
            cb.Items.AddRange(selectionSet.ToArray());
            cb.SelectedIndex = curIndex;
            cb.ComboBox.DisplayMember = "Name";
            cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cb.AutoCompleteSource = AutoCompleteSource.ListItems;
            cb.SelectedIndexChanged += (s, e) =>
            {
                Var selVar = null;
                if (cb.SelectedIndex != 0)
                {
                    selVar = (Var)cb.Items[cb.SelectedIndex];
                }
                logic.Params[sigid] = selVar; //set the param value
                onEdit?.Invoke(cb, EventArgs.Empty);
            };
            cb.KeyDown += (s, e) =>
            {
                cb.DroppedDown = false;
            };

            return cb;
        }
        public static ToolStripItem VarNewItem(Triggerscript script, Logic logic, int sigid, EventHandler onEdit = null)
        {
            var paramInfos = LogicParamInfos(logic.Type, logic.DBID, logic.Version);
            var paramInfo = paramInfos[sigid];

            ToolStripMenuItem add = new ToolStripMenuItem();

            add.Text = "New...";
            add.Click += (s, e) =>
            {
                Var var = new Var()
                {
                    Name = "new" + paramInfo.Type,
                    Value = "",
                };

                logic.Params[sigid] = var; //set the param value

                onEdit?.Invoke(add, EventArgs.Empty);
            };

            return add;
        }
        public static ToolStripItem LogicAddItem(Trigger trigger, TriggerLogicSlot slot, int index, EventHandler onEdit = null)
        {
            ToolStripMenuItem root = new ToolStripMenuItem("Add...");
            LogicType t = slot == TriggerLogicSlot.Condition ? LogicType.Condition : LogicType.Effect;

            Dictionary<string, ToolStripMenuItem> categories = new Dictionary<string, ToolStripMenuItem>();

            foreach (var i in LogicIds(t))
            {
                ToolStripMenuItem b = new ToolStripMenuItem(LogicName(t, i));
                b.Click += (s, e) =>
                {
                    if (slot == TriggerLogicSlot.Condition)
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
                        if (slot == TriggerLogicSlot.EffectTrue)
                            trigger.TriggerEffectsOnTrue.Insert(index, eff);
                        if (slot == TriggerLogicSlot.EffectFalse)
                            trigger.TriggerEffectsOnFalse.Insert(index, eff);
                    }

                    onEdit?.Invoke(b, EventArgs.Empty);
                };

                //category menu items
                string cat = "";
                ToolStripMenuItem last = root;
                foreach (string c in LogicCategory(t, i).Split("|"))
                {
                    if (c == "") break;

                    cat += c;
                    if (!categories.ContainsKey(cat))
                    {
                        categories.Add(cat, new ToolStripMenuItem(c));
                        last.DropDownItems.Add(categories[cat]);
                    }

                    last = categories[cat];
                }

                last.DropDownItems.Add(b);
            }
            return root;
        }

        //Item collections
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
        public static IEnumerable<ToolStripItem> VarListItems(Triggerscript script, EventHandler onEdit = null)
        {
            Dictionary<VarType, ToolStripMenuItem> types = new Dictionary<VarType, ToolStripMenuItem>();

            foreach (var varTy in Enum.GetValues<VarType>())
            {
                if (VarTypeIsEnum(varTy)) continue;

                ToolStripMenuItem varRoot = new ToolStripMenuItem(varTy.ToString());
                types.Add(varTy, varRoot);

                ToolStripMenuItem varAdd = new ToolStripMenuItem("Add...");
                varAdd.Click += (s, e) =>
                {
                    onEdit?.Invoke(varAdd, EventArgs.Empty);
                };

                varRoot.DropDownItems.Add(varAdd);
                varRoot.DropDownItems.Add(new ToolStripSeparator());

                //foreach(var v in script.TriggerVars.Values.Where(tv => tv.Type == varTy && tv.IsNull == false))
                //{
                //    ToolStripMenuItem varCur = new ToolStripMenuItem(v.Name == "" ? "\"\"" : v.Name);

                //    varRoot.DropDownItems.Add(varCur);
                //}
            }

            //foreach (Var v in script.TriggerVars.Values.OrderBy(v => v.Name))
            //{
            //    if (v.IsNull) continue;

            //    if (!types.ContainsKey(v.Type))
            //    {
            //        var newType = new ToolStripMenuItem(v.Type.ToString());
            //        types.Add(v.Type, newType);
            //    }

            //    var curVar = new ToolStripMenuItem(v.Name);
            //    curVar.DropDownItems.AddRange(VarOptionItems(v, (s, e) =>
            //    {
            //        curVar.Text = e;
            //    }).ToArray());

            //    var curRoot = types[v.Type];
            //    curRoot.DropDownItems.Add(curVar);
            //}

            return types.Values;
        }
        public static IEnumerable<ToolStripItem> VarOptionItems(Var var, EventHandler<string> textChanged = null, EventHandler onEdit = null)
        {
            List<ToolStripItem> items = new List<ToolStripItem>();

            //Info
            ToolStripLabel varLabel = new ToolStripLabel(var.Type + " Variable");
            items.Add(varLabel);
            items.Add(new ToolStripSeparator());

            //Name
            items.Add(new ToolStripLabel("Name:"));
            ToolStripTextBox name = new ToolStripTextBox();
            name.Text = var.Name;
            name.BorderStyle = BorderStyle.FixedSingle;
            name.TextChanged += (s, e) =>
            {
                var.Name = name.Text;
                textChanged?.Invoke(name, name.Text);
                onEdit?.Invoke(name, EventArgs.Empty);
            };
            items.Add(name);

            //Value
            items.Add(new ToolStripLabel("Value:"));
            ToolStripTextBox val = new ToolStripTextBox();
            val.Text = var.Value;
            val.BorderStyle = BorderStyle.FixedSingle;
            val.TextChanged += (s, e) =>
            {
                var.Value = val.Text;
                onEdit?.Invoke(name, EventArgs.Empty);
            };
            items.Add(val);

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
    }
}

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
        public static void ShowTriggerOptionsMenu(Trigger trigger, Point point)
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
            menu.Items.AddRange(TriggerOptionItems(trigger).ToArray());
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
                    //e.Cancel = true;
                }
            };
            menu.MouseHover += (s, e) => { menu.Focus(); };
            menu.Items.AddRange(ConditionOptionItems(condition).ToArray());
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
                    //e.Cancel = true;
                }
            };
            menu.MouseHover += (s, e) => { menu.Focus(); };
            menu.Items.AddRange(EffectOptionItems(effect).ToArray());
            menu.Show(point);
        }
        public static void ShowVarOptionsMenu(Var var, Point point)
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
            menu.Items.AddRange(VarOptionItems(var, (s, e) =>
            {

            }).ToArray());
            menu.Show(point);
        }
        public static void ShowSetVarMenu(Triggerscript script, Trigger trigger, TriggerLogicSlot slot, int logic, int sigid, Point point)
        {
            Logic l = Logics(trigger, slot).ElementAt(logic);

            if (!l.StaticParamInfo.ContainsKey(sigid)) return;

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.MouseHover += (s, e) => { menu.Focus(); };
            var paramInfo = l.StaticParamInfo[sigid];
            int currentId = l.GetValueOfParam(sigid);

            //Info
            menu.Items.Add(new ToolStripLabel(paramInfo.Name + " [" + paramInfo.Type + "]"));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(VarAddItem(script, paramInfo.Type, trigger.ID, slot, logic, sigid));
            menu.Items.Add(VarSetItem(script, "Set...", currentId, paramInfo.Type,
                (s, e) =>
                {
                    l.SetValueOfParam(sigid, e);
                    menu.Close();
                }));

            menu.Show(point);
        }
        public static void ShowVarList(Triggerscript script, Point point)
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            menu.Items.AddRange(VarListItems(script).ToArray());

            menu.Show(point);
        }
        public static void ShowLogicAddMenu(Triggerscript script, int triggerId, TriggerLogicSlot slot, int logicIndex, Point point)
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
            menu.Items.Add(LogicAddItem(trigger, slot, logicIndex));
            menu.Show(point);
        }

        //Items
        public static ToolStripItem VarSetItem(Triggerscript script, string text, int initialValue, VarType type, EventHandler<int> varClicked = null)
        {
            List<Var> selectionSet = script.TriggerVars.Values.Where(v => !v.IsNull && v.Type == type).ToList();
            selectionSet.Sort((l, r) =>
            {
                return l.Name.CompareTo(r.Name);
            });
            Var nullVar = script.TriggerVars[GetOrAddNullVar(script, type)];
            selectionSet.Insert(0, nullVar);

            ToolStripComboBox cb = new ToolStripComboBox();

            cb.Items.AddRange(selectionSet.ToArray());
            cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cb.AutoCompleteSource = AutoCompleteSource.ListItems;

            Var curVar = script.TriggerVars[initialValue];
            cb.SelectedIndex = selectionSet.IndexOf(curVar);

            cb.SelectedIndexChanged += (s, e) =>
            {
                varClicked?.Invoke(cb, ((Var)cb.Items[cb.SelectedIndex]).ID);
            };
            cb.KeyDown += (s, e) =>
            {
                cb.DroppedDown = false;
            };

            return cb;
        }
        public static ToolStripItem VarAddItem(Triggerscript script, VarType type, int setTrigger = -1, TriggerLogicSlot setSlot = TriggerLogicSlot.Condition, int setLogic = -1, int setVar = -1)
        {
            ToolStripMenuItem add = new ToolStripMenuItem();

            add.Text = "Add...";
            add.Click += (s, e) =>
            {
                Var var = new Var()
                {
                    Name = "new" + type,
                    ID = NextVarId(script),
                    IsNull = false,
                    Type = type,
                    Value = "",
                    Refs = new List<int>()
                };
                script.TriggerVars.Add(var.ID, var);
                ShowVarOptionsMenu(var, add.Owner.Location);

                if (setTrigger != -1 && setLogic != -1)
                {
                    Logics(script.Triggers[setTrigger], setSlot).ElementAt(setLogic).SetValueOfParam(setVar, var.ID);
                }
            };

            return add;
        }
        public static ToolStripItem LogicAddItem(Trigger trigger, TriggerLogicSlot slot, int index)
        {
            ToolStripMenuItem root = new ToolStripMenuItem("Add...");
            LogicType t = slot == TriggerLogicSlot.Condition ? LogicType.Condition : LogicType.Effect;

            Dictionary<string, ToolStripMenuItem> categories = new Dictionary<string, ToolStripMenuItem>();

            foreach (var i in LogicIds(t))
            {
                ToolStripMenuItem b = new ToolStripMenuItem(LogicName(t, i));
                b.Click += (s, e) =>
                {
                    var logic = LogicFromId(t, i, LogicVersions(t, i).First());
                    if (slot == TriggerLogicSlot.Condition)
                        trigger.Conditions.Insert(index, (Condition)logic);
                    if (slot == TriggerLogicSlot.EffectTrue)
                        trigger.TriggerEffectsOnTrue.Insert(index, (Effect)logic);
                    if (slot == TriggerLogicSlot.EffectFalse)
                        trigger.TriggerEffectsOnFalse.Insert(index, (Effect)logic);
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
        public static IEnumerable<ToolStripItem> TriggerOptionItems(Trigger trigger)
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
            };
            items.Add(conditional);

            return items;
        }
        public static IEnumerable<ToolStripItem> VarListItems(Triggerscript script)
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

                };

                varRoot.DropDownItems.Add(varAdd);
                varRoot.DropDownItems.Add(new ToolStripSeparator());

                foreach(var v in script.TriggerVars.Values.Where(tv => tv.Type == varTy && tv.IsNull == false))
                {
                    ToolStripMenuItem varCur = new ToolStripMenuItem(v.Name == "" ? "\"\"" : v.Name);

                    varRoot.DropDownItems.Add(varCur);
                }
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
        public static IEnumerable<ToolStripItem> VarOptionItems(Var var, EventHandler<string> textChanged = null)
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
            };
            items.Add(val);

            return items;
        }
        public static IEnumerable<ToolStripItem> ConditionOptionItems(Condition condition)
        {
            List<ToolStripItem> items = new List<ToolStripItem>();

            //Info
            ToolStripLabel conditionLabel = new ToolStripLabel("Condition");
            items.Add(conditionLabel);
            ToolStripLabel info = new ToolStripLabel(string.Format("{0} [v{1}]", condition.TypeName, condition.Version)/*, Properties.Resources.information*/);
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
            };
            items.Add(comment);

            return items;
        }
        public static IEnumerable<ToolStripItem> EffectOptionItems(Effect effect)
        {
            List<ToolStripItem> items = new List<ToolStripItem>();

            //Info
            ToolStripLabel effLabel = new ToolStripLabel("Effect");
            items.Add(effLabel);
            ToolStripLabel info = new ToolStripLabel(string.Format("{0} [v{1}]", effect.TypeName, effect.Version)/*, Properties.Resources.information*/);
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
            };
            items.Add(comment);
            items.Add(commentSeparator);

            return items;
        }
    }
}

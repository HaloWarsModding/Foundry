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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Chef.Win.UI
{
    public static class TriggerscriptMenus
    {
        //Show menu based on selection
        public static void ShowOptionsForSelection(Triggerscript script, int triggerId, TriggerLogicSlot slot, int logicIndex, int varId, Point point)
        {
            if (triggerId != -1)
            {
                Trigger trigger = script.Triggers[triggerId];
                if (logicIndex == -1) //no logic selected
                {
                    //if (selection.InContainer)
                    //{
                    //    ShowLogicAddMenu(trigger, selection.LogicType, Logics(trigger, selection.LogicType).Count(), point);
                    //}
                    //else
                    //{
                        ShowTriggerOptionsMenu(trigger, point);
                    //}
                }
                else
                {
                    Logic logic = Logics(trigger, slot).ElementAt(logicIndex);
                    if (logicIndex != -1)
                    {
                        if (varId == -1)
                        {
                            if (slot == TriggerLogicSlot.Condition)
                            {
                                ShowConditionOptionsMenu((Condition)logic, point);
                            }
                            else
                            {
                                ShowEffectOptionsMenu((Effect)logic, point);
                            }
                        }
                        else
                        {
                            //ShowSetVarMenu(script, trigger, logic, varId, point);
                        }
                    }
                }
            }
            else
            {
                ShowVarList(script, point);
            }
        }

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
            menu.Items.AddRange(VarOptionItems(var).ToArray());
            menu.Show(point);
        }
        public static void ShowSetVarMenu(Triggerscript script, Trigger trigger, TriggerLogicSlot slot, int logic, int sigid, Point point)
        {
            Validate(script);

            Logic l = Logics(trigger, slot).ElementAt(logic);

            if (!l.StaticParamInfo.ContainsKey(sigid)) return;

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.MouseHover += (s, e) => { menu.Focus(); };
            var paramInfo = l.StaticParamInfo[sigid];
            int currentId = l.GetValueOfParam(sigid);

            //Info
            menu.Items.Add(new ToolStripLabel(paramInfo.Name + " [" + paramInfo.Type + "]"));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(VarNewItem(script, paramInfo.Type, trigger.ID, slot, logic, sigid));
            menu.Items.Add(VarNullItem(script, l, sigid));
            menu.Items.Add(VarListItem(script, "Set...", trigger, new List<VarType>() { paramInfo.Type },
                (s, e) =>
                {
                    l.SetValueOfParam(sigid, e);
                }));

            menu.Show(point);
        }
        public static void ShowVarList(Triggerscript script, Point point)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add(VarListItem(script, "Variables", null, null,
                (s, e) =>
                {
                    ShowVarOptionsMenu(script.TriggerVars[e], point);
                }));

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
        public static ToolStripItem VarListItem(Triggerscript script, string text, Trigger localTrigger = null, IEnumerable<VarType> types = null, EventHandler<int> varClicked = null)
        {
            ToolStripMenuItem root = new ToolStripMenuItem(text);

            Dictionary<VarType, ToolStripMenuItem> typeItems = new Dictionary<VarType, ToolStripMenuItem>();

            List<Var> selectionSet =
                localTrigger == null ?
                script.TriggerVars.Values.ToList() :
                script.TriggerVars.Values.Where(v => (v.Refs.Contains(localTrigger.ID) || v.Refs.Count == 0)).ToList();
            selectionSet.Sort((l, r) =>
            {
                int ret = l.Type.ToString().CompareTo(r.Type.ToString());
                if (ret == 0) ret = l.Name.CompareTo(r.Name);
                return ret;
            });

            foreach (var v in selectionSet)
            {
                ToolStripMenuItem varItem = new ToolStripMenuItem();
                varItem.Text = v.Name == "" ? "\"\"" : v.Name;
                //varItem.Image = v.Refs.Count() > 1 ? Properties.Resources.world : null;
                varItem.Click += (s, e) => { varClicked?.Invoke(varItem, v.ID); };

                if (types == null || types.Count() != 1)
                {
                    if (!typeItems.ContainsKey(v.Type))
                    {
                        typeItems.Add(v.Type, new ToolStripMenuItem(v.Type.ToString()));
                        root.DropDownItems.Add(typeItems[v.Type]);
                        typeItems[v.Type].DropDownItems.Add(VarNewItem(script, v.Type)); //add "new var..." button.
                        typeItems[v.Type].DropDownItems.Add(new ToolStripSeparator()); //add separator.
                    }

                    if (types != null)
                    {
                        if (types.Contains(v.Type))
                            typeItems[v.Type].DropDownItems.Add(varItem);
                    }
                    else
                    {
                        typeItems[v.Type].DropDownItems.Add(varItem);
                    }
                }
                else
                {
                    if (types.Contains(v.Type))
                        root.DropDownItems.Add(varItem);
                }
            }

            return root;
        }
        public static ToolStripItem VarNewItem(Triggerscript script, VarType type, int setTrigger = -1, TriggerLogicSlot setSlot = TriggerLogicSlot.Condition, int setLogic = -1, int setVar = -1)
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
        public static ToolStripItem VarNullItem(Triggerscript script, Logic logic, int sigid)
        {
            ToolStripMenuItem nullVar = new ToolStripMenuItem();
            var paramInfo = logic.StaticParamInfo[sigid];
            int currentId = logic.GetValueOfParam(sigid);

            nullVar.Text = "Set null";
            nullVar.Click += (s, e) =>
            {
                logic.SetValueOfParam(sigid, GetOrAddNullVar(script, paramInfo.Type));
            };
            nullVar.Enabled = paramInfo.Optional; //only nullable if optional.
            nullVar.ToolTipText = !paramInfo.Optional ? "Only optional parameters can be set to null." : "";
            return nullVar;
        }
        public static ToolStripItem VarSearchItem(Triggerscript script, int defaultVarId, Trigger localTrigger = null, IEnumerable<VarType> types = null, EventHandler<int> varChanged = null)
        {
            ToolStripComboBox availableVars = new ToolStripComboBox();
            availableVars.AutoSize = false;
            availableVars.Size = new Size(165, 20);
            availableVars.AutoCompleteMode = AutoCompleteMode.Suggest;
            availableVars.AutoCompleteSource = AutoCompleteSource.ListItems;

            IEnumerable<Var> selectionSet =
                localTrigger == null ?
                script.TriggerVars.Values :
                script.TriggerVars.Values.Where(v => v.Refs.Contains(localTrigger.ID) || v.Refs.Count == 0).ToList();

            if (types != null)
            {
                selectionSet = selectionSet.Where(v => types.Contains(v.Type));
            }

            availableVars.Items.AddRange(selectionSet.ToArray());
            availableVars.ComboBox.DisplayMember = "Name";

            for (int i = 0; i < availableVars.Items.Count; i++)
            {
                if (((Var)availableVars.Items[i]).ID == defaultVarId)
                    availableVars.ComboBox.SelectedIndex = i;
            }

            availableVars.SelectedIndexChanged += (s, e) =>
            {
                varChanged?.Invoke(availableVars, selectionSet.ElementAt(availableVars.SelectedIndex).ID);
            };

            return availableVars;

            //var paramInfo = logic.StaticParamInfo[sigid];
            //int currentId = logic.GetValueOfParam(sigid);

            ////availableVars.AutoSize = false;
            ////availableVars.Size = new Size(165, 20);
            ////availableVars.AutoCompleteMode = AutoCompleteMode.Suggest;
            ////availableVars.AutoCompleteSource = AutoCompleteSource.ListItems;
            ////foreach (var v in Variables(script, paramInfo.Type))
            ////{
            ////    availableVars.Items.Add(v);
            ////    if (v.ID == currentId)
            ////    {
            ////        availableVars.SelectedIndex = availableVars.Items.Count - 1;
            ////    }
            ////}
            ////availableVars.ComboBox.SelectedIndexChanged += (s, e) =>
            ////{
            ////    logic.SetValueOfParam(sigid, ((Var)availableVars.SelectedItem).ID);
            ////};

            //return availableVars;
        }
        public static ToolStripItem TriggerSearchItem(Triggerscript script, int defaultTriggerId, EventHandler<int> triggerChanged = null)
        {
            ToolStripComboBox item = new ToolStripComboBox();
            return item;
        }
        public static ToolStripItem LogicAddItem(Trigger trigger, TriggerLogicSlot slot, int index)
        {
            ToolStripMenuItem root = new ToolStripMenuItem("Add...");
            LogicType t = slot == TriggerLogicSlot.Condition ? LogicType.Condition : LogicType.Effect;
            foreach (var i in Database.LogicIds(t))
            {
                ToolStripMenuItem b = new ToolStripMenuItem(Database.LogicName(t, i));
                b.Click += (s, e) =>
                {
                    var logic = Database.LogicFromId(t, i, Database.LogicVersions(t, i).First());
                    if (slot == TriggerLogicSlot.Condition)
                        trigger.Conditions.Insert(index, (Condition)logic);
                    if (slot == TriggerLogicSlot.EffectTrue)
                        trigger.TriggerEffectsOnTrue.Insert(index, (Effect)logic);
                    if (slot == TriggerLogicSlot.EffectFalse)
                        trigger.TriggerEffectsOnFalse.Insert(index, (Effect)logic);
                };
                root.DropDownItems.Add(b);
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
        public static IEnumerable<ToolStripItem> VarOptionItems(Var var)
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

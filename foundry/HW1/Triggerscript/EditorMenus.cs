using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Foundry.HW1.Triggerscript.EditorHelpers;

namespace Foundry.HW1.Triggerscript
{
    public static class EditorMenusWinForms
    {
        public static void ShowOptionsForSelection(Triggerscript script, Selection selection, Point point)
        {
            if (selection.TriggerId != -1)
            {
                if (selection.LogicIndex == -1) //no logic selected
                {
                    ShowTriggerOptionsMenu(script.Triggers[selection.TriggerId], point);
                }
                else
                {
                    Logic logic = SelectedLogic(script, selection);
                    if (logic != null)
                    {
                        if (selection.VarSigId == -1)
                        {
                            if (selection.LogicType == TriggerLogicSlot.Condition) ShowConditionOptionsMenu((Condition)logic, point);
                            else ShowEffectOptionsMenu((Effect)logic, point);
                        }
                        else
                        {
                            ShowSetVarMenu(script, logic, selection.VarSigId, point);
                        }
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

        public static void ShowSetVarMenu(Triggerscript script, Logic logic, int sigid, Point point)
        {
            if (!logic.StaticParamInfo.ContainsKey(sigid)) return;

            ContextMenuStrip menu = new ContextMenuStrip();
            var paramInfo = logic.StaticParamInfo[sigid];
            int currentId = logic.GetValueOfParam(sigid);

            //Info
            menu.Items.Add(new ToolStripLabel(paramInfo.Name + " [" + paramInfo.Type + "]"));
            menu.Items.Add(new ToolStripSeparator());

            //New var
            ToolStripMenuItem add = new ToolStripMenuItem();
            add.Text = "Add new var...";
            add.Click += (s, e) =>
            {
                Var var = new Var()
                {
                    Name = "new" + paramInfo.Type,
                    ID = NextVarId(script),
                    IsNull = false,
                    Type = paramInfo.Type,
                    Value = ""
                };
                script.TriggerVars.Add(var.ID, var);
                menu.Close();
                ShowVarOptionsMenu(var, point);
            };
            menu.Items.Add(add);

            //Null var
            ToolStripMenuItem nullVar = new ToolStripMenuItem();
            nullVar.Text = "Set null";
            nullVar.Click += (s, e) =>
            {
                logic.SetValueOfParam(sigid, GetOrAddNullVar(script, paramInfo.Type));
            };
            menu.Items.Add(nullVar);
            nullVar.Enabled = paramInfo.Optional; //only nullable if optional.
            nullVar.ToolTipText = "Only optional parameters can be set to null.";
            menu.Items.Add(new ToolStripSeparator());
                        
            //Vars to pick from
            ToolStripComboBox availableVars = new ToolStripComboBox();
            availableVars.AutoSize = false;
            availableVars.Size = new Size(165, 20);
            foreach (var v in Variables(script, paramInfo.Type))
            {
                availableVars.Items.Add(v);
                if (v.ID == currentId) availableVars.SelectedIndex = availableVars.Items.Count - 1;
            }
            availableVars.ComboBox.SelectedIndexChanged += (s, e) =>
            {
                logic.SetValueOfParam(sigid, ((Var)availableVars.SelectedItem).ID);
            };
            menu.Items.Add(availableVars);


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
                    e.Cancel = true;
                }
            };

            //Info
            ToolStripLabel varLabel = new ToolStripLabel(var.Type + " Variable");
            menu.Items.Add(varLabel);
            menu.Items.Add(new ToolStripSeparator());

            //Name
            menu.Items.Add(new ToolStripLabel("Name:"));
            ToolStripTextBox name = new ToolStripTextBox();
            name.Text = var.Name;
            name.BorderStyle = BorderStyle.FixedSingle;
            name.TextChanged += (s, e) =>
            {
                var.Name = name.Text;
            };
            menu.Items.Add(name);

            //Value
            menu.Items.Add(new ToolStripLabel("Value:"));
            ToolStripTextBox val = new ToolStripTextBox();
            val.Text = var.Value;
            val.BorderStyle = BorderStyle.FixedSingle;
            val.TextChanged += (s, e) =>
            {
                var.Value = name.Text;
            };
            menu.Items.Add(val);


            menu.Show(point);
        }
    }
}

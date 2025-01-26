using Chef.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using YAXLib;

namespace Chef.HW1.Script
{
    public static class TriggerscriptIO
    {
        /// <summary>
        /// Reads a triggerscript from an xml stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static void ReadXml(Stream stream, Triggerscript script)
        {
            XDocument doc = XDocument.Load(stream);
            XElement root = doc.Root;

            Dictionary<int, Var> vars = new Dictionary<int, Var>();

            foreach(var e in root.Elements())
            {
                if (e.Name == "TriggerGroups")
                {

                }

                if (e.Name == "TriggerVars")
                {
                    ReadVars(e, script, vars);
                }

                if (e.Name == "Triggers")
                {
                    ReadTriggers(e, script, vars);
                }
            }
        }
        private static void ReadVars(XElement r, Triggerscript script, Dictionary<int, Var> vars)
        {
            foreach(var v in r.Elements())
            {
                if (v.Name == "TriggerVar")
                {
                    int id = int.Parse(v.Attribute("ID").Value);
                    bool isNull = v.Attribute("IsNull").Value.ToLower() == "true" ? true : false;

                    var newVar = new Var()
                    {
                        Name = v.Attribute("Name").Value,
                        Type = TriggerscriptHelpers.TypeFromString(v.Attribute("Type").Value),
                        //Value = v.Value == null ? "" : v.Value
                    };

                    if (newVar.Name == "")
                    {
                        newVar.Name = "unnamed";
                    }

                    if (!isNull)
                    {
                        vars.Add(id, newVar);
                    }
                }
            }
        }
        private static void ReadTriggers(XElement r, Triggerscript script, Dictionary<int, Var> vars)
        {
            foreach (var triggerNode in r.Elements().Where(node => node.Name == "Trigger"))
            {
                int id = int.Parse(triggerNode.Attribute("ID").Value);

                Trigger trigger = new Trigger()
                {
                    ID = id,
                    Name = triggerNode.Attribute("Name").Value,
                    Active = bool.Parse(triggerNode.Attribute("Active").Value),
                    ConditionalTrigger = bool.Parse(triggerNode.Attribute("ConditionalTrigger").Value),
                    EvalLimit = float.Parse(triggerNode.Attribute("EvalLimit").Value),
                    EvaluateFrequency = float.Parse(triggerNode.Attribute("EvaluateFrequency").Value),
                    X = float.Parse(triggerNode.Attribute("X").Value) * TriggerscriptParams.TriggerSpacingMultiplier,
                    Y = float.Parse(triggerNode.Attribute("Y").Value) * TriggerscriptParams.TriggerSpacingMultiplier,
                    GroupID = int.Parse(triggerNode.Attribute("GroupID").Value)
                };

                ReadTriggerConditions(triggerNode, script, trigger, vars);
                ReadTriggerEffectsTrue(triggerNode, script, trigger, vars);
                ReadTriggerEffectsFalse(triggerNode, script, trigger, vars);

                script.Triggers.Add(id, trigger);
            }
        }
        private static void ReadTriggerConditions(XElement r, Triggerscript script, Trigger t, Dictionary<int, Var> vars)
        {
            XElement conditions = null;
            if (r.Element("TriggerConditions").Element("And") != null)
            {
                conditions = r.Element("TriggerConditions").Element("And");
                t.ConditionsAreAND = true;
            }
            if (r.Element("TriggerConditions").Element("Or") != null)
            {
                conditions = r.Element("TriggerConditions").Element("Or");
                t.ConditionsAreAND = false;
            }
            if (conditions == null) return;

            foreach (var cndNode in conditions.Elements().Where(node => node.Name == "Condition"))
            {
                Condition cnd = new Condition();
                cnd.Invert = bool.Parse(cndNode.Attribute("Invert").Value);
                cnd.Async = bool.Parse(cndNode.Attribute("Async").Value);
                cnd.AsyncParameterKey = int.Parse(cndNode.Attribute("AsyncParameterKey").Value);
                ReadTriggerLogicBase(cndNode, script, cnd, vars);

                if (TriggerscriptHelpers.LogicIds(LogicType.Condition).Contains(cnd.DBID)
                   &&
                   TriggerscriptHelpers.LogicVersions(LogicType.Condition, cnd.DBID).Contains(cnd.Version))
                {
                    t.Conditions.Add(cnd);
                }
            }
        }
        private static void ReadTriggerEffectsTrue(XElement r, Triggerscript script, Trigger t, Dictionary<int, Var> vars)
        {
            var trues = r.Element("TriggerEffectsOnTrue");
               
            foreach (var effNode in trues.Elements().Where(node => node.Name == "Effect"))
            {
                Effect eff = new Effect();
                ReadTriggerLogicBase(effNode, script, eff, vars);

                if (TriggerscriptHelpers.LogicIds(LogicType.Effect).Contains(eff.DBID)
                   &&
                   TriggerscriptHelpers.LogicVersions(LogicType.Effect, eff.DBID).Contains(eff.Version))
                {
                    t.TriggerEffectsOnTrue.Add(eff);
                }
            }
        }
        private static void ReadTriggerEffectsFalse(XElement r, Triggerscript script, Trigger t, Dictionary<int, Var> vars)
        {
            var trues = r.Element("TriggerEffectsOnFalse");

            foreach (var effNode in trues.Elements().Where(node => node.Name == "Effect"))
            {
                Effect eff = new Effect();
                ReadTriggerLogicBase(effNode, script, eff, vars);

                if (TriggerscriptHelpers.LogicIds(LogicType.Effect).Contains(eff.DBID)
                    && 
                    TriggerscriptHelpers.LogicVersions(LogicType.Effect, eff.DBID).Contains(eff.Version))
                {
                    t.TriggerEffectsOnFalse.Add(eff);
                }
            }
        }
        private static void ReadTriggerLogicBase(XElement r, Triggerscript script, Logic l, Dictionary<int, Var> vars)
        {
            XAttribute comment = r.Attribute("Comment");
            if (comment != null) l.Comment = comment.Value;

            l.DBID = int.Parse(r.Attribute("DBID").Value);
            l.Version = int.Parse(r.Attribute("Version").Value);
            foreach (var c in r.Elements())
            {
                if (c.Name == "Input" || c.Name == "Output")
                {
                    int sigid = int.Parse(c.Attribute("SigID").Value);
                    int sigval = int.Parse(c.Value);
                    if (vars.ContainsKey(sigval))
                    {
                        l.Params[sigid] = vars[sigval];
                    }
                    else
                    {
                        l.Params[sigid] = null;
                    }
                }
            }
        }


        public static void WriteXml(Stream stream, Triggerscript script)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("TriggerSystem");
            root.SetAttributeValue("Name", "");
            root.SetAttributeValue("Type", "TriggerScript");
            root.SetAttributeValue("NextTriggerVarID", 0);
            root.SetAttributeValue("NextTriggerID", 0);
            root.SetAttributeValue("NextConditionID", 0);
            root.SetAttributeValue("NextEffectID", 0);
            root.SetAttributeValue("External", false);

            XElement groups = new XElement("TriggerGroups");


            Dictionary<Var, int> varIds = new Dictionary<Var, int>();
            Dictionary<VarType, Var> nullVars = new Dictionary<VarType, Var>();

            XElement triggers = new XElement("Triggers");
            WriteTriggers(triggers, script, varIds, nullVars);

            XElement vars = new XElement("TriggerVars");
            WriteVars(vars, varIds, nullVars);

            root.Add(groups);
            root.Add(vars);
            root.Add(triggers);
            doc.Add(root);

            doc.Save(stream);
        }
        private static void WriteVars(XElement varsNode, Dictionary<Var, int> varIds, Dictionary<VarType, Var> nullVars)
        {
            foreach(var (var, id) in varIds)
            {
                XElement varNode = new XElement("TriggerVar");
                varNode.SetAttributeValue("ID", id);
                varNode.SetAttributeValue("Name", var.Name);
                varNode.SetAttributeValue("Type", var.Type);
                varNode.SetAttributeValue("IsNull", nullVars.ContainsValue(var));
                //varNode.Value = var;

                varsNode.Add(varNode);
            }
        }
        private static void WriteTriggers(XElement triggersNode, Triggerscript script, Dictionary<Var, int> varIds, Dictionary<VarType, Var> nullVars)
        {
            foreach (var trigger in script.Triggers)
            {
                XElement triggerNode = new XElement("Trigger");
                triggerNode.SetAttributeValue("ID", trigger.Key);
                triggerNode.SetAttributeValue("Name", trigger.Value.Name);
                triggerNode.SetAttributeValue("Active", trigger.Value.Active);
                triggerNode.SetAttributeValue("EvaluateFrequency", trigger.Value.EvaluateFrequency);
                triggerNode.SetAttributeValue("EvalLimit", trigger.Value.EvalLimit);
                triggerNode.SetAttributeValue("ConditionalTrigger", trigger.Value.ConditionalTrigger);
                triggerNode.SetAttributeValue("X", trigger.Value.X / TriggerscriptParams.TriggerSpacingMultiplier);
                triggerNode.SetAttributeValue("Y", trigger.Value.Y / TriggerscriptParams.TriggerSpacingMultiplier);
                triggerNode.SetAttributeValue("GroupID", trigger.Value.GroupID);

                WriteTriggerConditions(triggerNode, trigger.Value, varIds, nullVars);
                WriteTriggerEffectsTrue(triggerNode, trigger.Value, varIds, nullVars);
                WriteTriggerEffectsFalse(triggerNode, trigger.Value, varIds, nullVars);

                triggersNode.Add(triggerNode);
            }
        }
        private static void WriteTriggerConditions(XElement triggerNode, Trigger trigger, Dictionary<Var, int> varIds, Dictionary<VarType, Var> nullVars)
        {
            XElement triggerConditions = new XElement("TriggerConditions");
            triggerNode.Add(triggerConditions);
            
            XElement andOr = new XElement(trigger.ConditionsAreAND ? "And" : "Or");
            triggerConditions.Add(andOr);

            foreach (var cnd in trigger.Conditions)
            {
                XElement cndNode = new XElement("Condition");
                WriteTriggerLogicBase(cndNode, cnd, varIds, nullVars);
                cndNode.SetAttributeValue("Async", cnd.Async);
                cndNode.SetAttributeValue("AsyncParameterKey", cnd.AsyncParameterKey);
                cndNode.SetAttributeValue("Invert", cnd.Invert);
                andOr.Add(cndNode);
            }
        }
        private static void WriteTriggerEffectsTrue(XElement triggerNode, Trigger trigger, Dictionary<Var, int> varIds, Dictionary<VarType, Var> nullVars)
        {
            XElement effTrue = new XElement("TriggerEffectsOnTrue");
            triggerNode.Add(effTrue);

            foreach (var eff in trigger.TriggerEffectsOnTrue)
            {
                XElement effNode = new XElement("Effect");
                WriteTriggerLogicBase(effNode, eff, varIds, nullVars);
                effTrue.Add(effNode);
            }
        }
        private static void WriteTriggerEffectsFalse(XElement triggerNode, Trigger trigger, Dictionary<Var, int> varIds, Dictionary<VarType, Var> nullVars)
        {
            XElement effFalse = new XElement("TriggerEffectsOnFalse");
            triggerNode.Add(effFalse);

            foreach (var eff in trigger.TriggerEffectsOnFalse)
            {
                XElement effNode = new XElement("Effect");
                WriteTriggerLogicBase(effNode, eff, varIds, nullVars);
                effFalse.Add(effNode);
            }
        }
        private static void WriteTriggerLogicBase(XElement logicNode, Logic logic, Dictionary<Var, int> varIds, Dictionary<VarType, Var> nullVars)
        {
            logicNode.SetAttributeValue("Type", TriggerscriptHelpers.LogicName(logic.Type, logic.DBID));
            logicNode.SetAttributeValue("DBID", logic.DBID);
            logicNode.SetAttributeValue("Version", logic.Version);
            logicNode.SetAttributeValue("ID", 0);
            logicNode.SetAttributeValue("Comment", logic.Comment);

            foreach (var (sigid, par) in TriggerscriptHelpers.LogicParamInfos(logic.Type, logic.DBID, logic.Version))
            {
                XElement paramNode = new XElement(par.Output ? "Output" : "Input");
                paramNode.SetAttributeValue("SigID", sigid);
                paramNode.SetAttributeValue("Name", par.Name);
                paramNode.SetAttributeValue("Type", par.Type);

                //if the logic does not have a value for this sigid, or its value is null...
                if (!logic.Params.ContainsKey(sigid) || logic.Params[sigid] == null)
                {
                    //if we dont have a null var for this type add one...
                    if (!nullVars.ContainsKey(par.Type))
                    {
                        Var newNull = new Var()
                        {
                            Name = "NULL",
                            Type = par.Type,

                        };
                        int id = varIds.Values.Count == 0 ? 0 : varIds.Values.Max() + 1;
                        nullVars.Add(par.Type, newNull);
                        varIds.Add(newNull, id);
                    }

                    paramNode.Value = varIds[nullVars[par.Type]].ToString();
                }
                else
                {
                    //if we dont have an id for this var add one...
                    if (!varIds.ContainsKey(logic.Params[sigid]))
                    {
                        int id = varIds.Values.Count == 0 ? 0 : varIds.Values.Max() + 1;
                        varIds.Add(logic.Params[sigid], id);
                    }

                    paramNode.Value = varIds[logic.Params[sigid]].ToString();
                }
                
                logicNode.Add(paramNode);
            }
        }
    }
}

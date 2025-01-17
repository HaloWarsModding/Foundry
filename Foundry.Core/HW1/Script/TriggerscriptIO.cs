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

            foreach(var e in root.Elements())
            {
                if (e.Name == "TriggerGroups")
                {

                }

                if (e.Name == "TriggerVars")
                {
                    ReadVars(e, script);
                }

                if (e.Name == "Triggers")
                {
                    ReadTriggers(e, script);
                }
            }
        }

        private static void ReadVars(XElement r, Triggerscript script)
        {
            foreach(var v in r.Elements())
            {
                if (v.Name == "TriggerVar")
                {
                    int id = int.Parse(v.Attribute("ID").Value);

                    script.TriggerVars.Add(id, new Var()
                    {
                        ID = id,
                        IsNull = v.Attribute("IsNull").Value.ToLower() == "true" ? true : false,
                        Name = v.Attribute("Name").Value,
                        Type = TriggerscriptHelpers.TypeFromString(v.Attribute("Type").Value),
                        Value = v.Value == null ? "" : v.Value
                    });
                }
            }
        }
        private static void ReadTriggers(XElement r, Triggerscript script)
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
                    X = float.Parse(triggerNode.Attribute("X").Value),
                    Y = float.Parse(triggerNode.Attribute("Y").Value),
                    GroupID = int.Parse(triggerNode.Attribute("GroupID").Value)
                };

                ReadTriggerConditions(triggerNode, script, trigger);
                ReadTriggerEffectsTrue(triggerNode, script, trigger);
                ReadTriggerEffectsFalse(triggerNode, script, trigger);

                script.Triggers.Add(id, trigger);
            }
        }

        private static void ReadTriggerConditions(XElement r, Triggerscript script, Trigger t)
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
                ReadTriggerLogicBase(cndNode, script, cnd);
                t.Conditions.Add(cnd);
            }
        }
        private static void ReadTriggerEffectsTrue(XElement r, Triggerscript script, Trigger t)
        {
            var trues = r.Element("TriggerEffectsOnTrue");
               
            foreach (var effNode in trues.Elements().Where(node => node.Name == "Effect"))
            {
                Effect eff = new Effect();
                ReadTriggerLogicBase(effNode, script, eff);
                t.TriggerEffectsOnTrue.Add(eff);
            }
        }
        private static void ReadTriggerEffectsFalse(XElement r, Triggerscript script, Trigger t)
        {
            var trues = r.Element("TriggerEffectsOnFalse");

            foreach (var effNode in trues.Elements().Where(node => node.Name == "Effect"))
            {
                Effect eff = new Effect();
                ReadTriggerLogicBase(effNode, script, eff);
                t.TriggerEffectsOnTrue.Add(eff);
            }
        }

        private static void ReadTriggerLogicBase(XElement r, Triggerscript script, Logic l)
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
                    l.SetValueOfParam(sigid, int.Parse(c.Value));
                }
            }
        }
    

        public static void WriteXml(Stream stream, Triggerscript script)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("TriggerSystem");
            
            XElement groups = new XElement("TriggerGroups");
            
            XElement vars = new XElement("TriggerVars");
            WriteVars(vars, script);
            
            XElement triggers = new XElement("Triggers");
            WriteTriggers(triggers, script);

            root.Add(groups);
            root.Add(vars);
            root.Add(triggers);
            doc.Add(root);

            doc.Save(stream);
        }

        private static void WriteVars(XElement varsNode, Triggerscript script)
        {
            foreach(var var in script.TriggerVars)
            {
                XElement varNode = new XElement("TriggerVar");
                varNode.SetAttributeValue("ID", var.Key);
                varNode.SetAttributeValue("Name", var.Value.Name);
                varNode.SetAttributeValue("Type", var.Value.Type);
                varNode.SetAttributeValue("IsNull", var.Value.IsNull);
                
                if (var.Value.IsConst)
                {
                    varNode.Value = var.Value.Value;
                }
                else
                {
                    varNode.Value = "";
                }

                varsNode.Add(varNode);
            }
        }
        private static void WriteTriggers(XElement triggersNode, Triggerscript script)
        {
            foreach(var trigger in script.Triggers)
            {
                XElement triggerNode = new XElement("Trigger");
                triggerNode.SetAttributeValue("ID", trigger.Key);
                triggerNode.SetAttributeValue("Name", trigger.Value.Name);
                triggerNode.SetAttributeValue("Active", trigger.Value.Active);
                triggerNode.SetAttributeValue("EvaluateFrequency", trigger.Value.EvaluateFrequency);
                triggerNode.SetAttributeValue("EvalLimit", trigger.Value.EvalLimit);
                triggerNode.SetAttributeValue("ConditionalTrigger", trigger.Value.ConditionalTrigger);
                triggerNode.SetAttributeValue("X", trigger.Value.X);
                triggerNode.SetAttributeValue("Y", trigger.Value.Y);
                triggerNode.SetAttributeValue("GroupID", trigger.Value.GroupID);

                WriteTriggerConditions(triggerNode, trigger.Value);
                WriteTriggerEffectsTrue(triggerNode, trigger.Value);
                WriteTriggerEffectsFalse(triggerNode, trigger.Value);

                triggersNode.Add(triggerNode);
            }
        }
        
        private static void WriteTriggerConditions(XElement triggerNode, Trigger trigger)
        {
            XElement andOr = new XElement(trigger.ConditionsAreAND ? "And" : "Or");
            triggerNode.Add(andOr);

            foreach (var cnd in trigger.Conditions)
            {
                XElement cndNode = new XElement("Condition");
                WriteTriggerLogicBase(cndNode, cnd);
                cndNode.SetAttributeValue("Async", cnd.Async);
                cndNode.SetAttributeValue("AsyncParameterKey", cnd.AsyncParameterKey);
                cndNode.SetAttributeValue("Invert", cnd.Invert);
                andOr.Add(cndNode);
            }
        }
        private static void WriteTriggerEffectsTrue(XElement triggerNode, Trigger trigger)
        {
            XElement effTrue = new XElement("TriggerEffectsOnTrue");
            triggerNode.Add(effTrue);

            foreach (var eff in trigger.TriggerEffectsOnTrue)
            {
                XElement effNode = new XElement("Effect");
                WriteTriggerLogicBase(effNode, eff);
                effTrue.Add(effNode);
            }
        }
        private static void WriteTriggerEffectsFalse(XElement triggerNode, Trigger trigger)
        {
            XElement effTrue = new XElement("TriggerEffectsOnFalse");
            triggerNode.Add(effTrue);

            foreach (var eff in trigger.TriggerEffectsOnFalse)
            {
                XElement effNode = new XElement("Effect");
                WriteTriggerLogicBase(effNode, eff);
                effTrue.Add(effNode);
            }
        }

        private static void WriteTriggerLogicBase(XElement logicNode, Logic logic)
        {
            logicNode.SetAttributeValue("DBID", logic.DBID);
            logicNode.SetAttributeValue("Version", logic.Version);

            foreach (var param in logic.StaticParamInfo)
            {
                XElement paramNode = new XElement(param.Value.Output ? "Output" : "Input");
                paramNode.SetAttributeValue("SigID", param.Key);
                paramNode.Value = logic.GetValueOfParam(param.Key).ToString();
                
                logicNode.Add(paramNode);
            }
        }
    }
}

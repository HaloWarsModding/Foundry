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
    }
}

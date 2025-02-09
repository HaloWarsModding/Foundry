using Chef.HW1.Workspace;
using Chef.Util;
using System;
using System.Collections.Generic;
using System.IO;
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

            XElement varsRoot = root.Element("TriggerVars");
            XElement triggersRoot = root.Element("Triggers");

            //read all trigger nodes first. we'll parse their content after we have all var names resolved.
            Dictionary<int, string> triggerNames = new Dictionary<int, string>();
            Dictionary<int, int> triggerDupNum = new Dictionary<int, int>();
            Dictionary<int, Trigger> triggers = new Dictionary<int, Trigger>();
            Dictionary<Trigger, XElement> triggerElems = new Dictionary<Trigger, XElement>();
            foreach (var e in triggersRoot.Elements("Trigger"))
            {
                int id = int.Parse(e.Attribute("ID").Value);
                string name = e.Attribute("Name").Value;

                triggerDupNum[id] = triggerNames.Values.Where(v => v == name).Count() + 1;
                triggerNames[id] = name;

                Trigger trigger = new Trigger()
                {
                    Active = bool.Parse(e.Attribute("Active").Value),
                    ConditionalTrigger = bool.Parse(e.Attribute("ConditionalTrigger").Value),
                    EvalLimit = float.Parse(e.Attribute("EvalLimit").Value),
                    EvaluateFrequency = float.Parse(e.Attribute("EvaluateFrequency").Value),
                    X = float.Parse(e.Attribute("X").Value) * TriggerscriptParams.TriggerSpacingMultiplier,
                    Y = float.Parse(e.Attribute("Y").Value) * TriggerscriptParams.TriggerSpacingMultiplier,
                };

                triggers[id] = trigger;
                triggerElems[trigger] = e;
            }
            foreach (var (id, dup) in triggerDupNum)
            {
                //dupe name fixup
                if (dup > 1) triggerNames[id] += "_" + dup;
            }
            foreach (var (id, trigger) in triggers)
            {
                script.Triggers[triggerNames[id]] = trigger;
            }

            //parse variables. this comes between reading trigger nodes and their content to allow us to fixup the values of variables with type==trigger.
            Dictionary<int, string> varNames = new Dictionary<int, string>();
            Dictionary<int, int> varDupNumber = new Dictionary<int, int>();
            Dictionary<int, string> values = new Dictionary<int, string>();
            Dictionary<int, VarType> types = new Dictionary<int, VarType>();
            Dictionary<int, bool> isnulls = new Dictionary<int, bool>();
            foreach (var e in varsRoot.Elements("TriggerVar"))
            {
                int id = int.Parse(e.Attribute("ID").Value);

                string value = e.Value;
                values[id] = value;

                VarType type = TriggerscriptHelpers.TypeFromString(e.Attribute("Type").Value);
                if (type == VarType.Trigger)
                {
                    values[id] = triggerNames[int.Parse(value)];
                }
                types[id] = type;

                bool isnull = bool.Parse(e.Attribute("IsNull").Value);
                isnulls[id] = isnull;

                string name = e.Attribute("Name").Value;
                if (name == "") name = "unnamed";
                varDupNumber[id] = varNames.Values.Where(v => v == name).Count() + 1;
                if (isnull) name = null;
                varNames[id] = name;
            }
            foreach (var (id, dup) in varDupNumber)
            {
                //dupe name fixup
                if (dup > 1 && !isnulls[id])
                    varNames[id] += "_" + dup;
            }
            foreach (var (id, val) in values)
            {
                //if value is null or empty, it has no constant value.
                if (!isnulls[id]/* && values[id] != ""*/)
                {
                    if (!script.Constants.ContainsKey(types[id]))
                        script.Constants[types[id]] = new Dictionary<string, string>();
                    script.Constants[types[id]][varNames[id]] = val;
                }
            }

            //finally parse all triggers' content.
            foreach (var (trigger, elem) in triggerElems)
            {
                XElement cndRoot = elem.Element("TriggerConditions").Element("And");
                trigger.ConditionsAreAND = true;
                if (cndRoot == null)
                {
                    cndRoot = elem.Element("TriggerConditions").Element("Or");
                    trigger.ConditionsAreAND = false;
                }
                foreach (var cndElem in cndRoot.Elements("Condition"))
                {
                    Condition cnd = new Condition();
                    cnd.Async = bool.Parse(cndElem.Attribute("Async").Value);
                    cnd.AsyncParameterKey = int.Parse(cndElem.Attribute("AsyncParameterKey").Value);
                    cnd.Invert = bool.Parse(cndElem.Attribute("Invert").Value);
                    ReadTriggerLogicBase(script, cndElem, cnd, varNames);
                    trigger.Conditions.Add(cnd);
                }

                XElement effTrueRoot = elem.Element("TriggerEffectsOnTrue");
                foreach (var effElem in effTrueRoot.Elements("Effect"))
                {
                    Effect eff = new Effect();
                    ReadTriggerLogicBase(script, effElem, eff, varNames);
                    trigger.TriggerEffectsOnTrue.Add(eff);
                }

                XElement effFalseRoot = elem.Element("TriggerEffectsOnFalse");
                foreach (var effElem in effFalseRoot.Elements("Effect"))
                {
                    Effect eff = new Effect();
                    ReadTriggerLogicBase(script, effElem, eff, varNames);
                    trigger.TriggerEffectsOnFalse.Add(eff);
                }
            }
        }
        private static void ReadTriggerLogicBase(Triggerscript script, XElement logicRoot, Logic logic, Dictionary<int, string> varNames)
        {
            XAttribute comment = logicRoot.Attribute("Comment");
            if (comment != null) logic.Comment = comment.Value;

            var paramInfo = TriggerscriptHelpers.LogicParamInfos(logic.Type, logic.DBID, logic.Version);

            logic.DBID = int.Parse(logicRoot.Attribute("DBID").Value);
            logic.Version = int.Parse(logicRoot.Attribute("Version").Value);
            foreach (var paramNode in logicRoot.Elements())
            {
                int sigId = int.Parse(paramNode.Attribute("SigID").Value);
                int varId = int.Parse(paramNode.Value);
                logic.Params[sigId] = varNames.GetValueOrDefault(varId, null);
            }
        }


        /// <summary>
        /// Writes a triggerscript to an xml stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
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
            XElement vars = new XElement("TriggerVars");
            XElement triggers = new XElement("Triggers");

            Dictionary<VarType, Dictionary<string, int>> varIds = new Dictionary<VarType, Dictionary<string, int>>();
            Dictionary<VarType, int> varNullIds = new Dictionary<VarType, int>();
            Dictionary<string, int> triggerIds = new Dictionary<string, int>();

            int tid = 0;
            foreach (var (name, trigger) in script.Triggers)
            {
                triggerIds[name] = tid++;
            }

            foreach (var (name, trigger) in script.Triggers)
            {
                XElement triggerNode = new XElement("Trigger");
                triggers.Add(triggerNode);
                triggerNode.SetAttributeValue("ID", triggerIds[name]);
                triggerNode.SetAttributeValue("Name", name);
                triggerNode.SetAttributeValue("Active", trigger.Active);
                triggerNode.SetAttributeValue("ConditionalTrigger", trigger.ConditionalTrigger);
                triggerNode.SetAttributeValue("EvaluateFrequency", trigger.EvaluateFrequency);
                triggerNode.SetAttributeValue("EvalLimit", trigger.EvalLimit);
                triggerNode.SetAttributeValue("X", trigger.X / TriggerscriptParams.TriggerSpacingMultiplier);
                triggerNode.SetAttributeValue("Y", trigger.Y / TriggerscriptParams.TriggerSpacingMultiplier);
                triggerNode.SetAttributeValue("GroupID", -1);

                XElement cndRoot = new XElement("TriggerConditions");
                triggerNode.Add(cndRoot);
                if (trigger.ConditionsAreAND)
                {
                    var andNode = new XElement("And");
                    cndRoot.Add(andNode);
                    cndRoot = andNode;
                }
                else
                {
                    var orNode = new XElement("Or");
                    cndRoot.Add(orNode);
                    cndRoot = orNode;
                }
                foreach (var cnd in trigger.Conditions)
                {
                    XElement cndNode = new XElement("Condition");
                    WriteTriggerLogicBase(script, cndNode, vars, cnd, varIds, varNullIds, triggerIds);
                    cndNode.SetAttributeValue("Async", cnd.Async);
                    cndNode.SetAttributeValue("AsyncParameterKey", cnd.AsyncParameterKey);
                    cndNode.SetAttributeValue("Invert", cnd.Invert);
                    cndRoot.Add(cndNode);
                }

                XElement effTrueRoot = new XElement("TriggerEffectsOnTrue");
                triggerNode.Add(effTrueRoot);
                foreach (var eff in trigger.TriggerEffectsOnTrue)
                {
                    XElement effNode = new XElement("Effect");
                    WriteTriggerLogicBase(script, effNode, vars, eff, varIds, varNullIds, triggerIds);
                    effTrueRoot.Add(effNode);
                }

                XElement effFalseRoot = new XElement("TriggerEffectsOnFalse");
                triggerNode.Add(effFalseRoot);
                foreach (var eff in trigger.TriggerEffectsOnFalse)
                {
                    XElement effNode = new XElement("Effect");
                    WriteTriggerLogicBase(script, effNode, vars, eff, varIds, varNullIds, triggerIds);
                    effFalseRoot.Add(effNode);
                }
            }

            root.Add(groups);
            root.Add(vars);
            root.Add(triggers);
            doc.Add(root);

            doc.Save(stream);
        }
        private static void WriteTriggerLogicBase(Triggerscript script, XElement logicRoot, XElement varRoot, Logic logic, 
            Dictionary<VarType, Dictionary<string, int>> varIds, Dictionary<VarType, int> varNullIds, Dictionary<string, int> triggerIds)
        {
            logicRoot.SetAttributeValue("Type", TriggerscriptHelpers.LogicName(logic.Type, logic.DBID));
            logicRoot.SetAttributeValue("DBID", logic.DBID);
            logicRoot.SetAttributeValue("Version", logic.Version);
            logicRoot.SetAttributeValue("ID", 0);
            logicRoot.SetAttributeValue("Comment", logic.Comment);

            foreach (var (sigid, par) in TriggerscriptHelpers.LogicParamInfos(logic.Type, logic.DBID, logic.Version))
            {
                if (!varIds.ContainsKey(par.Type)) 
                    varIds[par.Type] = new Dictionary<string, int>();

                XElement paramNode = new XElement(par.Output ? "Output" : "Input");
                paramNode.SetAttributeValue("SigID", sigid);
                paramNode.SetAttributeValue("Name", par.Name);
                paramNode.SetAttributeValue("Type", par.Type);

                string sigval = logic.Params.GetValueOrDefault(sigid, null);

                int curId = 0;
                if (sigval == null && varNullIds.ContainsKey(par.Type))
                {
                    curId = varNullIds[par.Type];
                }
                else if (sigval != null && varIds[par.Type].ContainsKey(sigval))
                {
                    curId = varIds[par.Type][sigval];
                }
                else
                {
                    //get a new id from ones we already have.
                    foreach (var i in varIds)
                        foreach (var j in i.Value)
                            curId = Math.Max(curId, j.Value);
                    foreach (var i in varNullIds)
                        curId = Math.Max(curId, i.Value);
                    curId++;

                    string name = sigval == null ? "NULL" : sigval;

                    XElement newVarNode = new XElement("TriggerVar");
                    newVarNode.SetAttributeValue("ID", curId);
                    newVarNode.SetAttributeValue("Name", name);
                    newVarNode.SetAttributeValue("Type", par.Type);
                    newVarNode.SetAttributeValue("IsNull", sigval == null);
                    varRoot.Add(newVarNode);

                    if (sigval != null
                        && script.Constants.ContainsKey(par.Type)
                        && script.Constants[par.Type].ContainsKey(sigval))
                    {
                        newVarNode.Value = script.Constants[par.Type][sigval];
                        if (par.Type == VarType.Trigger)
                        {
                            newVarNode.Value = triggerIds[newVarNode.Value].ToString();
                        }
                    }

                    if (sigval == null)
                        varNullIds[par.Type] = curId;
                    else
                        varIds[par.Type][sigval] = curId;
                }

                paramNode.Value = curId.ToString();

                logicRoot.Add(paramNode);
            }
        }
    }
}
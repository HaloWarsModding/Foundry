using KSoft.Phoenix.Phx;
using System;
using System.Collections.Generic;
using System.Drawing; //cross platform System.Drawing.Primitives is used.
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using YAXLib;
using YAXLib.Attributes;
using static Chef.HW1.Script.TriggerscriptParams;

namespace Chef.HW1.Script
{
    public struct Selection
    {
        public Selection()
        {
            TriggerId = -1;
            LogicType = TriggerLogicSlot.Condition;
            LogicIndex = -1;
            InsertIndex = -1;
            VarSigId = -1;
            UnitId = -1;
        }

        public int TriggerId { get; set; }
        public TriggerLogicSlot LogicType { get; set; }
        public int LogicIndex { get; set; }
        public int InsertIndex { get; set; }
        public int VarSigId { get; set; }
        public int UnitId { get; set; }


        public static bool operator ==(Selection lhs, Selection rhs)
        {
            return lhs.TriggerId == rhs.TriggerId
                && lhs.LogicType == rhs.LogicType
                && lhs.LogicIndex == rhs.LogicIndex
                && lhs.VarSigId == rhs.VarSigId
                && lhs.UnitId == rhs.UnitId;
        }
        public static bool operator !=(Selection lhs, Selection rhs)
        {
            return !(lhs == rhs);
        }
    }

    public struct LogicParamInfo
    {
        public LogicParamInfo() { }

        [YAXAttributeForClass()]
        public string Name { get; set; } = "";
        [YAXAttributeForClass()]
        public VarType Type { get; set; } = VarType.Invalid;
        [YAXAttributeForClass()]
        public bool Optional { get; set; } = false;
        [YAXAttributeForClass()]
        public bool Output { get; set; } = false;
    }
    public struct LogicVersionInfo
    {
        public LogicVersionInfo() { }

        [YAXDictionary(
            EachPairName = "Param",
            KeyName = "sigid",
            SerializeKeyAs = YAXLib.Enums.YAXNodeTypes.Attribute,
            SerializeValueAs = YAXLib.Enums.YAXNodeTypes.Attribute)]
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
        public Dictionary<int, LogicParamInfo> Params { get; set; } = new Dictionary<int, LogicParamInfo>();
    }
    public struct LogicTypeInfo
    {
        public LogicTypeInfo() { }

        [YAXAttributeFor("..")]
        public string Name { get; set; } = "";

        [YAXAttributeFor("..")]
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore, DefaultValue = "")]
        public string Category { get; set; }

        [YAXDictionary(
            EachPairName = "Version",
            KeyName = "v",
            ValueName = "Params",
            SerializeKeyAs = YAXLib.Enums.YAXNodeTypes.Attribute,
            SerializeValueAs = YAXLib.Enums.YAXNodeTypes.Attribute)]
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
        public Dictionary<int, LogicVersionInfo> Versions { get; set; } = new Dictionary<int, LogicVersionInfo>();

    }
    public struct LogicDatabase
    {
        public LogicDatabase() { }

        [YAXDictionary(
            EachPairName = "Logic",
            KeyName = "dbid",
            ValueName = "Type",
            SerializeKeyAs = YAXLib.Enums.YAXNodeTypes.Attribute,
            SerializeValueAs = YAXLib.Enums.YAXNodeTypes.Attribute)]
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
        public Dictionary<int, LogicTypeInfo> Types { get; set; } = new Dictionary<int, LogicTypeInfo>();
    }

    public static class TriggerscriptHelpers
    {
        //Bounds
        public static Rectangle ScriptBounds(Triggerscript script)
        {
            if (script.Triggers.Count == 0) return Rectangle.Empty;
            Trigger minX = script.Triggers.Values.First();
            Trigger minY = script.Triggers.Values.First();
            Trigger maxX = script.Triggers.Values.First();
            Trigger maxY = script.Triggers.Values.First();
            foreach (Trigger t in script.Triggers.Values)
            {
                if (t.X < minX.X) minX = t;
                if (t.Y < minY.Y) minY = t;
                if (t.X > maxX.X) maxX = t;
                if (t.Y > maxY.Y) maxY = t;
            }

            Rectangle ret = new Rectangle(
                (int)minX.X,
                (int)minY.Y,
                (int)(maxX.X - minX.X + BoundsTriggerUnit(maxX).Width),
                (int)(maxY.Y - minY.Y + BoundsTriggerUnit(maxY).Height)
                );
            ret.Inflate(100, 100);
            return ret;
        }
        
        public static Rectangle BoundsTriggerUnit(Trigger trigger)
        {
            Rectangle triggerBounds = BoundsTriggerNode(trigger);
            Rectangle ret = triggerBounds;

            foreach (var type in Enum.GetValues<TriggerLogicSlot>())
            {
                for (int i = 0; i < Logics(trigger, type).Count(); i++)
                {
                    Rectangle logicBounds = BoundsLogicNode(trigger, type, i);
                    ret.Width = logicBounds.X - triggerBounds.X + logicBounds.Width;
                    ret.Height = Math.Max(ret.Height, logicBounds.Height);
                }
            }
            ret.Width += 50;
            //TODO: Separate logic unit container header from the rest of this.
            ret.Height += HeaderHeight;

            return ret;
        }
        public static Rectangle BoundsTriggerNode(Trigger trigger)
        {
            return new Rectangle(
                (int)trigger.X,
                (int)trigger.Y,
                DefaultWidth,
                HeaderHeight * 3);
        }
        public static Rectangle BoundsLogicUnit(Trigger trigger, TriggerLogicSlot type)
        {
            IEnumerable<Logic> logics;
            Point loc;
            if (type == TriggerLogicSlot.Condition)
            {
                logics = trigger.Conditions;
                var bounds = BoundsTriggerNode(trigger);
                loc = bounds.Location;
                loc.X += bounds.Width;
                loc.X += LogicSectionSpacing;
                //loc.Y -= HeaderHeight;
            }
            else if (type == TriggerLogicSlot.EffectTrue)
            {
                logics = trigger.TriggerEffectsOnTrue;
                Rectangle blu = BoundsLogicUnit(trigger, TriggerLogicSlot.Condition);
                loc = new Point(blu.X + blu.Width + LogicSectionSpacing, blu.Y);
            }
            else
            {
                logics = trigger.TriggerEffectsOnFalse;
                Rectangle blu = BoundsLogicUnit(trigger, TriggerLogicSlot.EffectTrue);
                loc = new Point(blu.X + blu.Width + LogicSectionSpacing, blu.Y);
            }

            int logicsCount = logics.Count();
            Size size = new Size(0, 25);
            if (logicsCount == 0)
            {
                size.Width = LogicSectionSpacing * 4;
            }
            else
            {
                size.Width = DefaultWidth * logicsCount;
                size.Width += LogicSpacing * (logicsCount - 1);
            }
            foreach (var l in logics)
            {
                size.Height = Math.Max(size.Height, BodySize(l).Height);
            }

            //TODO: Separate logic unit container header from the rest of this.
            size.Height += HeaderHeight;
            return new Rectangle(loc, size);
        }
        public static Rectangle BoundsLogicNode(Trigger trigger, TriggerLogicSlot type, int index)
        {
            IEnumerable<Logic> logics = Logics(trigger, type);

            Point loc = BoundsLogicUnit(trigger, type).Location;
            loc.Y += HeaderHeight;
            for (int i = 0; i < index; i++)
            {
                loc.X += BodySize(logics.ElementAt(i)).Width;
                loc.X += LogicSpacing;
            }

            return new Rectangle(loc, BodySize(logics.ElementAt(index)));
        }
        public static Rectangle BoundsLogicDrop(Trigger trigger, TriggerLogicSlot type, int index)
        {
            //TODO: technically this works, but its a mess.
            //I want to rework all the bounds to not need random += everywhere.
            //TODO: Separate logic unit container header from the rest of this.
            var logics = Logics(trigger, type);

            Rectangle fullbounds = BoundsTriggerUnit(trigger);
            Rectangle ubounds = BoundsLogicUnit(trigger, type);

            if (logics.Count() == 0)
            {
                ubounds.Height = fullbounds.Height;
                return ubounds; //if theres no nodes, just use the unit bounds.
            }

            index = Math.Min(index, logics.Count());
            Rectangle nbounds = ubounds;
            nbounds.X = ubounds.X - (DefaultWidth / 2) + (index * (DefaultWidth + LogicSpacing));
            nbounds.Width = DefaultWidth;

            ubounds.Inflate(LogicSectionSpacing / 2, 0);
            nbounds.Intersect(ubounds);
            nbounds.Height = fullbounds.Height;

            return nbounds;
        }

        public static Rectangle ParamNameBounds(Trigger trigger, TriggerLogicSlot type, int index, int paramIndex)
        {
            Rectangle logicBounds = BoundsLogicNode(trigger, type, index);
            Rectangle ret = new Rectangle(
                logicBounds.X + Margin,
                logicBounds.Y + HeaderHeight + paramIndex * VarSpacing + paramIndex * VarHeight + VarHeight / 2,
                logicBounds.Width - Margin * 2,
                VarNameHeight);
            return ret;
        }
        public static Rectangle ParamValBounds(Trigger trigger, TriggerLogicSlot type, int index, int paramIndex)
        {
            Rectangle logicBounds = BoundsLogicNode(trigger, type, index);
            Rectangle ret = new Rectangle(
                logicBounds.X + Margin,
                logicBounds.Y + HeaderHeight + paramIndex * VarSpacing + paramIndex * VarHeight + VarHeight / 2 + VarNameHeight,
                logicBounds.Width - Margin * 2,
                VarValHeight);
            return ret;
        }
       
        public static Size BodySize(Logic logic)
        {
            int varCount = logic.StaticParamInfo.Count;

            int width = DefaultWidth;

            return new Size(
                width,
                HeaderHeight + varCount * (VarHeight + VarSpacing) + VarHeight
                );
        }

        public static void BodyBoundsAtPoint(Triggerscript script, Point point, out int trigger, out TriggerLogicSlot slot, out int logic)
        {
            trigger = -1;
            slot = TriggerLogicSlot.Condition;
            logic = -1;

            foreach (var t in script.Triggers.Values)
            {
                if (BoundsTriggerNode(t).Contains(point))
                {
                    trigger = t.ID;
                    return;
                }

                foreach (var s in Enum.GetValues<TriggerLogicSlot>())
                {
                    var l = Logics(t, s);
                    for (int i = 0; i < l.Count(); i++)
                    {
                        if (BoundsLogicNode(t, s, i).Contains(point))
                        {
                            trigger = t.ID;
                            slot = s;
                            logic = i;
                            return;
                        }
                    }
                }
            }
            return;
        }
        public static void DropBoundsAtPoint(Triggerscript script, Point point, out int trigger, out TriggerLogicSlot slot, out int logic)
        {
            trigger = -1;
            slot = TriggerLogicSlot.Condition;
            logic = -1;

            foreach (var t in script.Triggers.Values)
            {
                foreach (var s in Enum.GetValues<TriggerLogicSlot>())
                {
                    var l = Logics(t, s);
                    for (int i = 0; i < l.Count() + 1; i++) //count + 1 because we also want the trailing drop bounds.
                    {
                        if (BoundsLogicDrop(t, s, i).Contains(point))
                        {
                            trigger = t.ID;
                            slot = s;
                            logic = i;
                            return;
                        }
                    }
                }
            }
            return;
        }
        public static void VarBoundsAtPoint(Triggerscript script, Point point, out int trigger, out TriggerLogicSlot slot, out int logic, out int param)
        {
            param = -1;
            BodyBoundsAtPoint(script, point, out trigger, out slot, out logic);
            if (trigger == -1 || logic == -1) return;

            Trigger t = script.Triggers[trigger];
            Logic l = Logics(t, slot).ElementAt(logic);
            for (int i = 0; i < l.StaticParamInfo.Count; i++)
            {
                if (//ParamNameBounds(t, slot, logic, i).Contains(point)
                    //||
                    ParamValBounds(t, slot, logic, i).Contains(point))
                {
                    param = l.StaticParamInfo.ElementAt(i).Key;
                    return;
                }
            }
        }


        //Queries
        //public static Selection SelectAt(Triggerscript script, Point point)
        //{
        //    Selection ret = new Selection();

        //    ret.TriggerId = -1;
        //    ret.LogicIndex = -1;
        //    foreach (var trigger in script.Triggers.Values)
        //    {
        //        Rectangle unitBounds = UnitBounds(trigger);
        //        if (unitBounds.Contains(point))
        //        {
        //            ret.UnitId = trigger.ID;
        //            ret.TriggerId = trigger.ID;

        //            foreach (var type in Enum.GetValues<TriggerLogicSlot>())
        //            {
        //                if (SlotBounds(trigger, type).Contains(point))
        //                {
        //                    ret.LogicType = type;

        //                    var logics = Logics(trigger, type);
        //                    for (int i = 0; i < logics.Count(); i++)
        //                    {
        //                        if (LogicDropBounds(trigger, type, i).Contains(point))
        //                        {
        //                            ret.InsertIndex = i;
        //                        }
        //                        if (LogicBodyBounds(trigger, type, i).Contains(point))
        //                        {
        //                            ret.LogicIndex = i;

        //                            for (int v = 0; v < Logics(trigger, type).ElementAt(i).StaticParamInfo.Count(); v++)
        //                            {
        //                                if (ParamValBounds(trigger, type, i, v).Contains(point))
        //                                {
        //                                    ret.VarSigId = Logics(trigger, type).ElementAt(i).StaticParamInfo.ElementAt(v).Key;
        //                                }
        //                            }
        //                        }
        //                    }

        //                    if (LogicDropBounds(trigger, type, logics.Count()).Contains(point))
        //                    {
        //                        ret.InsertIndex = logics.Count();
        //                    }

        //                    if (Logics(trigger, type).Count() == 0)
        //                    {
        //                        ret.LogicIndex = 0;
        //                        ret.InsertIndex = 0;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return ret;
        //}
        public static int NextVarId(Triggerscript script)
        {
            List<int> ids = script.TriggerVars.Keys.ToList();
            ids.Sort();
            for (int i = 0; i < ids.Count; i++)
            {
                //this is the last one.
                if (i == ids.Count - 1) return ids[i] + 1;
                else
                {
                    if (ids[i] != ids[i + 1] - 1) return ids[i] + 1;
                }
            }
            return -1; //this shouldnt happen.
        }
        public static int NextTriggerId(Triggerscript script)
        {
            List<int> ids = script.Triggers.Keys.ToList();
            ids.Sort();
            for (int i = 0; i < ids.Count; i++)
            {
                //this is the last one.
                if (i == ids.Count - 1) 
                    return ids[i] + 1;
                else
                {
                    if (ids[i] != ids[i + 1] - 1) 
                        return ids[i] + 1;
                }
            }
            return -1; //this shouldnt happen.
        }
        public static IEnumerable<Logic> Logics(Trigger trigger, TriggerLogicSlot slot)
        {
            if (slot == TriggerLogicSlot.Condition) return trigger.Conditions;
            else if (slot == TriggerLogicSlot.EffectTrue) return trigger.TriggerEffectsOnTrue;
            else return trigger.TriggerEffectsOnFalse;
        }
        public static IEnumerable<Var> Variables(Triggerscript script, VarType type)
        {
            return script.TriggerVars.Values.Where(v => v.Type == type);
        }
        /// <summary>
        /// Is var id used in any condition or effect?
        /// </summary>
        public static bool VarUsedIn(int varid, Trigger trigger)
        {
            if (VarUsedIn(varid, trigger, TriggerLogicSlot.Condition)) return true;
            if (VarUsedIn(varid, trigger, TriggerLogicSlot.EffectTrue)) return true;
            if (VarUsedIn(varid, trigger, TriggerLogicSlot.EffectFalse)) return true;
            return false;
        }
        /// <summary>
        /// Is var id used in any logic of a particular slot?
        /// </summary>
        public static bool VarUsedIn(int varid, Trigger trigger, TriggerLogicSlot slot)
        {
            IEnumerable<Logic> logics = Logics(trigger, slot);
            for (int i = 0; i < logics.Count(); i++)
            {
                if (VarUsedIn(varid, trigger, slot, i)) return true;
            }
            return false;
        }
        /// <summary>
        /// Is var id used in the logic of a particular slot at index?
        /// </summary>
        public static bool VarUsedIn(int varid, Trigger trigger, TriggerLogicSlot slot, int index)
        {
            IEnumerable<Logic> logics = Logics(trigger, slot);

            foreach (Logic logic in logics)
            {
                foreach (var (sigid, _) in logic.StaticParamInfo)
                {
                    if (logic.GetValueOfParam(sigid) == varid) return true;
                }
            }

            return false;
        }


        //Transformations
        public static bool TransferLogic(Trigger fromTrigger, TriggerLogicSlot fromType, int fromIndex, Trigger toTrigger, TriggerLogicSlot toType, int toIndex)
        {
            if (!CanTransfer(fromType, toType)) return false;
            //if (fromIndex >= TriggerLogicCount(fromTrigger, fromType)) return false;
            //if (toIndex >= TriggerLogicCount(toTrigger, toType)) return false;

            if (fromType == TriggerLogicSlot.Condition && toType == TriggerLogicSlot.Condition)
            {
                Condition move = fromTrigger.Conditions[fromIndex];
                fromTrigger.Conditions.Remove(move);
                toTrigger.Conditions.Insert(toIndex, move);
                return true;
            }


            Effect eff = null;
            if (fromType == TriggerLogicSlot.EffectTrue)
            {
                eff = fromTrigger.TriggerEffectsOnTrue[fromIndex];
                fromTrigger.TriggerEffectsOnTrue.Remove(eff);
            }
            if (fromType == TriggerLogicSlot.EffectFalse)
            {
                eff = fromTrigger.TriggerEffectsOnFalse[fromIndex];
                fromTrigger.TriggerEffectsOnFalse.Remove(eff);
            }
            if (eff == null) return false;

            if (toType == TriggerLogicSlot.EffectTrue)
            {
                toTrigger.TriggerEffectsOnTrue.Insert(toIndex, eff);
                return true;
            }
            if (toType == TriggerLogicSlot.EffectFalse)
            {
                toTrigger.TriggerEffectsOnFalse.Insert(toIndex, eff);
                return true;
            }

            return false;
        }
        public static int GetOrAddNullVar(Triggerscript script, VarType type)
        {
            Var ret = script.TriggerVars.Values.Where(v => v.IsNull && v.Type == type).FirstOrDefault((Var)null);
            if (ret == null)
            {
                ret = new Var()
                {
                    ID = script.NextTriggerVarID++,
                    IsNull = true,
                    Name = string.Format("Null{0}", type),
                    Type = type,
                    Value = "",
                    Refs = new List<int>()
                };
                script.TriggerVars.Add(ret.ID, ret);
            }
            ret.Name = "Null" + type.ToString() + "Var";
            return ret.ID;
        }

        //Validation
        public static bool CanTransfer(TriggerLogicSlot from, TriggerLogicSlot to)
        {
            if (from == to) return true;
            if (from == TriggerLogicSlot.EffectTrue && to == TriggerLogicSlot.EffectFalse) return true;
            if (from == TriggerLogicSlot.EffectFalse && to == TriggerLogicSlot.EffectTrue) return true;
            return false;
        }
        public static void Validate(Triggerscript script)
        {
            FixupVarLocality(script);
        }
        public static void FixupVarLocality(Triggerscript script)
        {
            foreach (Trigger t in script.Triggers.Values)
            {
                FixupVarLocalityFor(script, t, Logics(t, TriggerLogicSlot.Condition));
                FixupVarLocalityFor(script, t, Logics(t, TriggerLogicSlot.EffectTrue));
                FixupVarLocalityFor(script, t, Logics(t, TriggerLogicSlot.EffectFalse));
            }
        }
        private static void FixupVarLocalityFor(Triggerscript script, Trigger trigger, IEnumerable<Logic> logics)
        {
            foreach (Var v in script.TriggerVars.Values)
            {
                if (v.Refs == null)
                {
                    v.Refs = new List<int>();
                }
            }
            foreach (Logic logic in logics)
            {
                foreach (int sigid in logic.StaticParamInfo.Keys)
                {
                    int val = logic.GetValueOfParam(sigid);
                    if (!script.TriggerVars.ContainsKey(val)) continue;
                    Var v = script.TriggerVars[val];
                    if (!v.Refs.Contains(trigger.ID))
                    {
                        v.Refs.Add(trigger.ID);
                    }
                }
            }
        }

        public static void FixupTriggerVars(Triggerscript script)
        {

        }

        public static IEnumerable<int> LogicIds(LogicType type)
        {
            LogicDatabase db = TableForType(type);
            return db.Types.Keys;
        }
        public static IEnumerable<int> LogicVersions(LogicType type, int dbid)
        {
            LogicDatabase db = TableForType(type);

            if (!db.Types.ContainsKey(dbid)) return new List<int>() { 0 }; //0 == default, only one version.
            if (db.Types[dbid].Versions == null) return new List<int>() { 0 };

            return db.Types[dbid].Versions.Keys;
        }
        public static Dictionary<int, LogicParamInfo> LogicParamInfos(LogicType type, int dbid, int version)
        {
            Dictionary<int, LogicParamInfo> ret = new Dictionary<int, LogicParamInfo>();
            LogicDatabase db = TableForType(type);
            LogicVersionInfo vinfo = new LogicVersionInfo();

            if (!db.Types.ContainsKey(dbid)) return ret;
            if (db.Types[dbid].Versions == null) return ret;

            var validVersions = LogicVersions(type, dbid);

            if (validVersions.Count() == 1)
            {
                vinfo = db.Types[dbid].Versions.First().Value;
            }
            else if (db.Types[dbid].Versions.ContainsKey(version))
            {
                vinfo = db.Types[dbid].Versions[version];
            }

            if (vinfo.Params == null) return ret; //I hate YAXLib...

            return vinfo.Params;
        }
        public static string LogicName(LogicType type, int dbid)
        {
            LogicDatabase db = TableForType(type);
            return db.Types[dbid].Name;
        }
        public static string LogicCategory(LogicType type, int dbid)
        {
            LogicDatabase db = TableForType(type);
            return db.Types[dbid].Category;
        }
        public static VarType TypeFromString(string varTypeName)
        {
            //some strings map to the same type, so its not a 1:1 translation. TODO: put this in a dict?
            VarType varTypeEnum = VarType.Invalid;

            if (varTypeName == "Tech")
                varTypeEnum = VarType.Tech;
            else if (varTypeName == "TechStatus")
                varTypeEnum = VarType.TechStatus;
            else if (varTypeName == "Operator")
                varTypeEnum = VarType.Operator;
            else if (varTypeName == "ProtoObject")
                varTypeEnum = VarType.ProtoObject;
            else if (varTypeName == "ObjectType")
                varTypeEnum = VarType.ObjectType;
            else if (varTypeName == "ProtoSquad")
                varTypeEnum = VarType.ProtoSquad;
            else if (varTypeName == "Sound")
                varTypeEnum = VarType.Sound;
            else if (varTypeName == "Entity")
                varTypeEnum = VarType.Entity;
            else if (varTypeName == "EntityList")
                varTypeEnum = VarType.EntityList;
            else if (varTypeName == "Trigger")
                varTypeEnum = VarType.Trigger;
            else if (varTypeName == "Distance")
                varTypeEnum = VarType.Float;
            else if (varTypeName == "Time")
                varTypeEnum = VarType.Time;
            else if (varTypeName == "Player")
                varTypeEnum = VarType.Player;
            else if (varTypeName == "Count")
                varTypeEnum = VarType.Integer;
            else if (varTypeName == "Location")
                varTypeEnum = VarType.Vector;
            else if (varTypeName == "UILocation")
                varTypeEnum = VarType.UILocation;
            else if (varTypeName == "UIEntity")
                varTypeEnum = VarType.UIEntity;
            else if (varTypeName == "Cost")
                varTypeEnum = VarType.Cost;
            else if (varTypeName == "AnimType")
                varTypeEnum = VarType.AnimType;
            else if (varTypeName == "ActionStatus")
                varTypeEnum = VarType.ActionStatus;
            else if (varTypeName == "Percent")
                varTypeEnum = VarType.Float;
            else if (varTypeName == "Hitpoints")
                varTypeEnum = VarType.Float;
            else if (varTypeName == "Power")
                varTypeEnum = VarType.Power;
            else if (varTypeName == "Bool")
                varTypeEnum = VarType.Bool;
            else if (varTypeName == "Float")
                varTypeEnum = VarType.Float;
            else if (varTypeName == "Iterator")
                varTypeEnum = VarType.Iterator;
            else if (varTypeName == "Team")
                varTypeEnum = VarType.Team;
            else if (varTypeName == "PlayerList")
                varTypeEnum = VarType.PlayerList;
            else if (varTypeName == "TeamList")
                varTypeEnum = VarType.TeamList;
            else if (varTypeName == "PlayerState")
                varTypeEnum = VarType.PlayerState;
            else if (varTypeName == "Objective")
                varTypeEnum = VarType.Objective;
            else if (varTypeName == "Unit")
                varTypeEnum = VarType.Unit;
            else if (varTypeName == "UnitList")
                varTypeEnum = VarType.UnitList;
            else if (varTypeName == "Squad")
                varTypeEnum = VarType.Squad;
            else if (varTypeName == "SquadList")
                varTypeEnum = VarType.SquadList;
            else if (varTypeName == "UIUnit")
                varTypeEnum = VarType.UIUnit;
            else if (varTypeName == "UISquad")
                varTypeEnum = VarType.UISquad;
            else if (varTypeName == "UISquadList")
                varTypeEnum = VarType.UISquadList;
            else if (varTypeName == "String")
                varTypeEnum = VarType.String;
            else if (varTypeName == "MessageIndex")
                varTypeEnum = VarType.MessageIndex;
            else if (varTypeName == "MessageJustify")
                varTypeEnum = VarType.MessageJustify;
            else if (varTypeName == "MessagePoint")
                varTypeEnum = VarType.MessagePoint;
            else if (varTypeName == "Color")
                varTypeEnum = VarType.Color;
            else if (varTypeName == "ProtoObjectList")
                varTypeEnum = VarType.ProtoObjectList;
            else if (varTypeName == "ObjectTypeList")
                varTypeEnum = VarType.ObjectTypeList;
            else if (varTypeName == "ProtoSquadList")
                varTypeEnum = VarType.ProtoSquadList;
            else if (varTypeName == "TechList")
                varTypeEnum = VarType.TechList;
            else if (varTypeName == "MathOperator")
                varTypeEnum = VarType.MathOperator;
            else if (varTypeName == "ObjectDataType")
                varTypeEnum = VarType.ObjectDataType;
            else if (varTypeName == "ObjectDataRelative")
                varTypeEnum = VarType.ObjectDataRelative;
            else if (varTypeName == "Civ")
                varTypeEnum = VarType.Civ;
            else if (varTypeName == "ProtoObjectCollection")
                varTypeEnum = VarType.ProtoObjectCollection;
            else if (varTypeName == "Object")
                varTypeEnum = VarType.Object;
            else if (varTypeName == "ObjectList")
                varTypeEnum = VarType.ObjectList;
            else if (varTypeName == "Group")
                varTypeEnum = VarType.Group;
            else if (varTypeName == "LocationList")
                varTypeEnum = VarType.VectorList;
            else if (varTypeName == "RefCountType")
                varTypeEnum = VarType.RefCountType;
            else if (varTypeName == "UnitFlag")
                varTypeEnum = VarType.UnitFlag;
            else if (varTypeName == "LOSType")
                varTypeEnum = VarType.LOSType;
            else if (varTypeName == "EntityFilterSet")
                varTypeEnum = VarType.EntityFilterSet;
            else if (varTypeName == "PopBucket")
                varTypeEnum = VarType.PopBucket;
            else if (varTypeName == "ListPosition")
                varTypeEnum = VarType.ListPosition;
            else if (varTypeName == "RelationType")
                varTypeEnum = VarType.RelationType;
            else if (varTypeName == "Diplomacy")
                varTypeEnum = VarType.RelationType;
            else if (varTypeName == "ExposedAction")
                varTypeEnum = VarType.ExposedAction;
            else if (varTypeName == "SquadMode")
                varTypeEnum = VarType.SquadMode;
            else if (varTypeName == "ExposedScript")
                varTypeEnum = VarType.ExposedScript;
            else if (varTypeName == "KBBase")
                varTypeEnum = VarType.KBBase;
            else if (varTypeName == "KBBaseList")
                varTypeEnum = VarType.KBBaseList;
            else if (varTypeName == "DataScalar")
                varTypeEnum = VarType.DataScalar;
            else if (varTypeName == "KBBaseQuery")
                varTypeEnum = VarType.KBBaseQuery;
            else if (varTypeName == "DesignLine")
                varTypeEnum = VarType.DesignLine;
            else if (varTypeName == "LocStringID")
                varTypeEnum = VarType.LocStringID;
            else if (varTypeName == "Leader")
                varTypeEnum = VarType.Leader;
            else if (varTypeName == "Cinematic")
                varTypeEnum = VarType.Cinematic;
            else if (varTypeName == "TalkingHead")
                varTypeEnum = VarType.TalkingHead;
            else if (varTypeName == "Direction")
                varTypeEnum = VarType.Vector;
            else if (varTypeName == "FlareType")
                varTypeEnum = VarType.FlareType;
            else if (varTypeName == "CinematicTag")
                varTypeEnum = VarType.CinematicTag;
            else if (varTypeName == "IconType")
                varTypeEnum = VarType.IconType;
            else if (varTypeName == "Difficulty")
                varTypeEnum = VarType.Difficulty;
            else if (varTypeName == "Integer")
                varTypeEnum = VarType.Integer;
            else if (varTypeName == "HUDItem")
                varTypeEnum = VarType.HUDItem;
            else if (varTypeName == "UIItem")
                varTypeEnum = VarType.FlashableUIItem;
            else if (varTypeName == "FlashableUIItem")
                varTypeEnum = VarType.FlashableUIItem;
            else if (varTypeName == "ControlType")
                varTypeEnum = VarType.ControlType;
            else if (varTypeName == "UIButton")
                varTypeEnum = VarType.UIButton;
            else if (varTypeName == "MissionType")
                varTypeEnum = VarType.MissionType;
            else if (varTypeName == "MissionState")
                varTypeEnum = VarType.MissionState;
            else if (varTypeName == "MissionTargetType")
                varTypeEnum = VarType.MissionTargetType;
            else if (varTypeName == "IntegerList")
                varTypeEnum = VarType.IntegerList;
            else if (varTypeName == "BidType")
                varTypeEnum = VarType.BidType;
            else if (varTypeName == "BidState")
                varTypeEnum = VarType.BidState;
            else if (varTypeName == "BuildingCommandState")
                varTypeEnum = VarType.BuildingCommandState;
            else if (varTypeName == "Vector")
                varTypeEnum = VarType.Vector;
            else if (varTypeName == "VectorList")
                varTypeEnum = VarType.VectorList;
            else if (varTypeName == "PlacementRule")
                varTypeEnum = VarType.PlacementRule;
            else if (varTypeName == "KBSquad")
                varTypeEnum = VarType.KBSquad;
            else if (varTypeName == "KBSquadList")
                varTypeEnum = VarType.KBSquadList;
            else if (varTypeName == "KBSquadQuery")
                varTypeEnum = VarType.KBSquadQuery;
            else if (varTypeName == "AISquadAnalysis")
                varTypeEnum = VarType.AISquadAnalysis;
            else if (varTypeName == "AISquadAnalysisComponent")
                varTypeEnum = VarType.AISquadAnalysisComponent;
            else if (varTypeName == "KBSquadFilterSet")
                varTypeEnum = VarType.KBSquadFilterSet;
            else if (varTypeName == "ChatSpeaker")
                varTypeEnum = VarType.ChatSpeaker;
            else if (varTypeName == "RumbleType")
                varTypeEnum = VarType.RumbleType;
            else if (varTypeName == "RumbleMotor")
                varTypeEnum = VarType.RumbleMotor;
            else if (varTypeName == "TechDataCommandType")
                varTypeEnum = VarType.TechDataCommandType;
            else if (varTypeName == "CommandType")
                varTypeEnum = VarType.TechDataCommandType;
            else if (varTypeName == "SquadDataType")
                varTypeEnum = VarType.SquadDataType;
            else if (varTypeName == "EventType")
                varTypeEnum = VarType.EventType;
            else if (varTypeName == "TimeList")
                varTypeEnum = VarType.TimeList;
            else if (varTypeName == "DesignLineList")
                varTypeEnum = VarType.DesignLineList;
            else if (varTypeName == "GameStatePredicate")
                varTypeEnum = VarType.GameStatePredicate;
            else if (varTypeName == "FloatList")
                varTypeEnum = VarType.FloatList;
            else if (varTypeName == "UILocationMinigame")
                varTypeEnum = VarType.UILocationMinigame;
            else if (varTypeName == "SquadFlag")
                varTypeEnum = VarType.SquadFlag;
            else if (varTypeName == "Concept")
                varTypeEnum = VarType.Concept;
            else if (varTypeName == "ConceptList")
                varTypeEnum = VarType.ConceptList;
            else if (varTypeName == "UserClassType")
                varTypeEnum = VarType.UserClassType;

            return varTypeEnum;
        }

        public static Logic LogicFromId(LogicType type, int dbid, int version)
        {
            if (LogicIds(type).Contains(dbid)
                &&
                LogicVersions(type, dbid).Contains(version))
            {
                Logic l;

                if (type == LogicType.Effect) l = new Effect();
                else if (type == LogicType.Condition) l = new Condition();
                else return null;

                //foreach (var info in LogicParamInfos(type, dbid, version))
                //{
                //    var param = new LogicParam()
                //    {
                //        Name = info.Value.Name,
                //        Optional = info.Value.Optional,
                //        SigID = info.Key,
                //        Value = -1
                //    };

                //    if (info.Value.Output)
                //    {
                //        l.Outputs.Add(param);
                //    }
                //    else
                //    {
                //        l.Inputs.Add(param);
                //    }
                //}

                l.DBID = dbid;
                l.Version = version;
                l.Comment = "";
                return l;
            }
            return null;
        }

        private static LogicDatabase TableForType(LogicType type)
        {
            if (type == LogicType.Condition) return ConditionItems;
            else return EffectItems;
        }
        private static LogicDatabase EffectItems { get; set; } = new YAXSerializer<LogicDatabase>().DeserializeFromFile("effects.tsdef");
        private static LogicDatabase ConditionItems { get; set; } = new YAXSerializer<LogicDatabase>().DeserializeFromFile("conditions.tsdef");

    }
}
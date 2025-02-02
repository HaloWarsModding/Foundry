using System;
using System.Collections.Generic;
using System.Drawing; //cross platform System.Drawing.Primitives is used.
using System.Linq;
using System.Reflection;
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
            TriggerId = null;
            LogicType = LogicSlot.Condition;
            LogicIndex = -1;
            InsertIndex = -1;
            VarSigId = -1;
            UnitId = -1;
        }

        public Trigger TriggerId { get; set; }
        public LogicSlot LogicType { get; set; }
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
        public static Rectangle BoundsScript(Triggerscript script)
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
                (int)(maxX.X - minX.X + BoundsTrigger(maxX).Width),
                (int)(maxY.Y - minY.Y + BoundsTrigger(maxY).Height)
                );
            ret.Inflate(100, 100);
            return ret;
        }
        public static Rectangle BoundsTrigger(Trigger trigger)
        {
            Rectangle bounds = BoundsLogicSlot(trigger, LogicSlot.EffectFalse);
            bounds.Width = bounds.Right - (int)trigger.X;
            bounds.X = (int)trigger.X;
            bounds.Y = (int)trigger.Y;
            return bounds;
        }
        public static Rectangle BoundsTriggerMargin(Trigger trigger)
        {
            Rectangle bounds = BoundsTrigger(trigger);
            bounds.Inflate(LogicSectionSpacing, LogicSectionSpacing);
            return bounds;
        }
        public static Rectangle BoundsLogicSlot(Trigger trigger, LogicSlot type)
        {
            Rectangle bounds = new Rectangle();
            bounds.X = (int)trigger.X;
            bounds.Y = (int)trigger.Y;

            int maxParamCount = 0;
            foreach (var c in trigger.Conditions)
            {
                int count = LogicParamInfos(LogicType.Condition, c.DBID, c.Version).Count;
                maxParamCount = count > maxParamCount ? count : maxParamCount;
            }
            foreach (var t in trigger.TriggerEffectsOnTrue)
            {
                int count = LogicParamInfos(LogicType.Effect, t.DBID, t.Version).Count;
                maxParamCount = count > maxParamCount ? count : maxParamCount;
            }
            foreach (var f in trigger.TriggerEffectsOnFalse)
            {
                int count = LogicParamInfos(LogicType.Effect, f.DBID, f.Version).Count;
                maxParamCount = count > maxParamCount ? count : maxParamCount;
            }

            bounds.Height = (HeaderHeight * 3) + (maxParamCount * (VarHeight + VarSpacing)) + VarHeight;


            bounds.X += bounds.Width;
            bounds.Width = trigger.Conditions.Count != 0
                ? (trigger.Conditions.Count * (DefaultWidth + LogicSpacing)) - LogicSpacing
                : DefaultWidth;
            if (type == LogicSlot.Condition) 
                return bounds;

            bounds.X += bounds.Width;
            bounds.X += LogicSectionSpacing;
            bounds.Width = trigger.TriggerEffectsOnTrue.Count != 0 
                ? (trigger.TriggerEffectsOnTrue.Count * (DefaultWidth + LogicSpacing)) - LogicSpacing 
                : DefaultWidth;
            if (type == LogicSlot.EffectTrue)
                return bounds;

            bounds.X += bounds.Width;
            bounds.Width = trigger.TriggerEffectsOnFalse.Count != 0
                ? (trigger.TriggerEffectsOnFalse.Count * (DefaultWidth + LogicSpacing)) - LogicSpacing
                : DefaultWidth;
            if (type == LogicSlot.EffectFalse)
            {
                if (!trigger.ConditionalTrigger && trigger.TriggerEffectsOnFalse.Count == 0)
                {
                    bounds.Width = 0;
                }
                else
                {
                    bounds.X += LogicSectionSpacing; //we only want the spacing from the last section if we have this section.
                }
                return bounds;
            }

            return bounds;
        }
        public static Rectangle BoundsLogicBody(Trigger trigger, LogicSlot type, int index)
        {
            IEnumerable<Logic> logics = Logics(trigger, type);

            Point loc = BoundsLogicSlot(trigger, type).Location;
            loc.Y += HeaderHeight * 2;
            for (int i = 0; i < index; i++)
            {
                loc.X += DefaultWidth;
                loc.X += LogicSpacing;
            }

            Logic cur = logics.ElementAt(index);
            int varCount = LogicParamInfos(type == LogicSlot.Condition ? LogicType.Condition : LogicType.Effect, cur.DBID, cur.Version).Count;

            return new Rectangle(
                loc.X,
                loc.Y,
                DefaultWidth,
                HeaderHeight + (varCount * (VarHeight + VarSpacing)) + VarHeight
                );
        }
        public static Rectangle BoundsLogicInsert(Trigger trigger, LogicSlot type, int index)
        {
            if (type == LogicSlot.EffectFalse && !trigger.ConditionalTrigger)
            {
                return Rectangle.Empty;
            }

            //TODO: Separate logic unit container header from the rest of this.
            var logics = Logics(trigger, type);

            Rectangle ubounds = BoundsLogicSlot(trigger, type);
            if (type != LogicSlot.EffectFalse)
            {
                //grab the height from this, if we dont already have it.
                ubounds.Height = BoundsLogicSlot(trigger, LogicSlot.EffectFalse).Height;
            }

            if (logics.Count() == 0)
            {
                return ubounds; //if there are no nodes and we can have falses, just use the unit bounds.
            }

            index = Math.Min(index, logics.Count());
            Rectangle nbounds = ubounds;
            nbounds.X = ubounds.X - (DefaultWidth / 2) + (index * (DefaultWidth + LogicSpacing));
            nbounds.Width = DefaultWidth;

            ubounds.Inflate(LogicSectionSpacing / 2, 0);
            nbounds.Intersect(ubounds);

            return nbounds;
        }
        public static Rectangle BoundsParamName(Trigger trigger, LogicSlot type, int index, int paramIndex)
        {
            Rectangle logicBounds = BoundsLogicBody(trigger, type, index);
            Rectangle ret = new Rectangle(
                logicBounds.X,
                logicBounds.Y + HeaderHeight + paramIndex * VarSpacing + paramIndex * VarHeight + VarHeight / 2,
                logicBounds.Width,
                VarNameHeight);
            ret.Inflate(-Margin * 2, 0);
            return ret;
        }
        public static Rectangle BoundsParamValue(Trigger trigger, LogicSlot type, int index, int paramIndex)
        {
            Rectangle logicBounds = BoundsLogicBody(trigger, type, index);
            Rectangle ret = new Rectangle(
                logicBounds.X,
                logicBounds.Y + HeaderHeight + paramIndex * VarSpacing + paramIndex * VarHeight + VarHeight / 2 + VarNameHeight,
                logicBounds.Width,
                VarValHeight);
            ret.Inflate(-Margin * 2, 0);
            return ret;
        }


        //Selection
        public static void SelectBoundsBody(Triggerscript script, Point point, out Trigger trigger, out LogicSlot slot, out int logic, out int param, out int insert)
        {
            trigger = null;
            slot = LogicSlot.Condition;
            logic = -1;
            insert = -1;
            param = -1;

            foreach (var (v, t) in script.Triggers.Reverse())
            {
                if (BoundsTriggerMargin(t).Contains(point))
                {
                    trigger = t;
                    foreach (var s in Enum.GetValues<LogicSlot>())
                    {
                        var l = Logics(t, s);
                        //num logics + 1 to allow getting insert index of empty slot.
                        for (int i = 0; i < l.Count() + 1; i++)
                        {
                            //only check node bodies whose indices are less than num logics.
                            if (i < l.Count() && BoundsLogicBody(t, s, i).Contains(point))
                            {
                                slot = s;
                                logic = i;
                                var paramInfos = LogicParamInfos(LogicSlotType(slot), l.ElementAt(i).DBID, l.ElementAt(i).Version);
                                for (int p = 0; p < paramInfos.Count; p++)
                                {
                                    if (BoundsParamValue(trigger, slot, logic, p).Contains(point))
                                    {
                                        param = paramInfos.ElementAt(p).Key;
                                    }
                                }
                            }
                            if (BoundsLogicInsert(t, s, i).Contains(point))
                            {
                                slot = s;
                                insert = i;
                            }
                        }
                    }
                    return;
                }
            }
        }


        //Queries
        public static IEnumerable<Logic> Logics(Trigger trigger)
        {
            return
            [
                ..Logics(trigger, LogicSlot.Condition),
                ..Logics(trigger, LogicSlot.EffectTrue),
                ..Logics(trigger, LogicSlot.EffectFalse),
            ];
        }
        public static IEnumerable<Logic> Logics(Trigger trigger, LogicSlot slot)
        {
            if (slot == LogicSlot.Condition) return trigger.Conditions;
            else if (slot == LogicSlot.EffectTrue) return trigger.TriggerEffectsOnTrue;
            else return trigger.TriggerEffectsOnFalse;
        }


        //Transformations
        public static bool TransferLogic(Trigger fromTrigger, LogicSlot fromType, int fromIndex, Trigger toTrigger, LogicSlot toType, int toIndex)
        {
            if (!CanTransfer(fromType, toType)) return false;
            //if (fromIndex >= TriggerLogicCount(fromTrigger, fromType)) return false;
            //if (toIndex >= TriggerLogicCount(toTrigger, toType)) return false;

            if (fromType == LogicSlot.Condition && toType == LogicSlot.Condition)
            {
                Condition move = fromTrigger.Conditions[fromIndex];
                fromTrigger.Conditions.Remove(move);
                toTrigger.Conditions.Insert(toIndex, move);
                return true;
            }


            Effect eff = null;
            if (fromType == LogicSlot.EffectTrue)
            {
                eff = fromTrigger.TriggerEffectsOnTrue[fromIndex];
                fromTrigger.TriggerEffectsOnTrue.Remove(eff);
            }
            if (fromType == LogicSlot.EffectFalse)
            {
                eff = fromTrigger.TriggerEffectsOnFalse[fromIndex];
                fromTrigger.TriggerEffectsOnFalse.Remove(eff);
            }
            if (eff == null) return false;

            if (toType == LogicSlot.EffectTrue)
            {
                toTrigger.TriggerEffectsOnTrue.Insert(toIndex, eff);
                return true;
            }
            if (toType == LogicSlot.EffectFalse)
            {
                toTrigger.TriggerEffectsOnFalse.Insert(toIndex, eff);
                return true;
            }

            return false;
        }
        public static bool CanTransfer(LogicSlot from, LogicSlot to)
        {
            if (from == to) return true;
            if (from == LogicSlot.EffectTrue && to == LogicSlot.EffectFalse) return true;
            if (from == LogicSlot.EffectFalse && to == LogicSlot.EffectTrue) return true;
            return false;
        }


        //Static param info
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

            if (db.Types.ContainsKey(dbid))
            {
                return db.Types[dbid].Name;
            }
            else
            {
                return "Uknown" + type + dbid;
            }
        }
        public static string LogicCategory(LogicType type, int dbid)
        {
            LogicDatabase db = TableForType(type);
            return db.Types[dbid].Category;
        }
        public static LogicType LogicSlotType(LogicSlot slot)
        {
            return slot == LogicSlot.Condition ? LogicType.Condition : LogicType.Effect;
        }

        public static bool VarTypeIsList(VarType type)
        {
            switch (type)
            {
                case VarType.ConceptList:
                case VarType.DesignLineList:
                case VarType.EntityList:
                case VarType.FloatList:
                case VarType.IntegerList:
                case VarType.KBBaseList:
                case VarType.KBSquadList:
                case VarType.ObjectList:
                case VarType.ObjectTypeList:
                case VarType.PlayerList:
                case VarType.ProtoObjectList:
                case VarType.ProtoSquadList:
                case VarType.SquadList:
                case VarType.TeamList:
                case VarType.TechList:
                case VarType.TimeList:
                case VarType.UISquadList:
                case VarType.UnitList:
                case VarType.VectorList:
                    return true;

                default:
                    return false;
            }
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
                varTypeEnum = VarType.Diplomacy;
            else if (varTypeName == "Diplomacy")
                varTypeEnum = VarType.Diplomacy;
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
        private static LogicDatabase TableForType(LogicType type)
        {
            if (type == LogicType.Condition) return ConditionItems;
            else return EffectItems;
        }
        private static LogicDatabase EffectItems { get; set; } = new YAXSerializer<LogicDatabase>().DeserializeFromFile("effects.tsdef");
        private static LogicDatabase ConditionItems { get; set; } = new YAXSerializer<LogicDatabase>().DeserializeFromFile("conditions.tsdef");

    }
}
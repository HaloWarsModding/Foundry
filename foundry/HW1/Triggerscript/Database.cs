using HelixToolkit.SharpDX.Core;
using SharpDX.Direct2D1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib;
using YAXLib.Attributes;
using static Foundry.HW1.Triggerscript.Database;

namespace Foundry.HW1.Triggerscript
{
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

    public static class Database
    {
        static Database()
        {

        }

        public static IEnumerable<int> LogicVersions(LogicType type, int dbid)
        {
            LogicDatabase db = TableForType(type);

            if (!db.Types.ContainsKey(dbid)) return new List<int>() { 0 }; //0 == default, only one version.
            if (db.Types[dbid].Versions == null) return new List<int>() { 0 };

            return db.Types[dbid].Versions.Keys.ToList();
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
                int defaultVersion = validVersions.First();
                vinfo = db.Types[dbid].Versions[defaultVersion];
            }
            else if (db.Types[dbid].Versions.ContainsKey(version))
            {
                vinfo = db.Types[dbid].Versions[version];
            }

            return vinfo.Params;
        }
        public static string LogicName(LogicType type, int dbid)
        {
            LogicDatabase db = TableForType(type);
            if (!db.Types.ContainsKey(dbid)) return "INVALID";
            return db.Types[dbid].Name;
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

        private static LogicDatabase TableForType(LogicType type)
        {
            if (type == LogicType.Condition) return ConditionItems;
            else return EffectItems;
        }
        private static LogicDatabase EffectItems { get; set; } = new YAXSerializer<LogicDatabase>().DeserializeFromFile("HW1/Triggerscript/effects.tsdef");
        private static LogicDatabase ConditionItems { get; set; } = new YAXSerializer<LogicDatabase>().DeserializeFromFile("HW1/Triggerscript/conditions.tsdef");
    }
}

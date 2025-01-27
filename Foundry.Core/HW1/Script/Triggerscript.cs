using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Enums;
using static Chef.HW1.Script.TriggerscriptHelpers;
using static Chef.HW1.Script.TriggerscriptParams;

namespace Chef.HW1.Script
{
    public enum LogicSlot
    {
        Condition,
        EffectTrue,
        EffectFalse
    }
    public enum LogicType
    {
        Condition,
        Effect
    }
    public enum VarType
    {
        Invalid = 0,
        Tech,
        TechStatus,
        Operator,
        ProtoObject,
        ObjectType,
        ProtoSquad,
        Sound,
        Entity,
        EntityList,
        Trigger,
        Time,
        Player,
        UILocation,
        UIEntity,
        Cost,
        AnimType,
        ActionStatus,
        Power,
        Bool,
        Float,
        Iterator,
        Team,
        PlayerList,
        TeamList,
        PlayerState,
        Objective,
        Unit,
        UnitList,
        Squad,
        SquadList,
        UIUnit,
        UISquad,
        UISquadList,
        String,
        MessageIndex,
        MessageJustify,
        MessagePoint,
        Color,
        ProtoObjectList,
        ObjectTypeList,
        ProtoSquadList,
        TechList,
        MathOperator,
        ObjectDataType,
        ObjectDataRelative,
        Civ,
        ProtoObjectCollection,
        Object,
        ObjectList,
        Group,
        RefCountType,
        UnitFlag,
        LOSType,
        EntityFilterSet,
        PopBucket,
        ListPosition,
        Diplomacy,
        ExposedAction,
        SquadMode,
        ExposedScript,
        KBBase,
        KBBaseList,
        DataScalar,
        KBBaseQuery,
        DesignLine,
        LocStringID,
        Leader,
        Cinematic,
        FlareType,
        CinematicTag,
        IconType,
        Difficulty,
        Integer,
        HUDItem,
        ControlType,
        UIButton,
        MissionType,
        MissionState,
        MissionTargetType,
        IntegerList,
        BidType,
        BidState,
        BuildingCommandState,
        Vector,
        VectorList,
        PlacementRule,
        KBSquad,
        KBSquadList,
        KBSquadQuery,
        AISquadAnalysis,
        AISquadAnalysisComponent,
        KBSquadFilterSet,
        ChatSpeaker,
        RumbleType,
        RumbleMotor,
        TechDataCommandType,
        SquadDataType,
        EventType,
        TimeList,
        DesignLineList,
        GameStatePredicate,
        FloatList,
        UILocationMinigame,
        SquadFlag,
        FlashableUIItem,
        TalkingHead,
        Concept,
        ConceptList,
        UserClassType,
    }

    public class Var
    {
        public VarType Type { get; set; }
        public string Name { get; set; }
    }
    public abstract class Logic
    {
        public Logic()
        {
            Params = new Dictionary<int, Var>();
            Comment = "";
            DBID = -1;
            Version = -1;
        }

        public int Version { get; set; }
        public int DBID { get; set; }
        public string Comment { get; set; }
        public Dictionary<int, Var> Params { get; set; }
        public abstract LogicType Type { get; }
    }
    public class Effect : Logic
    {
        public Effect() : base() { }
        public override LogicType Type { get { return LogicType.Effect; } }
    }
    public class Condition : Logic
    {
        public Condition() : base() { }
        public bool Invert { get; set; }
        public bool Async { get; set; }
        public int AsyncParameterKey { get; set; }
        public override LogicType Type { get { return LogicType.Condition; } }
    }
    public class Trigger
    {
        public Trigger()
        {
            TriggerEffectsOnTrue = new List<Effect>();
            TriggerEffectsOnFalse = new List<Effect>();
            Conditions = new List<Condition>();
            ConditionsAreAND = true;
        }

        public string Name { get; set; }
        public bool Active { get; set; }
        public float EvaluateFrequency { get; set; }
        public float EvalLimit { get; set; }
        public bool ConditionalTrigger { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public bool ConditionsAreAND { get; set; }
        public List<Condition> Conditions { get; set; }
        public List<Effect> TriggerEffectsOnTrue { get; set; }
        public List<Effect> TriggerEffectsOnFalse { get; set; }
    }
    public class Triggerscript
    {
        public Triggerscript()
        {
            Triggers = new Dictionary<Var, Trigger>();
        }
        public Dictionary<Var, Trigger> Triggers { get; set; }
    }
}
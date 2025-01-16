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

//TODO: Get rid of YAXLib!!!! I hate it!!!!

namespace Chef.HW1.Script
{
    public enum TriggerLogicSlot
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
        RelationType,
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
    //public enum EffectType
    //{
    //    Invalid = 0x0,
    //    ActivateTentacle = 0x3B9,
    //    AddXP = 0x354,
    //    AIAddTeleporterZone = 0x407,
    //    AIAnalyzeKBSquadList = 0x2A1,
    //    AIAnalyzeOffenseAToB = 0x29D,
    //    AIAnalyzeProtoSquadList = 0x2A2,
    //    AIAnalyzeSquadList = 0x29C,
    //    AIBindLog = 0x2D0,
    //    AICalculateOffenseRatioAToB = 0x392,
    //    AICalculations = 0x29A,
    //    AIChat = 0x2F2,
    //    AIClearOpportunityRequests = 0x371,
    //    AICreateAreaTarget = 0x3EC,
    //    AICreateWrapper = 0x3F0,
    //    AIDestroyWrapper = 0x3F2,
    //    AIFactoidSubmit = 0x3E1,
    //    AIGetAlertData = 0x2F9,
    //    AIGetAttackAlerts = 0x2F7,
    //    AIGetFlareAlerts = 0x2F6,
    //    AIGetLastAttackAlert = 0x2FC,
    //    AIGetLastFlareAlert = 0x2F8,
    //    AIGetMemory = 0x3BD,
    //    AIGetMissions = 0x25D,
    //    AIGetMissionTargets = 0x227,
    //    AIGetOpportunityRequests = 0x36F,
    //    AIGetTerminalMissions = 0x370,
    //    AIMissionAddSquads = 0x22B,
    //    AIMissionCancel = 0x226,
    //    AIMissionCreate = 0x3ED,
    //    AIMissionGetLaunchScores = 0x262,
    //    AIMissionGetSquads = 0x22D,
    //    AIMissionGetTarget = 0x26B,
    //    AIMissionModifyTickets = 0x251,
    //    AIMissionRemoveSquads = 0x22C,
    //    AIMissionSetFlags = 0x41B,
    //    AIMissionSetMoveAttack = 0x3D6,
    //    AIMissionTargetGetLocation = 0x2F5,
    //    AIMissionTargetGetScores = 0x231,
    //    AIQueryMissionTargets = 0x381,
    //    AIRemoveFromMissions = 0x264,
    //    AIReorderWrapper = 0x3F1,
    //    AISAGetComponent = 0x2A0,
    //    AIScoreMissionTargets = 0x220,
    //    AISetAssetMultipliers = 0x36E,
    //    AISetBiases = 0x302,
    //    AISetFocus = 0x2C7,
    //    AISetPlayerAssetModifier = 0x40B,
    //    AISetPlayerBuildSpeedModifiers = 0x42A,
    //    AISetPlayerDamageModifiers = 0x40E,
    //    AISetPlayerMultipliers = 0x41A,
    //    AISetScoringParms = 0x234,
    //    AISetWinRange = 0x41F,
    //    AISortMissionTargets = 0x221,
    //    AITopicCreate = 0x24D,
    //    AITopicDestroy = 0x24E,
    //    AITopicLotto = 0x271,
    //    AITopicModifyTickets = 0x24F,
    //    AITopicPriorityRequest = 0x30F,
    //    AITopicSetFocus = 0x372,
    //    AIUnopposedTimeToKill = 0x2A9,
    //    AIWrapperModifyFlags = 0x3F4,
    //    AIWrapperModifyParms = 0x3F5,
    //    AIWrapperModifyRadius = 0x3F3,
    //    AsCount = 0x1F2,
    //    AsFloat = 0x166,
    //    AsString = 0xB5,
    //    AsTime = 0x2CC,
    //    AttachmentAddObject = 0x16B,
    //    AttachmentAddType = 0x4A,
    //    AttachmentAddUnit = 0x16C,
    //    AttachmentRemoveAll = 0x4B,
    //    AttachmentRemoveObject = 0x169,
    //    AttachmentRemoveType = 0x53,
    //    AttachmentRemoveUnit = 0x16D,
    //    BidAddToMissions = 0x3C0,
    //    BidClear = 0x179,
    //    BidCreateBlank = 0x171,
    //    BidCreateBuilding = 0x172,
    //    BidCreatePower = 0x3BE,
    //    BidCreateSquad = 0x174,
    //    BidCreateTech = 0x173,
    //    BidDelete = 0x175,
    //    BidGetData = 0x24B,
    //    BidPurchase = 0x187,
    //    BidQuery = 0x30A,
    //    BidRemoveFromMissions = 0x3C1,
    //    BidSetBlockedBuilders = 0x2FE,
    //    BidSetBuilder = 0x26F,
    //    BidSetBuilding = 0x176,
    //    BidSetPadSupplies = 0x3DB,
    //    BidSetPower = 0x3BF,
    //    BidSetPriority = 0x17A,
    //    BidSetQueueLimits = 0x367,
    //    BidSetSquad = 0x178,
    //    BidSetTargetLocation = 0x26E,
    //    BidSetTech = 0x177,
    //    Blocker = 0x1A2,
    //    BlockLeaderPowers = 0x2F4,
    //    BlockMinimap = 0x2F1,
    //    BuildingCommand = 0x22F,
    //    CalculatePercentCount = 0xB8,
    //    CalculatePercentHitpoints = 0xB9,
    //    CalculatePercentTime = 0xBA,
    //    CameraShake = 0x278,
    //    CarpetBomb = 0x47,
    //    ChangeControlledPlayer = 0x403,
    //    ChangeOwner = 0x89,
    //    ChangeSquadMode = 0x184,
    //    ChatForceSubtitles = 0x419,
    //    ClearBuildingCommandState = 0x3AC,
    //    ClearCorpseUnits = 0x34D,
    //    Cloak = 0x1A1,
    //    CloakDetected = 0x230,
    //    CombatDamage = 0x150,
    //    CombineString = 0x2EE,
    //    ConceptClearSub = 0x3A0,
    //    ConceptGetParameters = 0x351,
    //    ConceptPermission = 0x39C,
    //    ConceptResetCooldown = 0x3CB,
    //    ConceptSetParameters = 0x3A6,
    //    ConceptSetPrecondition = 0x39E,
    //    ConceptSetState = 0x39D,
    //    ConceptStartSub = 0x39F,
    //    ConvertKBSquadsToSquads = 0x2B9,
    //    CopyAISquadAnalysis = 0x399,
    //    CopyBool = 0x66,
    //    CopyChatSpeaker = 0x2F3,
    //    CopyColor = 0xA8,
    //    CopyControlType = 0x331,
    //    CopyCost = 0x5E,
    //    CopyCount = 0x62,
    //    CopyDesignLine = 0x368,
    //    CopyDesignLineList = 0x363,
    //    CopyDirection = 0x1F5,
    //    CopyDistance = 0x5F,
    //    CopyEntity = 0x5C,
    //    CopyEntityList = 0x5D,
    //    CopyFloat = 0x67,
    //    CopyFloatList = 0x376,
    //    CopyHitpoints = 0x65,
    //    CopyIconType = 0x2E6,
    //    CopyIntegerList = 0x232,
    //    CopyKBBase = 0x1C7,
    //    CopyKBSquad = 0x2BC,
    //    CopyLocation = 0x63,
    //    CopyLocationList = 0x261,
    //    CopyLocStringID = 0x332,
    //    CopyLOSType = 0x36C,
    //    CopyMessageIndex = 0x105,
    //    CopyObject = 0x253,
    //    CopyObjective = 0x168,
    //    CopyObjectList = 0x29F,
    //    CopyObjectType = 0x59,
    //    CopyObjectTypeList = 0x1E3,
    //    CopyOperator = 0x57,
    //    CopyPercent = 0x64,
    //    CopyPlayer = 0x61,
    //    CopyProtoObject = 0x58,
    //    CopyProtoObjectList = 0x1E1,
    //    CopyProtoSquad = 0x5A,
    //    CopyProtoSquadList = 0x1E2,
    //    CopySound = 0x5B,
    //    CopySquad = 0x90,
    //    CopySquadList = 0x91,
    //    CopyString = 0xA9,
    //    CopyTech = 0x55,
    //    CopyTechList = 0x1E4,
    //    CopyTechStatus = 0x56,
    //    CopyTime = 0x60,
    //    CopyTimeList = 0x358,
    //    CopyUnit = 0x8E,
    //    CopyUnitList = 0x8F,
    //    CostToFloat = 0x2A6,
    //    CountDecrement = 0x35,
    //    CountIncrement = 0x34,
    //    CountToInt = 0x237,
    //    CreateIconObject = 0x208,
    //    CreateObject = 0x23,
    //    CreateObstructionUnit = 0x341,
    //    CreateSquad = 0x24,
    //    CreateSquads = 0x36B,
    //    CreateTimer = 0x292,
    //    CreateUnit = 0x9A,
    //    CustomCommandAdd = 0x279,
    //    CustomCommandExecute = 0x409,
    //    CustomCommandRemove = 0x27A,
    //    Damage = 0x145,
    //    DesignFindSphere = 0x1A9,
    //    DesignLineGetPoints = 0x1CC,
    //    DesignLineListAdd = 0x364,
    //    DesignLineListGetSize = 0x366,
    //    DesignLineListRemove = 0x365,
    //    Destroy = 0x26,
    //    DestroyTimer = 0x293,
    //    EnableAttackNotifications = 0xFF,
    //    EnableChats = 0x349,
    //    EnableFogOfWar = 0x181,
    //    EnableFollowCam = 0x20A,
    //    EnableLetterBox = 0x40C,
    //    EnableMusicManager = 0x3EE,
    //    EnableOverrideTint = 0x308,
    //    EnableScreenBlur = 0x40D,
    //    EnableShield = 0x1CF,
    //    EnableUserMessage = 0x272,
    //    EntityFilterAddCanChangeOwner = 0x42B,
    //    EntityFilterAddDiplomacy = 0x17B,
    //    EntityFilterAddInList = 0x157,
    //    EntityFilterAddIsAlive = 0x156,
    //    EntityFilterAddIsIdle = 0x163,
    //    EntityFilterAddIsSelected = 0x41C,
    //    EntityFilterAddJacking = 0x42E,
    //    EntityFilterAddMaxObjectType = 0x291,
    //    EntityFilterAddObjectTypes = 0x15C,
    //    EntityFilterAddPlayers = 0x158,
    //    EntityFilterAddProtoObjects = 0x15A,
    //    EntityFilterAddProtoSquads = 0x15B,
    //    EntityFilterAddRefCount = 0x15F,
    //    EntityFilterAddTeams = 0x159,
    //    EntityFilterClear = 0x155,
    //    EntityListShuffle = 0x12B,
    //    EventClearFilters = 0x3DD,
    //    EventDelete = 0x3DC,
    //    EventFilterCamera = 0x346,
    //    EventFilterEntity = 0x347,
    //    EventFilterEntityList = 0x348,
    //    EventFilterGameState = 0x2FD,
    //    EventFilterNumeric = 0x33C,
    //    EventFilterType = 0x33B,
    //    EventReset = 0x345,
    //    EventSetFilter = 0x32C,
    //    EventSubscribe = 0x32B,
    //    EventSubscribeUseCount = 0x3FA,
    //    FadeToColor = 0x390,
    //    FadeTransition = 0x39A,
    //    FlareMinimapNormal = 0x87,
    //    FlareMinimapSpoof = 0x84,
    //    FlashEntity = 0x3E8,
    //    FlashUIElement = 0x39B,
    //    FloatListAdd = 0x377,
    //    FloatListGetSize = 0x379,
    //    FloatListRemove = 0x378,
    //    Forbid = 0x11B,
    //    GetAmmo = 0x185,
    //    GetBidsMatching = 0x26A,
    //    GetBuildingTrainQueue = 0x356,
    //    GetChildUnits = 0x144,
    //    GetClosestPath = 0x382,
    //    GetClosestPowerSquad = 0x4E,
    //    GetClosestSquad = 0x355,
    //    GetClosestUnit = 0x3AB,
    //    GetCost = 0x2A7,
    //    GetDeadSquadCount = 0x260,
    //    GetDeadUnitCount = 0x25F,
    //    GetDifficulty = 0x20D,
    //    GetDirection = 0x1F3,
    //    GetDirectionFromLocations = 0x1F4,
    //    GetDistanceLocationLocation = 0x200,
    //    GetDistanceUnitLocation = 0x6F,
    //    GetDistanceUnitUnit = 0x6E,
    //    GetGameMode = 0x429,
    //    GetGameTime = 0x109,
    //    GetGameTimeRemaining = 0x10B,
    //    GetGarrisonedSquads = 0x2FF,
    //    GetGarrisonedUnits = 0x20B,
    //    GetHealth = 0xAE,
    //    GetHitZoneHealth = 0x101,
    //    GetIdleDuration = 0xF3,
    //    GetKBBaseLocation = 0x1FF,
    //    GetLegalBuildings = 0x1FB,
    //    GetLegalSquads = 0x1F9,
    //    GetLegalTechs = 0x1FA,
    //    GetLevel = 0x336,
    //    GetLocation = 0xBD,
    //    GetLOS = 0x2AB,
    //    GetMeanLocation = 0x29E,
    //    GetNPCPlayersByName = 0x3AF,
    //    GetNumTransports = 0x304,
    //    GetObjectiveStats = 0x2EB,
    //    GetObstructionRadius = 0x36A,
    //    GetOwner = 0xC1,
    //    GetParentSquad = 0x207,
    //    GetPlayerCiv = 0xEF,
    //    GetPlayerColor = 0x330,
    //    GetPlayerEconomy = 0x287,
    //    GetPlayerLeader = 0x1DB,
    //    GetPlayerMilitaryStats = 0x2E9,
    //    GetPlayerPop = 0x286,
    //    GetPlayers = 0xAD,
    //    GetPlayers2 = 0x1AF,
    //    GetPlayerScore = 0x288,
    //    GetPlayerTeam = 0x189,
    //    GetPop = 0x2E0,
    //    GetPopularSquadType = 0x422,
    //    GetPowerRadius = 0x3D0,
    //    GetPrimaryHealthComponent = 0x2AA,
    //    GetPrimaryUser = 0x267,
    //    GetProtoObject = 0x277,
    //    GetProtoSquad = 0x276,
    //    GetResources = 0x116,
    //    GetResourcesTotals = 0x152,
    //    GetSquads = 0x71,
    //    GetSquadTrainerType = 0x1CA,
    //    GetTeamPlayers = 0x7B,
    //    GetTeams = 0x7A,
    //    GetTechResearcherType = 0x1CB,
    //    GetTrickleRate = 0x386,
    //    GetUnits = 0x70,
    //    GetUTechBldings = 0x37A,
    //    GrantAchievement = 0x406,
    //    GroupDeactivate = 0x11F,
    //    HintCalloutCreate = 0x329,
    //    HintCalloutDestroy = 0x32A,
    //    HintGlowToggle = 0x328,
    //    HintMessageDestroy = 0x32F,
    //    HintMessageShow = 0x325,
    //    HUDToggle = 0x20E,
    //    IgnoreDpad = 0x40A,
    //    Infect = 0x3D7,
    //    InputUIButton = 0x211,
    //    InputUILocation = 0x54,
    //    InputUILocationMinigame = 0x37C,
    //    InputUIPlaceSquads = 0x303,
    //    InputUISquad = 0xA2,
    //    InputUISquadList = 0x27F,
    //    InputUIUnit = 0xA1,
    //    IntegerListAdd = 0x244,
    //    IntegerListGetSize = 0x246,
    //    IntegerListRemove = 0x245,
    //    IntToCount = 0x238,
    //    IntToPower = 0x402,
    //    InvertBool = 0x11C,
    //    IteratorKBBaseList = 0x1BB,
    //    IteratorLocationList = 0x120,
    //    IteratorObjectList = 0x119,
    //    IteratorObjectTypeList = 0x1EC,
    //    IteratorPlayerList = 0x78,
    //    IteratorProtoObjectList = 0x1EA,
    //    IteratorProtoSquadList = 0x1EB,
    //    IteratorSquadList = 0x99,
    //    IteratorTeamList = 0x79,
    //    IteratorTechList = 0x1ED,
    //    IteratorUnitList = 0x98,
    //    KBAddSquadsToKB = 0x3D9,
    //    KBBaseGetDistance = 0x1BE,
    //    KBBaseGetKBSquads = 0x2BB,
    //    KBBaseGetMass = 0x1BF,
    //    KBBQExecute = 0x1C1,
    //    KBBQExecuteClosest = 0x1F8,
    //    KBBQMaxStaleness = 0x1D8,
    //    KBBQMinStaleness = 0x1D7,
    //    KBBQPlayerRelation = 0x1C3,
    //    KBBQPointRadius = 0x1C2,
    //    KBBQReset = 0x1C0,
    //    KBSFAddCurrentlyVisible = 0x2AD,
    //    KBSFAddInList = 0x2B1,
    //    KBSFAddMaxStaleness = 0x2B5,
    //    KBSFAddMinStaleness = 0x2B4,
    //    KBSFAddObjectTypes = 0x2AE,
    //    KBSFAddPlayerRelation = 0x2B2,
    //    KBSFAddPlayers = 0x2B0,
    //    KBSQBase = 0x2C1,
    //    KBSQCurrentlyVisible = 0x2C4,
    //    KBSQExecute = 0x299,
    //    KBSQExecuteClosest = 0x2BD,
    //    KBSQInit = 0x297,
    //    KBSQMaxStaleness = 0x2C3,
    //    KBSQMinStaleness = 0x2C2,
    //    KBSQObjectType = 0x2C0,
    //    KBSQPlayerRelation = 0x2BF,
    //    KBSQPointRadius = 0x2BE,
    //    KBSQReset = 0x298,
    //    KBSquadFilterClear = 0x2AC,
    //    KBSquadGetLocation = 0x2B8,
    //    KBSquadGetOwner = 0x2B7,
    //    KBSquadGetProtoSquad = 0x2BA,
    //    KBSquadListDiff = 0x33A,
    //    KBSquadListFilter = 0x2B3,
    //    KBSquadListGetSize = 0x2B6,
    //    Kill = 0x25,
    //    LaunchCinematic = 0x1E0,
    //    LaunchProjectile = 0x39,
    //    LaunchScript = 0x188,
    //    LerpColor = 0xBC,
    //    LerpCount = 0xBB,
    //    LerpLocation = 0x126,
    //    LerpPercent = 0x106,
    //    LerpTime = 0x2EC,
    //    LightsetAnimate = 0x215,
    //    LoadGame = 0x3F7,
    //    LocationAdjust = 0x3B,
    //    LocationAdjustDir = 0x22A,
    //    LocationListAdd = 0x121,
    //    LocationListGetByIndex = 0x3D3,
    //    LocationListGetClosest = 0x3D5,
    //    LocationListGetSize = 0x123,
    //    LocationListPartition = 0x135,
    //    LocationListRemove = 0x122,
    //    LocationListShuffle = 0x12A,
    //    LocationTieToGround = 0x3A,
    //    LockPlayerUser = 0x425,
    //    MathCount = 0xB3,
    //    MathDistance = 0x11E,
    //    MathFloat = 0x161,
    //    MathHitpoints = 0xB4,
    //    MathLocation = 0x127,
    //    MathPercent = 0xBE,
    //    MathResources = 0x117,
    //    MathTime = 0x107,
    //    MegaTurretAttack = 0x2E2,
    //    MissionResult = 0x3EA,
    //    MKTest = 0x8B,
    //    ModifyDataScalar = 0x19D,
    //    ModifyProtoData = 0xED,
    //    ModifyProtoSquadData = 0x33D,
    //    Move = 0x42,
    //    MovePath = 0x1B7,
    //    ObjectiveComplete = 0x85,
    //    ObjectiveDecrementCounter = 0x3A8,
    //    ObjectiveDisplay = 0xB7,
    //    ObjectiveGetCurrentCounter = 0x3A9,
    //    ObjectiveGetFinalCounter = 0x3AA,
    //    ObjectiveIncrementCounter = 0x3A7,
    //    ObjectiveUserMessage = 0xA3,
    //    ObjectListAdd = 0x34A,
    //    ObjectListGetSize = 0x34B,
    //    ObjectListRemove = 0x344,
    //    ObjectTypeListShuffle = 0x130,
    //    ObjectTypeToProtoObjects = 0x3FB,
    //    ParkingLotSet = 0x316,
    //    PatherObstructionRebuild = 0x42D,
    //    PatherObstructionUpdates = 0x42C,
    //    PayCost = 0x32,
    //    PlayAnimationObject = 0x24C,
    //    PlayAnimationSquad = 0xA0,
    //    PlayAnimationUnit = 0x9F,
    //    PlayChat = 0x2D9,
    //    PlayerListAdd = 0x7C,
    //    PlayerListGetSize = 0x3CA,
    //    PlayerListRemove = 0x7D,
    //    PlayerListShuffle = 0x12C,
    //    PlayerSelectSquads = 0x310,
    //    PlayersToTeams = 0x1B0,
    //    PlayRelationSound = 0x22,
    //    PlaySound = 0x21,
    //    PlayVideo = 0x342,
    //    PlayWorldSoundAtPosition = 0x202,
    //    PlayWorldSoundOnObject = 0x203,
    //    PowerClear = 0x2E1,
    //    PowerGrant = 0x1C8,
    //    PowerInvoke = 0x2E3,
    //    PowerMenuEnable = 0x414,
    //    PowerRevoke = 0x1C9,
    //    PowerToInt = 0x401,
    //    PowerUsed = 0x3C4,
    //    PowerUserShutdown = 0x3FE,
    //    ProtoObjectListAdd = 0x239,
    //    ProtoObjectListRemove = 0x23A,
    //    ProtoObjectListShuffle = 0x12F,
    //    ProtoSquadListAdd = 0x23B,
    //    ProtoSquadListGetSize = 0x369,
    //    ProtoSquadListRemove = 0x23C,
    //    ProtoSquadListShuffle = 0x131,
    //    RallyPointClear = 0x2CE,
    //    RallyPointGet = 0x2CF,
    //    RallyPointSet = 0x2CD,
    //    RandomCount = 0xB2,
    //    RandomLocation = 0x37,
    //    RandomTime = 0x204,
    //    RecycleBuilding = 0x3BB,
    //    RefCountSquadAdd = 0x13C,
    //    RefCountSquadRemove = 0x13D,
    //    RefCountUnitAdd = 0x137,
    //    RefCountUnitRemove = 0x138,
    //    RefundCost = 0x33,
    //    ReinforceSquad = 0x1AE,
    //    Repair = 0x13E,
    //    RepairByCombatValue = 0x37B,
    //    ResetAbilityTimer = 0x408,
    //    ResetDopple = 0x3EF,
    //    Revealer = 0x11D,
    //    ReverseHotDrop = 0x3F8,
    //    RoundFloat = 0x2DF,
    //    RumbleStart = 0x305,
    //    RumbleStop = 0x306,
    //    SaveGame = 0x3F6,
    //    SensorLock = 0x1A3,
    //    SetAmmo = 0x186,
    //    SetAutoAttackable = 0x393,
    //    SetCamera = 0x374,
    //    SetCitizensSaved = 0x2A5,
    //    SetDirection = 0x1E9,
    //    SetFlagNearLayer = 0x21D,
    //    SetFollowCam = 0x209,
    //    SetGarrisonedCount = 0x3C8,
    //    SetHitZoneActive = 0x103,
    //    SetHitZoneHealth = 0x102,
    //    SetIgnoreUserInput = 0x17E,
    //    SetLevel = 0x334,
    //    SetMinimapNorthPointerRotation = 0x415,
    //    SetMinimapSkirtMirroring = 0x41E,
    //    SetMobile = 0x1FE,
    //    SetOccluded = 0x218,
    //    SetOverrideTint = 0x307,
    //    SetPlayableBounds = 0x2C8,
    //    SetPlayerPop = 0x2E5,
    //    SetPlayerState = 0x82,
    //    SetPosition = 0x219,
    //    SetPowerAvailableTime = 0x413,
    //    SetRenderTerrainSkirt = 0x214,
    //    SetResourceHandicap = 0x313,
    //    SetResources = 0x115,
    //    SetResourcesTotals = 0x151,
    //    SetScenarioScore = 0x340,
    //    SetScenarioScoreInfo = 0x3D8,
    //    SetSelectable = 0x384,
    //    SetTeleporterDestination = 0x3C7,
    //    Settle = 0x167,
    //    SetTowerWallDestination = 0x3E9,
    //    SetTransportPickUpLocations = 0x309,
    //    SetTrickleRate = 0x385,
    //    SetUIPowerRadius = 0x4F,
    //    SetUnitAttackTarget = 0x2C6,
    //    ShowCitizensSaved = 0x2A4,
    //    ShowGarrisonedCount = 0x2A3,
    //    ShowInfoDialog = 0x3FD,
    //    ShowMessage = 0x388,
    //    ShowObjectCounter = 0x2DC,
    //    ShowObjectivePointer = 0x2AF,
    //    SquadFlagSet = 0x38E,
    //    SquadListAdd = 0x95,
    //    SquadListDiff = 0x14E,
    //    SquadListFilter = 0x15E,
    //    SquadListGetSize = 0x93,
    //    SquadListPartition = 0x128,
    //    SquadListRemove = 0x97,
    //    SquadListShuffle = 0x129,
    //    TableLoad = 0x31C,
    //    TeamListAdd = 0x7E,
    //    TeamListRemove = 0x7F,
    //    TeamListShuffle = 0x12D,
    //    TeamSetDiplomacy = 0x3AD,
    //    TeamsToPlayers = 0x1B1,
    //    TechActivate = 0x3C,
    //    TechDeactivate = 0x3D,
    //    TechListAdd = 0x23D,
    //    TechListRemove = 0x23E,
    //    TechListShuffle = 0x132,
    //    Teleport = 0x162,
    //    TeleportUnitsOffObstruction = 0x424,
    //    TimeListAdd = 0x359,
    //    TimeListGetSize = 0x35B,
    //    TimeListRemove = 0x35A,
    //    TimerGet = 0x30C,
    //    TimerSet = 0x30B,
    //    TimerSetPaused = 0x30D,
    //    TimeToFloat = 0x2EF,
    //    TimeUserMessage = 0xA7,
    //    TransferGarrisoned = 0x301,
    //    TransferGarrisonedSquad = 0x300,
    //    Transform = 0xAC,
    //    TransportSquads = 0x45,
    //    TriggerActivate = 0x1F,
    //    TriggerDeactivate = 0x20,
    //    UITogglePowerOverlay = 0x2D6,
    //    UIUnlock = 0x14A,
    //    UnitFlagSet = 0x13F,
    //    UnitListAdd = 0x94,
    //    UnitListDiff = 0x14F,
    //    UnitListFilter = 0x15D,
    //    UnitListGetSize = 0x92,
    //    UnitListPartition = 0x134,
    //    UnitListRemove = 0x96,
    //    UnitListShuffle = 0x12E,
    //    Unload = 0x41,
    //    Unpack = 0x317,
    //    UpdateObjectCounter = 0x2DD,
    //    UsePower = 0x4D,
    //    UserMessage = 0xA4,
    //    Work = 0x75,
    //}; 
    //public enum ConditionType
    //{
    //    Invalid = 0x0,
    //    AICanGetDifficultySetting = 920,
    //    AICanGetTopicFocus = 883,
    //    AITopicGetTickets = 1059,
    //    AITopicIsActive = 600,
    //    ASYNCUnitsOnScreenSelected = 1053,
    //    BidState = 586,
    //    BuildingCommandDone = 596,
    //    CanGetBuilder = 524,
    //    CanGetCentroid = 621,
    //    CanGetCoopPlayer = 991,
    //    CanGetCorpseUnits = 844,
    //    CanGetDesignSpheres = 974,
    //    CanGetGreatestThreat = 743,
    //    CanGetHoverPoint = 990,
    //    CanGetObjects = 819,
    //    CanGetOneDesignLine = 865,
    //    CanGetOneFloat = 885,
    //    CanGetOneInteger = 552,
    //    CanGetOneLocation = 565,
    //    CanGetOneObject = 835,
    //    CanGetOneObjectType = 487,
    //    CanGetOnePlayer = 434,
    //    CanGetOneProtoObject = 485,
    //    CanGetOneProtoSquad = 486,
    //    CanGetOneSocketUnit = 909,
    //    CanGetOneSquad = 357,
    //    CanGetOneTeam = 435,
    //    CanGetOneTech = 488,
    //    CanGetOneTime = 860,
    //    CanGetOneUnit = 356,
    //    CanGetRandomLocation = 956,
    //    CanGetSocketParentBuilding = 954,
    //    CanGetSocketPlugUnit = 986,
    //    CanGetSocketUnits = 908,
    //    CanGetSquads = 4,
    //    CanGetTargetedSquad = 744,
    //    CanGetUnitLaunchLocation = 714,
    //    CanGetUnits = 2,
    //    CanGetUnitsAlongRay = 558,
    //    CanPayCost = 15,
    //    CanRemoveOneFloat = 975,
    //    CanRemoveOneInteger = 563,
    //    CanRemoveOneLocation = 566,
    //    CanRemoveOneProtoObject = 583,
    //    CanRemoveOneProtoSquad = 584,
    //    CanRemoveOneTech = 585,
    //    CanRetrieveExternalFlag = 877,
    //    CanRetrieveExternalFloat = 977,
    //    CanRetrieveExternalLocation = 440,
    //    CanRetrieveExternalLocationList = 441,
    //    CanRetrieveExternals = 192,
    //    CanUsePower = 27,
    //    ChatCompleted = 905,
    //    CheckAndSetFalse = 1056,
    //    CheckDifficulty = 972,
    //    CheckDiplomacy = 362,
    //    CheckModeChange = 386,
    //    CheckPlacement = 82,
    //    CheckPop = 763,
    //    CheckResourceTotals = 340,
    //    CompareAIMissionState = 547,
    //    CompareAIMissionTargetType = 548,
    //    CompareAIMissionType = 546,
    //    CompareAmmoPercent = 256,
    //    CompareBool = 114,
    //    CompareCiv = 240,
    //    CompareCost = 339,
    //    CompareCount = 14,
    //    CompareDesignLine = 866,
    //    CompareFloat = 453,
    //    CompareHitpoints = 182,
    //    CompareLeader = 476,
    //    CompareLocStringID = 825,
    //    ComparePercent = 175,
    //    ComparePlayers = 428,
    //    ComparePlayerSquadCount = 606,
    //    ComparePlayerUnitCount = 436,
    //    ComparePopulation = 352,
    //    CompareProtoObject = 575,
    //    CompareProtoSquad = 513,
    //    CompareString = 171,
    //    CompareTeams = 429,
    //    CompareTech = 576,
    //    CompareTime = 170,
    //    CompareUnit = 1028,
    //    CompareVector = 752,
    //    ConceptCompareState = 932,
    //    ConceptGetCommand = 930,
    //    ConceptGetStateChange = 931,
    //    ContainsGarrisoned = 136,
    //    CustomCommandCheck = 635,
    //    EventTriggered = 813,
    //    GameTime = 266,
    //    GameTimeReached = 268,
    //    GetTableRow = 762,
    //    HasAttached = 508,
    //    HasCinematicTagFired = 518,
    //    HasGarrisoned = 602,
    //    HasHitched = 801,
    //    IsAlive = 80,
    //    IsAttached = 604,
    //    IsAttacking = 709,
    //    IsAutoAttackable = 916,
    //    IsBeingGatheredFrom = 918,
    //    IsBuilt = 414,
    //    IsCapturing = 723,
    //    IsConfigDefined = 629,
    //    IsDead = 81,
    //    IsEmptySocketUnit = 942,
    //    IsForbidden = 944,
    //    IsGarrisoned = 603,
    //    IsGathering = 722,
    //    IsHitched = 800,
    //    IsHitZoneActive = 260,
    //    IsIdle = 264,
    //    IsInQueue = 814,
    //    IsMobile = 509,
    //    IsMoving = 416,
    //    IsObjectiveComplete = 746,
    //    IsObjectType = 649,
    //    IsOwnedBy = 29,
    //    IsPassable = 611,
    //    IsProtoObject = 30,
    //    IsSelectable = 899,
    //    IsSquadAtMaxSize = 438,
    //    IsTimerDone = 661,
    //    IsUnderAttack = 461,
    //    IsUnitPower = 28,
    //    IsUserModeNormal = 789,
    //    MarkerSquadsInArea = 980,
    //    NextKBBase = 444,
    //    NextKBSquad = 662,
    //    NextLocation = 292,
    //    NextObject = 282,
    //    NextObjectType = 496,
    //    NextPlayer = 118,
    //    NextProtoObject = 494,
    //    NextProtoSquad = 495,
    //    NextSquad = 141,
    //    NextTeam = 119,
    //    NextTech = 497,
    //    NextUnit = 140,
    //    PlayerInState = 131,
    //    PlayerIsComputerAI = 740,
    //    PlayerIsGaia = 427,
    //    PlayerIsHuman = 426,
    //    PlayerIsPrimaryUser = 624,
    //    PlayerLookingAtUnit = 21,
    //    PlayerSelectingSquad = 280,
    //    PlayerSelectingUnit = 20,
    //    PlayerUsingLeader = 477,
    //    ProtoObjectListContains = 577,
    //    ProtoSquadListContains = 578,
    //    RandomListLocation = 293,
    //    RefCountSquad = 315,
    //    RefCountUnit = 314,
    //    SquadFlag = 911,
    //    SquadLocationDistance = 238,
    //    SquadSquadDistance = 807,
    //    TechListContains = 579,
    //    TechStatus = 5,
    //    TriggerActiveTime = 7,
    //    TriggerActiveTimeReached = 269,
    //    UIButtonPressed = 527,
    //    UIButtonWaiting = 528,
    //    UILocationCancel = 17,
    //    UILocationMinigameCancel = 895,
    //    UILocationMinigameOK = 894,
    //    UILocationMinigameUILockError = 896,
    //    UILocationMinigameWaiting = 893,
    //    UILocationOK = 16,
    //    UILocationUILockError = 327,
    //    UILocationWaiting = 331,
    //    UISquadCancel = 158,
    //    UISquadListCancel = 641,
    //    UISquadListOK = 640,
    //    UISquadListUILockError = 642,
    //    UISquadListWaiting = 643,
    //    UISquadOK = 157,
    //    UISquadUILockError = 329,
    //    UISquadWaiting = 333,
    //    UIUnitCancel = 156,
    //    UIUnitOK = 155,
    //    UIUnitUILockError = 328,
    //    UIUnitWaiting = 332,
    //    UnitFlag = 320,
    //    UnitListLocationDistance = 128,
    //    UnitLocationDistance = 9,
    //    UnitUnitDistance = 8,
    //};



    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    public class Group
    {
        [YAXAttributeForClass()]
        public int ID { get; set; }

        [YAXAttributeForClass()]
        public string Name { get; set; }

        [YAXValueForClass()]
        [YAXCollection(YAXCollectionSerializationTypes.Serially, SeparateBy = ",")]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        public List<int> Values { get; set; }
    }


    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    public class Var
    {
        [YAXAttributeForClass()]
        public int ID { get; set; }

        [YAXDontSerialize()]
        public VarType Type { get; set; }

        [YAXAttributeForClass()]
        [YAXSerializeAs("Type")]
        private string TypeSer
        {
            get { return Type.ToString(); }
            set { Type = TypeFromString(value); }
        }

        [YAXAttributeForClass()]
        public string Name { get; set; }

        [YAXAttributeForClass()]
        public bool IsNull { get; set; }

        [YAXValueForClass()]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        public string Value { get; set; }

        [YAXAttributeForClass()]
        [YAXCollection(YAXCollectionSerializationTypes.Serially, SeparateBy = ",")]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        public List<int> Refs { get; set; }

        public override string ToString()
        {
            if (Name == "") return "\"\"";
            else return Name;
        }
    }

    public class LogicParam
    {
        public LogicParam()
        {

        }
        public LogicParam(LogicParam copy) : this()
        {
            Name = copy.Name;
            SigID = copy.SigID;
            Optional = copy.Optional;
            Value = copy.Value;
        }

        [YAXDontSerialize()]
        public string Name { get; set; }

        [YAXAttributeForClass()]
        public int SigID { get; set; }

        [YAXDontSerialize()]
        public bool Optional { get; set; }

        [YAXValueForClass()]
        public int Value { get; set; }
    }

    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    public abstract class Logic
    {
        public Logic()
        {
            ParamValues = new Dictionary<int, int>();
        }

        public abstract string TypeName { get; }

        [YAXAttributeForClass()]
        public int Version { get; set; }

        [YAXAttributeForClass()]
        public int DBID { get; set; }

        [YAXErrorIfMissed(YAXExceptionTypes.Ignore, DefaultValue = "")]
        [YAXAttributeForClass()]
        public string Comment { get; set; }

        public abstract Dictionary<int, LogicParamInfo> StaticParamInfo { get; }

        [YAXDontSerialize]
        private Dictionary<int, int> ParamValues { get; set; }

        [YAXValueForClass()]
        [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Input")]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        public List<LogicParam> Inputs
        {
            get
            {
                List<LogicParam> ret = new List<LogicParam>();
                foreach (var p in StaticParamInfo)
                {
                    if (p.Value.Output) continue;
                    ret.Add(new LogicParam()
                    {
                        Name = p.Value.Name,
                        Optional = p.Value.Optional,
                        SigID = p.Key,
                        Value = GetValueOfParam(p.Key)
                    });
                }
                return ret;
            }
            set
            {
                if (value == null) return;
                foreach (var v in value)
                {
                    SetValueOfParam(v.SigID, v.Value);
                }
            }
        }
        [YAXValueForClass()]
        [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Output")]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        public List<LogicParam> Outputs
        {
            get
            {
                List<LogicParam> ret = new List<LogicParam>();
                foreach (var p in StaticParamInfo)
                {
                    if (!p.Value.Output) continue;
                    ret.Add(new LogicParam()
                    {
                        Name = p.Value.Name,
                        Optional = p.Value.Optional,
                        SigID = p.Key,
                        Value = GetValueOfParam(p.Key)
                    });
                }
                return ret;
            }
            set
            {
                if (value == null) return;
                foreach (var v in value)
                {
                    SetValueOfParam(v.SigID, v.Value);
                }
            }
        }

        public int GetValueOfParam(int sigID)
        {
            if (!StaticParamInfo.ContainsKey(sigID)) return -1;

            if (!ParamValues.ContainsKey(sigID)) ParamValues.Add(sigID, -1);
            return ParamValues[sigID];
        }
        public void SetValueOfParam(int sigID, int value)
        {
            if (!StaticParamInfo.ContainsKey(sigID)) return;

            if (!ParamValues.ContainsKey(sigID)) ParamValues.Add(sigID, value);
            ParamValues[sigID] = value;
        }
    }

    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    public class Effect : Logic
    {
        public Effect() : base() { }

        [YAXAttributeForClass()]
        [YAXSerializeAs("Type")]
        public override string TypeName { get { return LogicName(LogicType.Effect, DBID); } }

        [YAXDontSerialize]
        public override Dictionary<int, LogicParamInfo> StaticParamInfo
        {
            get
            {
                //why was this here?
                //if ((cachedDBID == -1 && cachedVersion == -1)
                //    ||
                //    (cachedDBID != DBID && cachedVersion != Version))
                //{
                //    cachedDBID = DBID;
                //    cachedVersion = Version;
                //    cachedParams = LogicParamInfos(LogicType.Effect, DBID, Version);
                //}
                //return cachedParams;
                return LogicParamInfos(LogicType.Effect, DBID, Version);
            }
        }
        [YAXDontSerialize]
        private int cachedDBID = -1;
        [YAXDontSerialize]
        private int cachedVersion = -1;
        [YAXDontSerialize]
        private Dictionary<int, LogicParamInfo> cachedParams;
    }

    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    public class Condition : Logic
    {
        public Condition() : base() { }

        [YAXAttributeForClass()]
        [YAXSerializeAs("Type")]
        public override string TypeName { get { return LogicName(LogicType.Condition, DBID); } }

        [YAXAttributeForClass()]
        public bool Invert { get; set; }

        [YAXAttributeForClass()]
        public bool Async { get; set; }

        [YAXAttributeForClass()]
        public int AsyncParameterKey { get; set; }

        [YAXDontSerialize]
        public override Dictionary<int, LogicParamInfo> StaticParamInfo
        {
            get
            {
                //why was this here?
                //if ((cachedDBID == -1 && cachedVersion == -1)
                //    ||
                //    (cachedDBID != DBID && cachedVersion != Version))
                //{
                //    cachedDBID = DBID;
                //    cachedVersion = Version;
                //    cachedParams = LogicParamInfos(LogicType.Condition, DBID, Version);
                //}
                //return cachedParams;
                return LogicParamInfos(LogicType.Condition, DBID, Version);
            }
        }
        [YAXDontSerialize]
        private int cachedDBID = -1;
        [YAXDontSerialize]
        private int cachedVersion = -1;
        [YAXDontSerialize]
        private Dictionary<int, LogicParamInfo> cachedParams;
    }


    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields, Options = YAXSerializationOptions.DisplayLineInfoInExceptions)]
    public class Trigger
    {
        public Trigger()
        {
            TriggerEffectsOnTrue = new List<Effect>();
            TriggerEffectsOnFalse = new List<Effect>();
            ConditionsAnd = new List<Condition>();
        }

        [YAXAttributeForClass()]
        public int ID { get; set; }
        [YAXAttributeForClass()]
        public string Name { get; set; }
        [YAXAttributeForClass()]
        public bool Active { get; set; }
        [YAXAttributeForClass()]
        public int EvaluateFrequency { get; set; }
        [YAXAttributeForClass()]
        public int EvalLimit { get; set; }
        [YAXAttributeForClass()]
        public bool CommentOut { get; set; }
        [YAXAttributeForClass()]
        public bool ConditionalTrigger { get; set; }
        [YAXAttributeForClass()]
        public int GroupID { get; set; }

        //[YAXAttributeForClass()]
        //[YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        //public float X { get; set; }
        //[YAXAttributeForClass()]
        //[YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        //public float Y { get; set; }


        [YAXAttributeForClass()]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        [YAXSerializeAs("X")]
        private float pX { get; set; }
        [YAXAttributeForClass()]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        [YAXSerializeAs("Y")]
        private float pY { get; set; }
        [YAXDontSerialize]
        public float X { get { return pX * TriggerSpacingMultiplier; } set { pX = value / TriggerSpacingMultiplier; } }
        [YAXDontSerialize]
        public float Y { get { return pY * TriggerSpacingMultiplier; } set { pY = value / TriggerSpacingMultiplier; } }


        [YAXCollection(YAXCollectionSerializationTypes.Recursive, EachElementName = "Condition")]
        [YAXElementFor("TriggerConditions")]
        [YAXSerializeAs("Or")]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        private List<Condition> ConditionsOr
        {
            get
            {
                if (!ConditionsAreAND) return Conditions;
                else return null;
            }
            set
            {
                ConditionsAreAND = false;
                if (value == null) Conditions = new List<Condition>();
                else Conditions = value;
            }
        }

        [YAXCollection(YAXCollectionSerializationTypes.Recursive, EachElementName = "Condition")]
        [YAXElementFor("TriggerConditions")]
        [YAXSerializeAs("And")]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        private List<Condition> ConditionsAnd
        {
            get
            {
                if (ConditionsAreAND) return Conditions;
                else return null;
            }
            set
            {
                ConditionsAreAND = true;
                if (value == null) Conditions = new List<Condition>();
                else Conditions = value;
            }
        }

        [YAXDontSerialize()]
        public bool ConditionsAreAND { get; private set; }
        [YAXDontSerialize()]
        public List<Condition> Conditions { get; set; }

        [YAXValueForClass()]
        [YAXCollection(YAXCollectionSerializationTypes.Recursive, EachElementName = "Effect")]
        [YAXDontSerializeIfNull()]
        public List<Effect> TriggerEffectsOnTrue { get; set; }

        [YAXValueForClass()]
        [YAXCollection(YAXCollectionSerializationTypes.Recursive, EachElementName = "Effect")]
        [YAXDontSerializeIfNull()]
        public List<Effect> TriggerEffectsOnFalse { get; set; }
    }


    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields, Options = YAXSerializationOptions.DisplayLineInfoInExceptions)]
    public class Triggerscript
    {
        public Triggerscript()
        {
            TriggerGroups = new Dictionary<int, Group>();
            TriggerVars = new Dictionary<int, Var>();
            Triggers = new Dictionary<int, Trigger>();
            Name = "";
            Type = "TriggerScript";
        }

        [YAXAttributeForClass()]
        public string Name { get; set; }

        [YAXAttributeForClass()]
        public string Type { get; set; }

        [YAXAttributeForClass()]
        public int NextTriggerVarID { get; set; }

        [YAXAttributeForClass()]
        public int NextTriggerID { get; set; }

        [YAXAttributeForClass()]
        public int NextConditionID { get; set; }

        [YAXAttributeForClass()]
        public int NextEffectID { get; set; }

        [YAXErrorIfMissed(YAXExceptionTypes.Ignore, DefaultValue = true)]
        [YAXAttributeForClass()]
        public bool External { get; set; }


        //YAX needs a list, we want to expose a dictionary for fast acces via id.
        [YAXCollection(YAXCollectionSerializationTypes.Recursive, EachElementName = "Group")]
        [YAXSerializeAs("TriggerGroups")]
        private List<Group> TriggerGroupsSer
        {
            get { return TriggerGroups.Values.ToList(); }
            set
            {
                TriggerGroups.Clear();
                foreach (var g in value)
                {
                    TriggerGroups.Add(g.ID, g);
                }
            }
        }
        [YAXDontSerialize]
        public Dictionary<int, Group> TriggerGroups { get; set; }


        //YAX needs a list, we want to expose a dictionary for fast acces via id.
        [YAXCollection(YAXCollectionSerializationTypes.Recursive, EachElementName = "TriggerVar")]
        [YAXSerializeAs("TriggerVars")]
        private List<Var> TriggerVarsSer
        {
            //get; set;
            get { return TriggerVars.Values.ToList(); }
            set
            {
                TriggerVars.Clear();
                foreach (var v in value)
                {
                    TriggerVars.Add(v.ID, v);
                }
            }
        }
        [YAXDontSerialize]
        public Dictionary<int, Var> TriggerVars { get; set; }


        //YAX needs a list, we want to expose a dictionary for fast acces via id.
        [YAXCollection(YAXCollectionSerializationTypes.Recursive, EachElementName = "Trigger")]
        [YAXSerializeAs("Triggers")]
        private List<Trigger> TriggersSer
        {
            get { return Triggers.Values.ToList(); }
            set
            {
                Triggers.Clear();
                foreach (var t in value)
                {
                    Triggers.Add(t.ID, t);
                }
            }
        }
        [YAXDontSerialize]
        public Dictionary<int, Trigger> Triggers { get; set; }
    }
}
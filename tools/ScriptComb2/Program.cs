using System.Text.RegularExpressions;
using YAXLib;
using YAXLib.Attributes;
using static Program;
using Foundry.HW1.Triggerscript;
using static Foundry.HW1.Triggerscript.EditorHelpers;
using SharpDX.Direct2D1;
using Foundry.HW1.Triggerscript;

class Program
{
    private static List<string> types = new List<string>();
    public static void Main(string[] args)
    {
        if (args.Length < 2) return;
        string projectDir = args[0];
        string outputDir = args[0] + "../../foundry/hw1/triggerscript/";
        //YAXSerializer ser = new YAXSerializer(typeof(Dictionary<int, Dictionary<int, ProtoLogic>>));
        YAXSerializer ser = new YAXSerializer(typeof(LogicDatabase));



        //variable types
        //ser = new YAXSerializer(typeof(< string >));
        //List<string> varTypes = GetVarTypes();
        //ser.SerializeToFile(varTypes, outputDir + "variables.tstyp");



        ////effects
        //List<ProtoLogic> effectItems = new List<ProtoLogic>();
        //effectItems.AddRange(GetItems(File.ReadAllText(projectDir + "triggereffect.cpp"), "void BTriggerEffect::te"));
        //effectItems.AddRange(GetItems(File.ReadAllText(projectDir + "triggereffectai.cpp"), "void BTriggerEffect::te"));
        //effectItems.AddRange(GetItems(File.ReadAllText(projectDir + "triggereffectcopy.cpp"), "void BTriggerEffect::te"));

        Dictionary<string, int> effectDBIDs = GetDBIDs(File.ReadAllText(projectDir + "triggereffect.h"), "cTE");

        LogicDatabase db = GetItems(File.ReadAllText(projectDir + "triggereffect.cpp"), "void BTriggerEffect::te", effectDBIDs);
        LogicDatabase dbai = GetItems(File.ReadAllText(projectDir + "triggereffectai.cpp"), "void BTriggerEffect::te", effectDBIDs);
        LogicDatabase dbcopy = GetItems(File.ReadAllText(projectDir + "triggereffectcopy.cpp"), "void BTriggerEffect::te", effectDBIDs);

        foreach (var v in dbai.Types) db.Types.Add(v.Key, v.Value);
        foreach (var v in dbcopy.Types) db.Types.Add(v.Key, v.Value);
        ser.SerializeToFile(db, outputDir + "effects.tsdef");



        Dictionary<string, int> conditionDBIDs = GetDBIDs(File.ReadAllText(projectDir + "triggercondition.h"), "cTC");

        LogicDatabase cdb = GetItems(File.ReadAllText(projectDir + "triggercondition.cpp"), "bool BTriggerCondition::tc", conditionDBIDs);
        ser.SerializeToFile(cdb, outputDir + "conditions.tsdef");


        //List<string> effectCategories = File.ReadAllLines(projectDir + "")

        //Dictionary<int, Dictionary<int, ProtoLogic>> effects = new Dictionary<int, Dictionary<int, ProtoLogic>>();
        //foreach(ProtoLogic item in effectItems)
        //{
        //    if(effectDBIDs.ContainsKey(item.Name))
        //    {
        //        item.DBID = effectDBIDs[item.Name];

        //        if(!effects.ContainsKey(item.DBID))
        //            effects.Add(item.DBID, new Dictionary<int, ProtoLogic>());

        //        effects[item.DBID].Add(item.Version, item);
        //    }
        //}
        //ser.SerializeToFile(effects, outputDir + "effects.tsdef");


        ////conditions
        //List<ProtoLogic> conditionItems = new List<ProtoLogic>();
        //conditionItems.AddRange(GetItems(File.ReadAllText(projectDir + "triggercondition.cpp"), "bool BTriggerCondition::tc"));

        //Dictionary<string, int> conditionDBIDs = GetDBIDs(File.ReadAllText(projectDir + "triggercondition.h"), "cTC");

        //Dictionary<int, Dictionary<int, ProtoLogic>> conditions = new Dictionary<int, Dictionary<int, ProtoLogic>>();
        //foreach (ProtoLogic item in conditionItems)
        //{
        //    if (conditionDBIDs.ContainsKey(item.Name))
        //    {
        //        item.DBID = conditionDBIDs[item.Name];

        //        if (!conditions.ContainsKey(item.DBID))
        //            conditions.Add(item.DBID, new Dictionary<int, ProtoLogic>());

        //        conditions[item.DBID].Add(item.Version, item);
        //    }
        //}
        //ser.SerializeToFile(conditions, outputDir + "conditions.tsdef");
    }

    public static LogicDatabase GetItems(string file, string prefix, Dictionary<string, int> dbids)
    {
        LogicDatabase db = new LogicDatabase();

        int offset = 0;
        while (file.IndexOf(prefix, offset) != -1)
        {
            offset = file.IndexOf(prefix, offset);
            offset += prefix.Length;
            
            string name = file.Substring(offset, file.IndexOf("()", offset) - offset);

            var versionMatch = Regex.Match(name, @"V?([0-9]+)", RegexOptions.RightToLeft);
            int version = 0;
            if (versionMatch.Success)
            {
                version = int.Parse(versionMatch.Groups[1].Value);
            }
            name = name.Substring(0, name.Length - versionMatch.Length);

            offset = file.IndexOf("{", offset) + 1;
            

            int start = offset;
            int len = 0;
            int opens = 1;
            while (opens != 0)
            {
                if (file[offset] == '{') opens++;
                if (file[offset] == '}') opens--;

                offset++;
                len++;
            }

            LogicTypeInfo item = new LogicTypeInfo()
            {
                Name = name,
            };

            try
            {
                if (!db.Types.ContainsKey(dbids[name]))
                {
                    db.Types.Add(dbids[name], item);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            string body = file.Substring(start, len);

            Match match = Regex.Match(body, @"enum[\s]*[\n]*{([\s\S]*)};");
            if (match.Success)
            {
                LogicVersionInfo vinfo = new LogicVersionInfo();

                string enumstr = Regex.Replace(match.Groups[1].Value, @"[^\w=,]*", "");
                string[] enums = enumstr.Split(",");
                int tracker = 0;
                foreach (string e in enums)
                {
                    if (e == "") continue;

                    string[] keyval = e.Split("=");

                    string pname = keyval[0].Substring(1);

                    if (pname.StartsWith("Input")) pname = pname.Substring(5);
                    else if (pname.StartsWith("Output")) pname = pname.Substring(6);

                    int pid = keyval.Length > 1 ? int.Parse(keyval[1]) : tracker;
                    tracker = pid + 1;

                    bool poptional = Regex.Match(body, string.Format("{0}\\)->isUsed", pname)).Success;
                    bool poutput = Regex.Match(body, string.Format("{0}\\)->as[\\w]*\\(\\)->writeVar", pname)).Success;

                    string ptypestr = "Unused";
                    Match ptype = Regex.Match(body, string.Format(@"{0}\)->as([\w]*)\(\)", pname));
                    if(ptype.Success)
                    {
                        ptypestr = ptype.Groups[1].Value;
                    }

                    try
                    {
                        vinfo.Params.Add(pid, new LogicParamInfo()
                        {
                            Name = pname,
                            Optional = poptional,
                            Output = poutput,
                            Type = Foundry.HW1.Triggerscript.Database.TypeFromString(ptypestr),
                        });
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }
                try
                {
                    db.Types[dbids[name]].Versions.Add(version, vinfo);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }

        return db;
    }
    public static Dictionary<string, string> GetEnums(string file, string prefix, string func)
    {
        Dictionary<string, string> items = new Dictionary<string, string>();

        Match match = Regex.Match(file, func);
        if (!match.Success) return items;

        int offset = match.Index;
        offset = file.IndexOf('{', offset) + 1;
        int start = offset;
        int len = 0;
        int opens = 1;
        while (opens != 0)
        {
            char aaaaaaa = file[offset];
            if (file[offset] == '{') 
                opens++;
            if (file[offset] == '}') 
                opens--;

            offset++;
            len++;
        }

        string block = file.Substring(start, len);

        offset = 0;
        while(true)
        {
            match = Regex.Match(block.Substring(offset), "case " + prefix + @"([\w]*):\s*{ ([\w]*)\(\)");
            if (!match.Success) break;

            offset += match.Index + match.Length;

            items.Add(match.Groups[1].Value, match.Groups[2].Value);
        }

        return items;
    }
    public static Dictionary<string, int> GetDBIDs(string file, string prefix)
    {
        Dictionary<string, int> items = new Dictionary<string, int>();

        int offset = 0;
        while(true)
        {
            Match match = Regex.Match(file.Substring(offset), prefix + @"([\w]*)[\s]*= ([0-9]*)");
            if (!match.Success) break;

            offset += match.Index + match.Length;

            items.Add(match.Groups[1].Value, int.Parse(match.Groups[2].Value));
        }

        return items;
    }

    public static List<string> GetVarTypes()
    {
        List<string> ret = new List<string>();

        string file =
@"   BTriggerVarTech* asTech(void) { BASSERT(isType(cVarTypeTech)); return(reinterpret_cast<BTriggerVarTech*>(this)); }
   BTriggerVarTechStatus* asTechStatus(void) { BASSERT(isType(cVarTypeTechStatus)); return(reinterpret_cast<BTriggerVarTechStatus*>(this)); }
   BTriggerVarOperator* asOperator(void) { BASSERT(isType(cVarTypeOperator)); return(reinterpret_cast<BTriggerVarOperator*>(this)); }
   BTriggerVarProtoObject* asProtoObject(void) { BASSERT(isType(cVarTypeProtoObject)); return(reinterpret_cast<BTriggerVarProtoObject*>(this)); }
   BTriggerVarObjectType* asObjectType(void) { BASSERT(isType(cVarTypeObjectType)); return(reinterpret_cast<BTriggerVarObjectType*>(this)); }
   BTriggerVarProtoSquad* asProtoSquad(void) { BASSERT(isType(cVarTypeProtoSquad)); return(reinterpret_cast<BTriggerVarProtoSquad*>(this)); }
   BTriggerVarSound* asSound(void) { BASSERT(isType(cVarTypeSound)); return(reinterpret_cast<BTriggerVarSound*>(this)); }
   BTriggerVarEntity* asEntity(void) { BASSERT(isType(cVarTypeEntity)); return(reinterpret_cast<BTriggerVarEntity*>(this)); }
   BTriggerVarEntityList* asEntityList(void) { BASSERT(isType(cVarTypeEntityList)); return(reinterpret_cast<BTriggerVarEntityList*>(this)); }
   BTriggerVarTrigger* asTrigger(void) { BASSERT(isType(cVarTypeTrigger)); return(reinterpret_cast<BTriggerVarTrigger*>(this)); }
   BTriggerVarTime* asTime(void) { BASSERT(isType(cVarTypeTime)); return(reinterpret_cast<BTriggerVarTime*>(this)); }
   BTriggerVarPlayer* asPlayer(void) { BASSERT(isType(cVarTypePlayer)); return(reinterpret_cast<BTriggerVarPlayer*>(this)); }
   BTriggerVarUILocation* asUILocation(void) { BASSERT(isType(cVarTypeUILocation)); return(reinterpret_cast<BTriggerVarUILocation*>(this)); }
   BTriggerVarUIEntity* asUIEntity(void) { BASSERT(isType(cVarTypeUIEntity)); return(reinterpret_cast<BTriggerVarUIEntity*>(this)); }
   BTriggerVarCost* asCost(void) { BASSERT(isType(cVarTypeCost)); return(reinterpret_cast<BTriggerVarCost*>(this)); }
   BTriggerVarAnimType* asAnimType(void) { BASSERT(isType(cVarTypeAnimType)); return(reinterpret_cast<BTriggerVarAnimType*>(this)); }
   BTriggerVarActionStatus* asActionStatus(void) { BASSERT(isType(cVarTypeActionStatus)); return(reinterpret_cast<BTriggerVarActionStatus*>(this)); }
   BTriggerVarPower* asPower(void) { BASSERT(isType(cVarTypePower)); return(reinterpret_cast<BTriggerVarPower*>(this)); }
   BTriggerVarBool* asBool(void) { BASSERT(isType(cVarTypeBool)); return(reinterpret_cast<BTriggerVarBool*>(this)); }
   BTriggerVarFloat* asFloat(void) { BASSERT(isType(cVarTypeFloat)); return(reinterpret_cast<BTriggerVarFloat*>(this)); }
   BTriggerVarIterator* asIterator(void) { BASSERT(isType(cVarTypeIterator)); return(reinterpret_cast<BTriggerVarIterator*>(this)); }
   BTriggerVarTeam* asTeam(void) { BASSERT(isType(cVarTypeTeam)); return(reinterpret_cast<BTriggerVarTeam*>(this)); }
   BTriggerVarPlayerList* asPlayerList(void) { BASSERT(isType(cVarTypePlayerList)); return(reinterpret_cast<BTriggerVarPlayerList*>(this)); }
   BTriggerVarTeamList* asTeamList(void) { BASSERT(isType(cVarTypeTeamList)); return(reinterpret_cast<BTriggerVarTeamList*>(this)); }
   BTriggerVarPlayerState* asPlayerState(void) { BASSERT(isType(cVarTypePlayerState)); return(reinterpret_cast<BTriggerVarPlayerState*>(this)); }
   BTriggerVarObjective* asObjective(void) { BASSERT(isType(cVarTypeObjective)); return(reinterpret_cast<BTriggerVarObjective*>(this)); }
   BTriggerVarUnit* asUnit(void) { BASSERT(isType(cVarTypeUnit)); return(reinterpret_cast<BTriggerVarUnit*>(this)); }
   BTriggerVarUnitList* asUnitList(void) { BASSERT(isType(cVarTypeUnitList)); return(reinterpret_cast<BTriggerVarUnitList*>(this)); }
   BTriggerVarSquad* asSquad(void) { BASSERT(isType(cVarTypeSquad)); return(reinterpret_cast<BTriggerVarSquad*>(this)); }
   BTriggerVarSquadList* asSquadList(void) { BASSERT(isType(cVarTypeSquadList)); return(reinterpret_cast<BTriggerVarSquadList*>(this)); }
   BTriggerVarUIUnit* asUIUnit(void) { BASSERT(isType(cVarTypeUIUnit)); return(reinterpret_cast<BTriggerVarUIUnit*>(this)); }
   BTriggerVarUISquad* asUISquad(void) { BASSERT(isType(cVarTypeUISquad)); return(reinterpret_cast<BTriggerVarUISquad*>(this)); } 
   BTriggerVarUISquadList* asUISquadList() { BASSERT(isType(cVarTypeUISquadList)); return (reinterpret_cast<BTriggerVarUISquadList*>(this)); }
   BTriggerVarString* asString(void){ BASSERT(isType(cVarTypeString)); return(reinterpret_cast<BTriggerVarString*>(this)); }
   BTriggerVarMessageIndex* asMessageIndex(void){ BASSERT(isType(cVarTypeMessageIndex)); return(reinterpret_cast<BTriggerVarMessageIndex*>(this)); }
   BTriggerVarMessageJustify* asMessageJustify(void){ BASSERT(isType(cVarTypeMessageJustify)); return(reinterpret_cast<BTriggerVarMessageJustify*>(this)); }
   BTriggerVarMessagePoint* asMessagePoint(void){ BASSERT(isType(cVarTypeMessagePoint)); return(reinterpret_cast<BTriggerVarMessagePoint*>(this)); }
   BTriggerVarColor* asColor(void){ BASSERT(isType(cVarTypeColor)); return(reinterpret_cast<BTriggerVarColor*>(this)); }
   BTriggerVarProtoObjectList* asProtoObjectList(void) { BASSERT(isType(cVarTypeProtoObjectList)); return(reinterpret_cast<BTriggerVarProtoObjectList*>(this)); }
   BTriggerVarObjectTypeList* asObjectTypeList(void) { BASSERT(isType(cVarTypeObjectTypeList)); return(reinterpret_cast<BTriggerVarObjectTypeList*>(this)); }
   BTriggerVarProtoSquadList* asProtoSquadList(void) { BASSERT(isType(cVarTypeProtoSquadList)); return(reinterpret_cast<BTriggerVarProtoSquadList*>(this)); }
   BTriggerVarTechList* asTechList(void) { BASSERT(isType(cVarTypeTechList)); return(reinterpret_cast<BTriggerVarTechList*>(this)); }
   BTriggerVarMathOperator* asMathOperator( void ){ BASSERT( isType( cVarTypeMathOperator ) ); return( reinterpret_cast<BTriggerVarMathOperator*>( this ) ); }
   BTriggerVarObjectDataType* asObjectDataType( void ){ BASSERT( isType( cVarTypeObjectDataType ) ); return( reinterpret_cast<BTriggerVarObjectDataType*>( this ) ); }
   BTriggerVarObjectDataRelative* asObjectDataRelative( void ){ BASSERT( isType( cVarTypeObjectDataRelative ) ); return( reinterpret_cast<BTriggerVarObjectDataRelative*>( this ) ); }
   BTriggerVarCiv* asCiv(void) { BASSERT(isType(cVarTypeCiv)); return(reinterpret_cast<BTriggerVarCiv*>(this)); }
   BTriggerVarProtoObjectCollection* asProtoObjectCollection(void) { BASSERT(isType(cVarTypeProtoObjectCollection)); return(reinterpret_cast<BTriggerVarProtoObjectCollection*>(this)); }
   BTriggerVarObject* asObject(void) { BASSERT(isType(cVarTypeObject)); return(reinterpret_cast<BTriggerVarObject*>(this)); }
   BTriggerVarObjectList* asObjectList(void) { BASSERT(isType(cVarTypeObjectList)); return(reinterpret_cast<BTriggerVarObjectList*>(this)); }
   BTriggerVarGroup* asGroup(void) { BASSERT(isType(cVarTypeGroup)); return(reinterpret_cast<BTriggerVarGroup*>(this)); }
   BTriggerVarRefCountType* asRefCountType(void) { BASSERT(isType(cVarTypeRefCountType)); return(reinterpret_cast<BTriggerVarRefCountType*>(this)); }
   BTriggerVarUnitFlag* asUnitFlag(void) { BASSERT(isType(cVarTypeUnitFlag)); return(reinterpret_cast<BTriggerVarUnitFlag*>(this)); }
   BTriggerVarLOSType* asLOSType(void) { BASSERT(isType(cVarTypeLOSType)); return(reinterpret_cast<BTriggerVarLOSType*>(this)); }
   BTriggerVarEntityFilterSet* asEntityFilterSet(void) { BASSERT(isType(cVarTypeEntityFilterSet)); return(reinterpret_cast<BTriggerVarEntityFilterSet*>(this)); }
   BTriggerVarPopBucket* asPopBucket(void){ BASSERT(isType(cVarTypePopBucket)); return (reinterpret_cast<BTriggerVarPopBucket*>(this)); }
   BTriggerVarListPosition* asListPosition(void){ BASSERT(isType(cVarTypeListPosition)); return (reinterpret_cast<BTriggerVarListPosition*>(this)); }
   BTriggerVarRelationType* asRelationType(void){ BASSERT(isType(cVarTypeRelationType)); return (reinterpret_cast<BTriggerVarRelationType*>(this)); }
   BTriggerVarExposedAction* asExposedAction(void){ BASSERT(isType(cVarTypeExposedAction)); return (reinterpret_cast<BTriggerVarExposedAction*>(this)); }
   BTriggerVarSquadMode* asSquadMode(void){ BASSERT(isType(cVarTypeSquadMode)); return (reinterpret_cast<BTriggerVarSquadMode*>(this)); }
   BTriggerVarExposedScript* asExposedScript(){ BASSERT(isType(cVarTypeExposedScript)); return (reinterpret_cast<BTriggerVarExposedScript*>(this)); }
   BTriggerVarKBBase* asKBBase() { BASSERT(isType(cVarTypeKBBase)); return (reinterpret_cast<BTriggerVarKBBase*>(this)); }
   BTriggerVarKBBaseList* asKBBaseList() { BASSERT(isType(cVarTypeKBBaseList)); return (reinterpret_cast<BTriggerVarKBBaseList*>(this)); }
   BTriggerVarDataScalar* asDataScalar() { BASSERT(isType(cVarTypeDataScalar)); return (reinterpret_cast<BTriggerVarDataScalar*>(this)); }
   BTriggerVarKBBaseQuery* asKBBaseQuery() { BASSERT(isType(cVarTypeKBBaseQuery)); return (reinterpret_cast<BTriggerVarKBBaseQuery*>(this)); }
   BTriggerVarDesignLine* asDesignLine() { BASSERT(isType(cVarTypeDesignLine)); return (reinterpret_cast<BTriggerVarDesignLine*>(this)); }
   BTriggerVarLocStringID* asLocStringID() { BASSERT(isType(cVarTypeLocStringID)); return (reinterpret_cast<BTriggerVarLocStringID*>(this)); }
   BTriggerVarLeader* asLeader() { BASSERT(isType(cVarTypeLeader)); return (reinterpret_cast<BTriggerVarLeader*>(this)); }
   BTriggerVarCinematic* asCinematic() { BASSERT(isType(cVarTypeCinematic)); return (reinterpret_cast<BTriggerVarCinematic*>(this)); }
   BTriggerVarTalkingHead* asTalkingHead() { BASSERT(isType(cVarTypeTalkingHead)); return (reinterpret_cast<BTriggerVarTalkingHead*>(this)); }
   BTriggerVarFlareType* asFlareType() { BASSERT(isType(cVarTypeFlareType)); return (reinterpret_cast<BTriggerVarFlareType*>(this)); }
   BTriggerVarCinematicTag* asCinematicTag(){ BASSERT(isType(cVarTypeCinematicTag)); return (reinterpret_cast<BTriggerVarCinematicTag*>(this)); }
   BTriggerVarIconType* asIconType() { BASSERT(isType(cVarTypeIconType)); return (reinterpret_cast<BTriggerVarIconType*>(this)); }
   BTriggerVarDifficulty* asDifficulty() { BASSERT(isType(cVarTypeDifficulty)); return (reinterpret_cast<BTriggerVarDifficulty*>(this)); }
   BTriggerVarInteger* asInteger() { BASSERT(isType(cVarTypeInteger)); return (reinterpret_cast<BTriggerVarInteger*>(this)); }
   BTriggerVarHUDItem* asHUDItem() { BASSERT(isType(cVarTypeHUDItem)); return(reinterpret_cast<BTriggerVarHUDItem*>(this)); }
   BTriggerVarFlashableUIItem* asUIItem() { BASSERT(isType(cVarTypeFlashableUIItem)); return(reinterpret_cast<BTriggerVarFlashableUIItem*>(this)); }
   BTriggerVarControlType* asControlType() { BASSERT(isType(cVarTypeControlType)); return(reinterpret_cast<BTriggerVarControlType*>(this)); }
   BTriggerVarUIButton* asUIButton() { BASSERT(isType(cVarTypeUIButton)); return(reinterpret_cast<BTriggerVarUIButton*>(this)); }
   BTriggerVarMissionType* asMissionType() { BASSERT(isType(cVarTypeMissionType)); return (reinterpret_cast<BTriggerVarMissionType*>(this)); }
   BTriggerVarMissionState* asMissionState() { BASSERT(isType(cVarTypeMissionState)); return (reinterpret_cast<BTriggerVarMissionState*>(this)); }
   BTriggerVarMissionTargetType* asMissionTargetType() { BASSERT(isType(cVarTypeMissionTargetType)); return (reinterpret_cast<BTriggerVarMissionTargetType*>(this)); }
   BTriggerVarIntegerList* asIntegerList() { BASSERT(isType(cVarTypeIntegerList)); return (reinterpret_cast<BTriggerVarIntegerList*>(this)); }
   BTriggerVarBidType* asBidType() { BASSERT(isType(cVarTypeBidType)); return (reinterpret_cast<BTriggerVarBidType*>(this)); }
   BTriggerVarBidState* asBidState() { BASSERT(isType(cVarTypeBidState)); return (reinterpret_cast<BTriggerVarBidState*>(this)); }
   BTriggerVarBuildingCommandState * asBuildingCommandState() { BASSERT(isType(cVarTypeBuildingCommandState)); return (reinterpret_cast<BTriggerVarBuildingCommandState*>(this)); }
   BTriggerVarVector* asVector() { BASSERT(isType(cVarTypeVector)); return (reinterpret_cast<BTriggerVarVector*>(this)); }
   BTriggerVarVectorList* asVectorList() { BASSERT(isType(cVarTypeVectorList)); return (reinterpret_cast<BTriggerVarVectorList*>(this)); }
   BTriggerVarPlacementRule* asPlacementRule() { BASSERT(isType(cVarTypePlacementRule)); return (reinterpret_cast<BTriggerVarPlacementRule*>(this)); }
   BTriggerVarKBSquad* asKBSquad() { BASSERT(isType(cVarTypeKBSquad)); return (reinterpret_cast<BTriggerVarKBSquad*>(this)); }
   BTriggerVarKBSquadList* asKBSquadList() { BASSERT(isType(cVarTypeKBSquadList)); return (reinterpret_cast<BTriggerVarKBSquadList*>(this)); }
   BTriggerVarKBSquadQuery* asKBSquadQuery() { BASSERT(isType(cVarTypeKBSquadQuery)); return (reinterpret_cast<BTriggerVarKBSquadQuery*>(this)); }
   BTriggerVarAISquadAnalysis* asAISquadAnalysis() { BASSERT(isType(cVarTypeAISquadAnalysis)); return (reinterpret_cast<BTriggerVarAISquadAnalysis*>(this)); }
   BTriggerVarAISquadAnalysisComponent* asAISquadAnalysisComponent() { BASSERT(isType(cVarTypeAISquadAnalysisComponent)); return (reinterpret_cast<BTriggerVarAISquadAnalysisComponent*>(this)); }
   BTriggerVarKBSquadFilterSet* asKBSquadFilterSet() { BASSERT(isType(cVarTypeKBSquadFilterSet)); return (reinterpret_cast<BTriggerVarKBSquadFilterSet*>(this)); }
   BTriggerVarChatSpeaker* asChatSpeaker(void) { BASSERT(isType(cVarTypeChatSpeaker)); return(reinterpret_cast<BTriggerVarChatSpeaker*>(this)); }
   BTriggerVarRumbleType* asRumbleType() { BASSERT(isType(cVarTypeRumbleType)); return(reinterpret_cast<BTriggerVarRumbleType*>(this)); }
   BTriggerVarRumbleMotor* asRumbleMotor() { BASSERT(isType(cVarTypeRumbleMotor)); return(reinterpret_cast<BTriggerVarRumbleMotor*>(this)); }
   BTriggerVarTechDataCommandType* asTechDataCommandType() { BASSERT(isType(cVarTypeTechDataCommandType)); return (reinterpret_cast<BTriggerVarTechDataCommandType*>(this)); }
   BTriggerVarSquadDataType* asSquadDataType() { BASSERT(isType(cVarTypeSquadDataType)); return (reinterpret_cast<BTriggerVarSquadDataType*>(this)); }
   BTriggerVarEventType* asEventType() { BASSERT(isType(cVarTypeEventType)); return (reinterpret_cast<BTriggerVarEventType*>(this)); }
   BTriggerVarTimeList* asTimeList() { BASSERT(isType(cVarTypeTimeList)); return (reinterpret_cast<BTriggerVarTimeList*>(this)); }
   BTriggerVarDesignLineList* asDesignLineList() { BASSERT(isType(cVarTypeDesignLineList)); return (reinterpret_cast<BTriggerVarDesignLineList*>(this)); }
   BTriggerVarGameStatePredicate* asGameStatePredicate() { BASSERT(isType(cVarTypeGameStatePredicate)); return (reinterpret_cast<BTriggerVarGameStatePredicate*>(this)); }
   BTriggerVarFloatList* asFloatList() { BASSERT(isType(cVarTypeFloatList)); return (reinterpret_cast<BTriggerVarFloatList*>(this)); }
   BTriggerVarUILocationMinigame* asUILocationMinigame(void) { BASSERT(isType(cVarTypeUILocationMinigame)); return(reinterpret_cast<BTriggerVarUILocationMinigame*>(this)); }
   BTriggerVarSquadFlag* asSquadFlag(void) { BASSERT(isType(cVarTypeSquadFlag)); return(reinterpret_cast<BTriggerVarSquadFlag*>(this)); }
   BTriggerVarConcept* asConcept(void) { BASSERT(isType(cVarTypeConcept)); return(reinterpret_cast<BTriggerVarConcept*>(this)); }
   BTriggerVarConceptList* asConceptList(void) { BASSERT(isType(cVarTypeConceptList)); return(reinterpret_cast<BTriggerVarConceptList*>(this)); }
   BTriggerVarUserClassType* asUserClassType(void) { BASSERT(isType(cVarTypeUserClassType)); return(reinterpret_cast<BTriggerVarUserClassType*>(this)); }";
    
        foreach(string line in file.Split("\n"))
        {
            Match m = Regex.Match(line, @"as([\w]*)\(");
            if (m.Success)
            {
                ret.Add(m.Groups[1].Value);
                Console.WriteLine(m.Groups[1].Value);
            }
        }


        return ret;
    }
}
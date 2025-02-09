using System.Xml.Linq;

string type = @"Tech,TechStatus,Operator,ProtoObject,ObjectType,ProtoSquad,Sound,Entity,EntityList,Trigger,Time,Player,UILocation,UIEntity,Cost,AnimType,ActionStatus,Power,Bool,Float,Iterator,Team,PlayerList,TeamList,PlayerState,Objective,Unit,UnitList,Squad,SquadList,UIUnit,UISquad,UISquadList,String,MessageIndex,MessageJustify,MessagePoint,Color,ProtoObjectList,ObjectTypeList,ProtoSquadList,TechList,MathOperator,ObjectDataType,ObjectDataRelative,Civ,ProtoObjectCollection,Object,ObjectList,Group,RefCountType,UnitFlag,LOSType,EntityFilterSet,PopBucket,ListPosition,Diplomacy,ExposedAction,SquadMode,ExposedScript,KBBase,KBBaseList,DataScalar,KBBaseQuery,DesignLine,LocStringID,Leader,Cinematic,FlareType,CinematicTag,IconType,Difficulty,Integer,HUDItem,ControlType,UIButton,MissionType,MissionState,MissionTargetType,IntegerList,BidType,BidState,BuildingCommandState,Vector,VectorList,PlacementRule,KBSquad,KBSquadList,KBSquadQuery,AISquadAnalysis,AISquadAnalysisComponent,KBSquadFilterSet,ChatSpeaker,RumbleType,RumbleMotor,CommandType,SquadDataType,EventType,TimeList,DesignLineList,GameStatePredicate,FloatList,UILocationMinigame,SquadFlag,FlashableUIItem,TalkingHead,Concept,ConceptList,UserClassType";
string[] types = type.Split(",");

XDocument doc = new XDocument();
doc.AddFirst(new XElement("Variables"));
foreach(string t in types)
{
    var elem = new XElement("VarType");
    elem.SetAttributeValue("Name", t);

    if (t != "ListLocation" && t.Contains("List"))
    {
        elem.SetAttributeValue("ListType", t.Replace("List", ""));
    }

    var alisases = new XElement("Aliases");
    if (t == "Vector")
    {
        alisases.Add(new XElement("Alias", "Location"));
        alisases.Add(new XElement("Alias", "Direction"));
    }
    if (t == "VectorList")
    {
        alisases.Add(new XElement("Alias", "LocationList"));
        alisases.Add(new XElement("Alias", "DirectionList"));
    }
    if (t == "Float")
    {
        alisases.Add(new XElement("Alias", "Hitpoints"));
        alisases.Add(new XElement("Alias", "Percent"));
        alisases.Add(new XElement("Alias", "Distance"));
    }
    if (t == "Integer")
    {
        alisases.Add(new XElement("Alias", "Count"));
    }
    if (t == "Diplomacy")
    {
        alisases.Add(new XElement("Alias", "RelationType"));
    }
    if (t == "CommandType")
    {
        alisases.Add(new XElement("Alias", "TechDataCommandType"));
    }
    elem.Add(alisases);


    var vals = new XElement("Values");


    doc.Root.Add(elem);
}

doc.Save("vars.tsdef");

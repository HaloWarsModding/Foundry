using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using YAXLib.Attributes;

namespace Chef.HW1.Map
{
    public class Scenario
    {
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public string Terrain { get; set; }
        [YAXAttributeFor("Terrain")]
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public bool LoadVisRep { get; set; }

        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public string Sky { get; set; }
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public string TerrainEnv { get; set; }
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public string Lightset { get; set; }
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public string Pathing { get; set; }

        public class CinematicClass
        {
            [YAXAttributeForClass()]
            public int ID { get; set; }

            [YAXValueForClass()]
            public string File { get; set; }
        }
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Cinematic")]
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public List<CinematicClass> Cinematics { get; set; }

        public class TalkingHeadsClass
        {
            [YAXAttributeForClass()]
            public int ID { get; set; }

            [YAXValueForClass()]
            public string File { get; set; }
        }
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "TalkingHead")]
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public List<TalkingHeadsClass> TalkingHeads { get; set; }

        class DiplomacyClass
        {
            [YAXSerializeAs("Team")]
            [YAXAttributeForClass()]
            public int TeamNumber { get; set; }
            public class TeamClass
            {
                public int ID { get; set; }

                public enum RelationEnum
                {
                    Ally,
                    Enemy,
                    Neutral,
                }
                [YAXValueForClass()]
                public RelationEnum Relation { get; set; }
            }
        }
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Diplomacy")]
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        List<DiplomacyClass> Diplomacies { get; set; }

        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public string PlayerPlacement { get; set; }
        public enum PlayerPlacementTypeEnum
        {
            Random,
            Fixed,
            Grouped,
        }
        [YAXSerializeAs("Type")]
        [YAXAttributeFor("PlayerPlacement")]
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public PlayerPlacementTypeEnum PlayerPlacementType { get; set; }
        [YAXSerializeAs("Spacing")]
        [YAXAttributeFor("PlayerPlacement")]
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public int PlayerPlacementSpacing { get; set; }

        public class PositionClass
        {
            [YAXAttributeForClass()]
            public int Player { get; set; }
            [YAXAttributeForClass()]
            public int Number { get; set; }
            [YAXAttributeForClass()]
            public string Position { get; set; }
            [YAXAttributeForClass()]
            public string Forward { get; set; }
            [YAXAttributeForClass()]
            public bool DefaultCamera { get; set; }
            [YAXAttributeForClass()]
            public float CameraYaw { get; set; }
            [YAXAttributeForClass()]
            public float CameraPitch { get; set; }
            [YAXAttributeForClass()]
            public float CameraZoom { get; set; }
            [YAXAttributeForClass()]
            public int UnitStartObject1 { get; set; }
            [YAXAttributeForClass()]
            public int UnitStartObject2 { get; set; }
            [YAXAttributeForClass()]
            public int UnitStartObject3 { get; set; }
            [YAXAttributeForClass()]
            public int UnitStartObject4 { get; set; }
            [YAXAttributeForClass()]
            public int RallyStartObject { get; set; }
        }
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Position")]
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public List<PositionClass> Positions { get; set; }

        public class PlayerClass
        {
            [YAXAttributeForClass()]
            public string Name { get; set; }
            [YAXAttributeForClass()]
            [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
            public string LocalisedDisplayName { get; set; }
            [YAXAttributeForClass()]
            public bool UseStartingUnits { get; set; }
            [YAXAttributeForClass()]
            public bool Controllable { get; set; }
            [YAXAttributeForClass()]
            public bool UsePlayerSettings { get; set; }
            [YAXAttributeForClass()]
            public string Civ { get; set; }
            [YAXAttributeForClass()]
            [YAXSerializeAs("Color")]
            public int ColorID { get; set; }
            [YAXAttributeForClass()]
            public int Team { get; set; }
            [YAXAttributeForClass()]
            [YAXSerializeAs("DefaultResources")]
            public bool UsesDefaultResources { get; set; }
            [YAXAttributeForClass()]
            public int Supplies { get; set; }
            [YAXAttributeForClass()]
            public int Power { get; set; }
            [YAXAttributeForClass()]
            public int Favor { get; set; }
            [YAXAttributeForClass()]
            public int Relics { get; set; }
            [YAXAttributeForClass()]
            public int Honor { get; set; }

            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Pop")]
            public List<string> Pops { get; set; }
            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Object")]
            public List<string> ForbidObjects { get; set; }
            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Squad")]
            public List<string> ForbidSquads { get; set; }
            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Tech")]
            public List<string> ForbidTechs { get; set; }
        }
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Player")]
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public List<PlayerClass> Players { get; set; }

        public class ObjectiveClass
        {
            [YAXSerializeAs("id")]
            [YAXAttributeForClass()]
            public int ID { get; set; }

            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Flag")]
            public List<string> Flags { get; set; }

            public string Description { get; set; }
            public string TrackerText { get; set; }
            public int TrackerDuration { get; set; }
            public int MinTrackerIncrement { get; set; }
            public string Hint { get; set; }
            public int Score { get; set; }
            public int FinalCount { get; set; }

            [YAXValueForClass()]
            public string Name { get; set; }
        }
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Objective")]
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public List<ObjectiveClass> Objectives { get; set; }

        public class MinimapClass
        {
            public string MinimapTexture { get; set; }
        }
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public MinimapClass Minimap { get; set; }

        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public int BuildingTextureIndexUNSC { get; set; }
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public int BuildingTextureIndexCOVN { get; set; }

        public class DesignObjectsClass
        {
            public class DataClass
            {
                public string Type { get; set; }
                public string Attract { get; set; }
                public string Repel { get; set; }
                public string ChokePoint { get; set; }
                public string Value { get; set; }
                public string Value1 { get; set; }
                public string Value2 { get; set; }
                public string Value3 { get; set; }
                public string Value4 { get; set; }
                public string Value5 { get; set; }
                public string Value6 { get; set; }
                public string Value7 { get; set; }
                public string Value8 { get; set; }
                public string Value9 { get; set; }
                public string Value10 { get; set; }
                public string Value11 { get; set; }
                public string Value12 { get; set; }
            }

            public class SphereClass
            {
                [YAXAttributeForClass()]
                public int ID { get; set; }
                [YAXAttributeForClass()]
                public string Name { get; set; }
                [YAXAttributeForClass()]
                public string Position { get; set; }
                [YAXAttributeForClass()]
                public float Radius { get; set; }

                public DataClass Data { get; set; }
            }
            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Sphere")]
            public List<SphereClass> Spheres { get; set; }

            public class LineClass
            {
                [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Serially, SeparateBy = "|")]
                public List<Vector3> Points { get; set; }

                public DataClass Data { get; set; }
            }
            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Lines")] //yes "Lines", plural.
            public List<LineClass> Lines { get; set; }
        }
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public DesignObjectsClass DesignObjects { get; set; }

        public class ObjectClass
        {
            [YAXAttributeForClass()]
            public bool IsSquad { get; set; }
            [YAXAttributeForClass()]
            public int Player { get; set; }
            [YAXAttributeForClass()]
            public int ID { get; set; }
            [YAXAttributeForClass()]
            public float TintValue { get; set; }
            [YAXAttributeForClass()]
            public string EditorName { get; set; }
            [YAXAttributeForClass()]
            public string Position { get; set; }
            [YAXAttributeForClass()]
            public string Forward { get; set; }
            [YAXAttributeForClass()]
            public string Right { get; set; }
            [YAXAttributeForClass()]
            public int Group { get; set; }
            [YAXAttributeForClass()]
            public int VisualVariationIndex { get; set; }

            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Flag")]
            public List<string> Flags { get; set; }

            [YAXValueForClass()]
            public string Unit { get; set; }
        }
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Object")]
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public List<ObjectClass> Objects { get; set; }

        public class LightsClass
        {

        }
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Light")]
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public List<LightsClass> Lights { get; set; }

        //Triggersystem

        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public int NextID { get; set; }

        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public int SimBoundsMinX { get; set; }
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public int SimBoundsMinZ { get; set; }
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public int SimBoundsMaxX { get; set; }
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public int SimBoundsMaxZ { get; set; }

        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "SoundBank")]
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public List<string> SoundBanks { get; set; }

        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public string EnvironmentReverbPreset { get; set; }
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public string ScenarioWorld { get; set; }
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public string AllowVeterancy { get; set; }

        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "???")]
        [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
        public List<string> GlobalExcludeObjects { get; set; }
    }
}

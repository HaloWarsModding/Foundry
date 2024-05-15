using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace Foundry.Asset
{
    [YAXSerializeAs("Visual")]
    public class VisXmlData
    {
        [YAXSerializeAs("defaultmodel")]
        [YAXAttributeForClass]
        public string DefaultModel { get; set; }


        [YAXSerializeAs("model")]
        public class Model
        {
            [YAXSerializeAs("name")]
            [YAXAttributeForClass]
            public string Name { get; set; }

            public class ComponentClass
            {
                public class LogicClass //TODO 
                {
                    public enum TypeEnum
                    {
                        Variation,
                        BuildingCompletion,
                        Tech,
                        SquadMode,
                        ImpactSize,
                        Destruction
                    }
                }
                [YAXSerializeAs("logic")]
                [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                public LogicClass Logic { get; set; }

                public class AssetClass
                {
                    public enum TypeEnum
                    {
                        Model,
                        Anim,
                        Particle,
                        Light
                    }
                    [YAXSerializeAs("type")]
                    [YAXAttributeForClass]
                    public TypeEnum Type { get; set; }

                    [YAXSerializeAs("file")]
                    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                    public string File { get; set; }

                    [YAXSerializeAs("damagefile")]
                    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                    public string DamageFile { get; set; }

                    public class PointClass
                    {
                        public enum TypeEnum
                        {
                            Impact,
                            Launch,
                            HitpointBar,
                            Reflect,
                            Cover,
                            Carry,
                            Pickup,
                            Board,
                            Bobble
                        }
                        [YAXSerializeAs("pointType")]
                        public TypeEnum Type { get; set; }

                        [YAXSerializeAs("bone")]
                        public string Bone { get; set; }

                        [YAXSerializeAs("pointData")]
                        public string PointData { get; set; }
                    }
                    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "point")]
                    public List<PointClass> Points { get; set; }
                }
                [YAXSerializeAs("asset")]
                [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                public AssetClass Asset { get; set; }

                public class AttachmentClass
                {
                    public enum TypeEnum
                    {
                        ParticleFile,
                        ModelFile,
                        ModelRef,
                        LightFile,
                        TerrainEffect
                    }
                    [YAXSerializeAs("type")]
                    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                    public TypeEnum Type { get; set; }

                    [YAXSerializeAs("name")]
                    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                    public string Name { get; set; }

                    [YAXSerializeAs("tobone")]
                    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                    public string ToBone { get; set; }

                    [YAXSerializeAs("frombone")]
                    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                    public string FromBone { get; set; }

                    [YAXSerializeAs("syncanims")]
                    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                    public bool SyncAnims { get; set; }

                    [YAXSerializeAs("disregardorient")]
                    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                    public bool DisregardOrient { get; set; }
                }
                [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "attach")]
                public List<AttachmentClass> Attachments { get; set; }
            }
            [YAXSerializeAs("component")]
            public ComponentClass Component { get; set; }

            public class AnimClass
            {
                [YAXSerializeAs("type")]
                public string Type { get; set; }

                [YAXSerializeAs("exitAction")]
                public string ExitAction { get; set; }

                [YAXSerializeAs("tweenTime")]
                public float TweenTime { get; set; }

                [YAXSerializeAs("tweenToAnimation")]
                public string TweenToAnimation { get; set; }

                public class AssetClass
                {
                    public enum TypeEnum
                    {
                        Anim
                    }
                    [YAXSerializeAs("type")]
                    public TypeEnum Type { get; set; }

                    [YAXSerializeAs("weight")]
                    public float Weight { get; set; }

                    public class TagClass
                    {
                        public enum TypeEnum
                        {
                            Attack,
                            Sound,
                            Particle,
                            TerrainEffect,
                            CameraShake,
                            Loop,
                            Light,
                            GroundIK,
                            AttachTarget,
                            SweetSpot,
                            TerrainAlpha,
                            Rumble,
                            BuildingDecal,
                            UVOffset,
                            KillAndThrow,
                            PhysicsImpulse
                        }
                        [YAXAttributeForClass]
                        [YAXSerializeAs("weight")]
                        public TypeEnum Type { get; set; }

                        [YAXAttributeForClass]
                        [YAXSerializeAs("start")]
                        public float Start { get; set; }

                        [YAXAttributeForClass]
                        [YAXSerializeAs("position")]
                        public float Position { get; set; }

                        [YAXAttributeForClass]
                        [YAXSerializeAs("end")]
                        public float End { get; set; }

                        [YAXAttributeForClass]
                        [YAXSerializeAs("tobone")]
                        public string ToBone { get; set; }

                        [YAXAttributeForClass]
                        [YAXSerializeAs("checkvisible")]
                        public bool CheckVisible { get; set; }

                        [YAXAttributeForClass]
                        [YAXSerializeAs("lockToGround")]
                        public bool LockToGround { get; set; }

                        [YAXAttributeForClass]
                        [YAXSerializeAs("attach")]
                        public bool Attach { get; set; }

                        [YAXAttributeForClass]
                        [YAXSerializeAs("disregardorient")]
                        public bool DisregardOrient { get; set; }

                        [YAXAttributeForClass]
                        [YAXSerializeAs("force")]
                        public float Force { get; set; }

                        [YAXAttributeForClass]
                        [YAXSerializeAs("force2")]
                        public float Force2 { get; set; }

                        [YAXAttributeForClass]
                        [YAXSerializeAs("lifespan")]
                        public float Lifespan { get; set; }

                        [YAXAttributeForClass]
                        [YAXSerializeAs("loop")]
                        public bool Loop { get; set; }

                        [YAXAttributeForClass]
                        [YAXSerializeAs("checkSelected")]
                        public bool CheckSelected { get; set; }

                        [YAXAttributeForClass]
                        [YAXSerializeAs("weight")]
                        public bool CheckFOW { get; set; }
                    }
                    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "tag")]
                    public List<TagClass> Tags { get; set; }
                }
            }
            [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "asset")]
            public List<AnimClass> Animations { get; set; }

        }
        [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "model")]
        public List<Model> Models { get; set; }
    }
}

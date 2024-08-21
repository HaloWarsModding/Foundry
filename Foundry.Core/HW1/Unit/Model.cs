using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Chef.HW1.Unit
{
    public enum ModelMaterialMap
    {
        Diffuse,
        Normal,
        Gloss,
        Opacity,
        XForm,
        Emissive,
        AmbOcc,
        Env,
        EnvMask,
        EmXForm,
        Distortion,
        Highlight,
        Modulate,
    }
    public class ModelMaterial
    {
        public void SetMap(ModelMaterialMap map, string path, Vector3 uvwVel)
        {
            Maps[(int)map] = path;
            UVWVels[(int)map] = uvwVel;
        }
        public string GetMap(ModelMaterialMap map)
        {
            return Maps[(int)map];
        }

        private string[] Maps = new string[Enum.GetNames<ModelMaterialMap>().Length];
        private Vector3[] UVWVels = new Vector3[Enum.GetNames<ModelMaterialMap>().Length];

        public bool ColorGloss { get; set; }
        public bool OpacityValid { get; set; }
        public bool TwoSided { get; set; }
        public bool DisableShadows { get; set; }
        public bool GlobalEnv { get; set; }
        public bool TerrainConform { get; set; }
        public bool LocalReflection { get; set; }
        public bool DisableShadowReception { get; set; }
    }

    public enum ModelVertexElemUsage
    {
        POSITION = 'P',
        BASIS = 'B',
        TANGENT = 'A',
        NORMAL = 'N',
        UV = 'T',
        SKIN = 'S',
        COLOR = 'D',
        INDEX = 'I',
        BASIS_SCALE = 'X'
    }
    public enum ModelVertexElemType
    {
        FLOAT1,
        FLOAT2,
        FLOAT3,
        FLOAT4,
        COLOR,
        BYTE4,
        SHORT2,
        SHORT4,
        UBYTE4N,
        SHORT2N,
        SHORT4N,
        USHORT2N,
        USHORT4N,
        UDEC3,
        DEC3N,
        HALFFLOAT2,
        HALFFLOAT4,
    }
    public class ModelVertexSpec
    {
        public bool HasPosition { get; set; }
        public int NumBasis { get; set; }
        public bool HasNormals { get; set; }
        public int NumUVs { get; set; }
        public bool HasIndices { get; set; }
        public bool HasSkin { get; set; }
        public bool HasColor { get; set; }
    }
    public struct ModelVertex
    {
        public static int cVertexSize { get; } = 48;

        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        //public Vector3 Tangent { get; set; }
        public Vector2 Uv0 { get; set; }
        public Vector2 Uv1 { get; set; }
        public Vector2 Uv2 { get; set; }
        //public byte Bone0 { get; set; }
        //public byte Bone1 { get; set; }
        //public byte Bone2 { get; set; }
        //public byte Bone3 { get; set; }
        //public byte Weight0 { get; set; }
        //public byte Weight1 { get; set; }
        //public byte Weight2 { get; set; }
        //public byte Weight3 { get; set; }
    }
    public class ModelSection
    {
        public ModelVertexSpec VertexSpec { get; set; }
        public ModelVertex[] Vertices { get; set; }
        public ushort[] Indices { get; set; }
        public ModelMaterial Material { get; set; }
        //public int RigidBoneIndex { get; set; } //will disable skinning in the export.
    }

    public class Model
    {
        public ModelSection[] Sections { get; set; }
        public Vector3 BoundsMin { get; set; }
        public Vector3 BoundsMax { get; set; }
    }
}

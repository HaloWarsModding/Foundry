//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Numerics;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Linq;
//using Foundry.util;
//using Foundry.Util;
//using SharpDX.Direct2D1.Effects;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

//namespace Foundry.HW1.Ecf
//{
//    public class UgxCachedVertextFormat
//    {
//        public PackedElement PackOrder { get; set; } = new PackedElement();
//        public PackedElement DeclOrder { get; set; } = new PackedElement();
//        public int[] ElementTypes { get; set; }

//        public bool Read(BinaryReader r)
//        {
//            PackOrder = new PackedElement();
//            PackOrder.Read(r);
//            DeclOrder = new PackedElement();
//            DeclOrder.Read(r);
//            ElementTypes = new int[17];
//            for (int i = 0; i < ElementTypes.Length; i++)
//                ElementTypes[i] = r.ReadInt32();
//            r.ReadInt32();
//            return true;
//        }
//        public bool Write(BinaryWriter w)
//        {
//                PackOrder.Write(w);
//                DeclOrder.Write(w);
//                for (int i = 0; i < 17; i++)
//                    w.Write(ElementTypes[i]);
//                w.Write(0);
//            return true;
//        }
//        public bool Pack(BinaryWriter w)
//        {
//            PackOrder.Pack(w);
//            DeclOrder.Pack(w);
//            return true;
//        }
//    }
//    public class UgxCachedSection
//    {
//        public int MaterialIndex { get; set; }
//        public int AccessoryIndex { get; set; }
//        public int MaxBones { get; set; }
//        public int RigidBoneIndex { get; set; }

//        public int IndexBufferOffset { get; set; }
//        public int TriangleCount { get; set; }

//        public int VertexBufferOffset { get; set; }
//        public int VertexBufferSize { get; set; }
//        public int VertexSize { get; set; }
//        public int VertexCount { get; set; }

//        public PackedArray<StreamableInt> GlobalToLocalBoneRemap { get; set; } = new PackedArray<StreamableInt>(); //unused.
//        public UgxCachedVertextFormat VertexFormat { get; set; } = new UgxCachedVertextFormat();

//        public bool RigidOnly { get; set; }


//        public bool Read(BinaryReader r)
//        {
//            MaterialIndex = r.ReadInt32();
//            AccessoryIndex = r.ReadInt32();
//            MaxBones = r.ReadInt32();
//            RigidBoneIndex = r.ReadInt32();
//            IndexBufferOffset = r.ReadInt32();
//            TriangleCount = r.ReadInt32();
//            VertexBufferOffset = r.ReadInt32();
//            VertexBufferSize = r.ReadInt32();
//            VertexSize = r.ReadInt32();
//            VertexCount = r.ReadInt32();
//            GlobalToLocalBoneRemap = new PackedArray<StreamableInt>();
//            GlobalToLocalBoneRemap.Read(stream);
//            VertexFormat = new UgxCachedVertextFormat();
//            VertexFormat.Read(stream);
//            RigidOnly = r.ReadBoolean();
//            /*PAD0*/
//            r.ReadBytes(7);
//            return true;
//        }
//        public bool Write(BinaryWriter w)
//        {
//            w.Write(MaterialIndex);
//            w.Write(AccessoryIndex);
//            w.Write(MaxBones);
//            w.Write(RigidBoneIndex);
//            w.Write(IndexBufferOffset);
//            w.Write(TriangleCount);
//            w.Write(VertexBufferOffset);
//            w.Write(VertexBufferSize);
//            w.Write(VertexSize);
//            w.Write(VertexCount);
//            GlobalToLocalBoneRemap.Write(stream);
//            VertexFormat.Write(stream);
//            w.Write(RigidOnly);
//            w.Write(new byte[7] /*PAD0*/);
//            return true;
//        }
//        public bool Pack(BinaryWriter w)
//        {
//            VertexFormat.Pack(w);
//            return true;
//        }
//    }
//    public class UgxCachedBone
//    {
//        public PackedElement NameHeader { get; set; }
//        public StreamableMatrix4 Transform { get; set; }
//        public int ParentIndex { get; set; }
//        public int PAD0 { get; set; }

//        public bool Read(BinaryReader r)
//        {
//            NameHeader.Read(r);
//            Transform.Read(r);
//            ParentIndex = r.ReadInt32();
//            /*PAD0*/ = r.ReadInt32();
//            return true;
//        }
//        public bool Write(Stream stream)
//        {
//            NameHeader.Write(w);
//            Transform.Write(stream, Endianness.Little);
//            w.Write(ParentIndex);
//            w.Write(0 /*PAD0*/);
//            return true;
//        }
//        public bool Pack(BinaryWriter r)
//        {
//            Name.Pack(r);
//            return true;
//        }
//    }
//    public class UgxCachedAccessory
//    {
//        public int FirstBone { get; set; }
//        public int NumberOfBones { get; set; }
//        private PackedArray SectionIndicesHeader { get; set; }

//        public void Read(Stream stream)
//        {
//            using (BinaryReader r = new BinaryReader(stream, Encoding.ASCII, true))
//            {
//                FirstBone = r.ReadInt32();
//                NumberOfBones = r.ReadInt32();
//                SectionIndicesHeader.Read(stream);
//            }
//        }
//        public void Write(Stream stream)
//        {
//            using (BinaryWriter w = new BinaryWriter(stream, Encoding.ASCII, true))
//            {
//                w.Write(FirstBone);
//                w.Write(NumberOfBones);
//                SectionIndicesHeader.Write(stream);
//            }
//        }
//    }
//    public class UgxCachedData
//    {
//        public uint Signature { get; set; }
//        public int RigidBoneIndex { get; set; }
//        public StreamableVector3 BoundingSphereCenter { get; set; } = new StreamableVector3();
//        public float BoundingSphereRadius { get; set; }
//        public StreamableVector3 BoundsMin { get; set; } = new StreamableVector3();
//        public StreamableVector3 BoundsMax { get; set; } = new StreamableVector3();
//        public short MaxInstances { get; set; }
//        public short InstanceIndexMultiplier { get; set; }
//        public short LargeGeomBoneIndex { get; set; }
//        public bool AllSectionRigid { get; set; }
//        public bool GlobalBones { get; set; }
//        public bool AllSectionsSkinned { get; set; }
//        public bool RigidOnly { get; set; }
//    }


//    public enum UgxMaterialMap
//    {
//        Diffuse,
//        Normal,
//        Gloss,
//        Opacity,
//        XForm,
//        Emissive,
//        AmbOcc,
//        Env,
//        EnvMask,
//        EmXForm,
//        Distortion,
//        Highlight,
//        Modulate,
//    }
//    public class UgxMaterial
//    {
//        void SetMap(UgxMaterialMap map, string path, Vector3 uvwVel)
//        {
//            Maps[(int)map] = path;
//            UVWVels[(int)map] = uvwVel;
//        }

//        private string[] Maps = new string[Enum.GetNames<UgxMaterialMap>().Length];
//        private Vector3[] UVWVels = new Vector3[Enum.GetNames<UgxMaterialMap>().Length];

//        public bool ColorGloss { get; set; }
//        public bool OpacityValid { get; set; }
//        public bool TwoSided { get; set; }
//        public bool DisableShadows { get; set; }
//        public bool GlobalEnv { get; set; }
//        public bool TerrainConform { get; set; }
//        public bool LocalReflection { get; set; }
//        public bool DisableShadowReception { get; set; }
//    }
//    public struct UgxVertex
//    {
//        public Vector3 Position { get; set; }
//        public Vector3 Normal { get; set; }
//        public Vector3 Tangent { get; set; }
//        public Vector2 Uv0 { get; set; }
//        public Vector2 Uv1 { get; set; }
//        public Vector2 Uv2 { get; set; }
//        public Vector2 Uv3 { get; set; }
//        public byte Bone0 { get; set; }
//        public byte Bone1 { get; set; }
//        public byte Bone2 { get; set; }
//        public byte Bone3 { get; set; }
//        public float Weight0 { get; set; }
//        public float Weight1 { get; set; }
//        public float Weight2 { get; set; }
//        public float Weight3 { get; set; }
//    }
//    public class UgxSection
//    {
//        public UgxVertex[] Vertices { get; set; }
//        public ushort[] Indices { get; set; }
//        public UgxMaterial Material { get; set; }
//        //public int RigidBoneIndex { get; set; } //will disable skinning in the export.
//    }
//    public static class Ugx
//    {
//        public static List<UgxSection> ReadEcf(Stream stream)
//        {
//            EcfFile ecf = new EcfFile();
//            EcfChunk cgranny = null;
//            EcfChunk ccached = null;
//            EcfChunk cvb = null;
//            EcfChunk cib = null;
//            EcfChunk cmaterials = null;

//            foreach (EcfChunk chunk in ecf.Chunks)
//            {
//                if (chunk.ID == 0x703) cgranny = chunk;
//                if (chunk.ID == 0x700) ccached = chunk;
//                if (chunk.ID == 0x702) cib = chunk;
//                if (chunk.ID == 0x701) cvb = chunk;
//                if (chunk.ID == 0x704) cmaterials = chunk;
//            }

//            UgxMaterial[] materials = ReadMaterials(new MemoryStream(cmaterials.Data));


//            List<UgxSection> sections = new List<UgxSection>();
//        }

//        public long[] ReadVertexOffsets(Stream stream)
//        {

//        }
//        public long[] ReadVertexCounts(Stream stream)
//        {

//        }
//        public string ReadVertexCounts(Stream stream)
//        {

//        }
//        public static UgxMaterial[] ReadMaterials(Stream stream)
//        {

//        }
//    }
//}
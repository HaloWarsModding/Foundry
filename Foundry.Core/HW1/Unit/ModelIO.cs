using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Chef.Util;

namespace Chef.HW1.Unit
{
    //public class UgxCachedVertextFormat
    //{
    //    public PackedElement PackOrder { get; set; } = new PackedElement();
    //    public PackedElement DeclOrder { get; set; } = new PackedElement();
    //    public int[] ElementTypes { get; set; }

    //    public bool Read(BinaryReader r)
    //    {
    //        PackOrder = new PackedElement();
    //        PackOrder.Read(r);
    //        DeclOrder = new PackedElement();
    //        DeclOrder.Read(r);
    //        ElementTypes = new int[17];
    //        for (int i = 0; i < ElementTypes.Length; i++)
    //            ElementTypes[i] = r.ReadInt32();
    //        r.ReadInt32();
    //        return true;
    //    }
    //    public bool Write(BinaryWriter w)
    //    {
    //        PackOrder.Write(w);
    //        DeclOrder.Write(w);
    //        for (int i = 0; i < 17; i++)
    //            w.Write(ElementTypes[i]);
    //        w.Write(0);
    //        return true;
    //    }
    //    public bool Pack(BinaryWriter w)
    //    {
    //        PackOrder.Pack(w);
    //        DeclOrder.Pack(w);
    //        return true;
    //    }
    //}
    //public class UgxCachedSection
    //{
    //    public int MaterialIndex { get; set; }
    //    public int AccessoryIndex { get; set; }
    //    public int MaxBones { get; set; }
    //    public int RigidBoneIndex { get; set; }

    //    public int IndexBufferOffset { get; set; }
    //    public int TriangleCount { get; set; }

    //    public int VertexBufferOffset { get; set; }
    //    public int VertexBufferSize { get; set; }
    //    public int VertexSize { get; set; }
    //    public int VertexCount { get; set; }

    //    public PackedArray<StreamableInt> GlobalToLocalBoneRemap { get; set; } = new PackedArray<StreamableInt>(); //unused.
    //    public UgxCachedVertextFormat VertexFormat { get; set; } = new UgxCachedVertextFormat();

    //    public bool RigidOnly { get; set; }


    //    public bool Read(BinaryReader r)
    //    {
    //        MaterialIndex = r.ReadInt32();
    //        AccessoryIndex = r.ReadInt32();
    //        MaxBones = r.ReadInt32();
    //        RigidBoneIndex = r.ReadInt32();
    //        IndexBufferOffset = r.ReadInt32();
    //        TriangleCount = r.ReadInt32();
    //        VertexBufferOffset = r.ReadInt32();
    //        VertexBufferSize = r.ReadInt32();
    //        VertexSize = r.ReadInt32();
    //        VertexCount = r.ReadInt32();
    //        GlobalToLocalBoneRemap = new PackedArray<StreamableInt>();
    //        GlobalToLocalBoneRemap.Read(stream);
    //        VertexFormat = new UgxCachedVertextFormat();
    //        VertexFormat.Read(stream);
    //        RigidOnly = r.ReadBoolean();
    //        /*PAD0*/
    //        r.ReadBytes(7);
    //        return true;
    //    }
    //    public bool Write(BinaryWriter w)
    //    {
    //        w.Write(MaterialIndex);
    //        w.Write(AccessoryIndex);
    //        w.Write(MaxBones);
    //        w.Write(RigidBoneIndex);
    //        w.Write(IndexBufferOffset);
    //        w.Write(TriangleCount);
    //        w.Write(VertexBufferOffset);
    //        w.Write(VertexBufferSize);
    //        w.Write(VertexSize);
    //        w.Write(VertexCount);
    //        GlobalToLocalBoneRemap.Write(stream);
    //        VertexFormat.Write(stream);
    //        w.Write(RigidOnly);
    //        w.Write(new byte[7] /*PAD0*/);
    //        return true;
    //    }
    //    public bool Pack(BinaryWriter w)
    //    {
    //        VertexFormat.Pack(w);
    //        return true;
    //    }
    //}
    //public class UgxCachedBone
    //{
    //    public PackedElement NameHeader { get; set; }
    //    public StreamableMatrix4 Transform { get; set; }
    //    public int ParentIndex { get; set; }
    //    public int PAD0 { get; set; }

    //    public bool Read(BinaryReader r)
    //    {
    //        NameHeader.Read(r);
    //        Transform.Read(r);
    //        ParentIndex = r.ReadInt32();
    //        /*PAD0*/ = r.ReadInt32();
    //        return true;
    //    }
    //    public bool Write(Stream stream)
    //    {
    //        NameHeader.Write(w);
    //        Transform.Write(stream, Endianness.Little);
    //        w.Write(ParentIndex);
    //        w.Write(0 /*PAD0*/);
    //        return true;
    //    }
    //    public bool Pack(BinaryWriter r)
    //    {
    //        Name.Pack(r);
    //        return true;
    //    }
    //}
    //public class UgxCachedAccessory
    //{
    //    public int FirstBone { get; set; }
    //    public int NumberOfBones { get; set; }
    //    private PackedArray SectionIndicesHeader { get; set; }

    //    public void Read(Stream stream)
    //    {
    //        using (BinaryReader r = new BinaryReader(stream, Encoding.ASCII, true))
    //        {
    //            FirstBone = r.ReadInt32();
    //            NumberOfBones = r.ReadInt32();
    //            SectionIndicesHeader.Read(stream);
    //        }
    //    }
    //    public void Write(Stream stream)
    //    {
    //        using (BinaryWriter w = new BinaryWriter(stream, Encoding.ASCII, true))
    //        {
    //            w.Write(FirstBone);
    //            w.Write(NumberOfBones);
    //            SectionIndicesHeader.Write(stream);
    //        }
    //    }
    //}
    //public class UgxCachedData
    //{
    //    public uint Signature { get; set; }
    //    public int RigidBoneIndex { get; set; }
    //    public StreamableVector3 BoundingSphereCenter { get; set; } = new StreamableVector3();
    //    public float BoundingSphereRadius { get; set; }
    //    public StreamableVector3 BoundsMin { get; set; } = new StreamableVector3();
    //    public StreamableVector3 BoundsMax { get; set; } = new StreamableVector3();
    //    public short MaxInstances { get; set; }
    //    public short InstanceIndexMultiplier { get; set; }
    //    public short LargeGeomBoneIndex { get; set; }
    //    public bool AllSectionRigid { get; set; }
    //    public bool GlobalBones { get; set; }
    //    public bool AllSectionsSkinned { get; set; }
    //    public bool RigidOnly { get; set; }
    //}

    //public static class Ugx
    //{
    //    public static List<Model> ReadEcf(Stream stream)
    //    {
    //        EcfFile ecf = new EcfFile();
    //        EcfChunk cgranny = null;
    //        EcfChunk ccached = null;
    //        EcfChunk cvb = null;
    //        EcfChunk cib = null;
    //        EcfChunk cmaterials = null;

    //        foreach (EcfChunk chunk in ecf.Chunks)
    //        {
    //            if (chunk.ID == 0x703) cgranny = chunk;
    //            if (chunk.ID == 0x700) ccached = chunk;
    //            if (chunk.ID == 0x702) cib = chunk;
    //            if (chunk.ID == 0x701) cvb = chunk;
    //            if (chunk.ID == 0x704) cmaterials = chunk;
    //        }

    //        ModelMaterial[] materials = ReadMaterials(new MemoryStream(cmaterials.Data));


    //        List<Model> sections = new List<Model>();
    //    }

    //    public long[] ReadVertexOffsets(Stream stream)
    //    {

    //    }
    //    public long[] ReadVertexCounts(Stream stream)
    //    {

    //    }
    //    public string ReadVertexCounts(Stream stream)
    //    {

    //    }
    //    public static ModelMaterial[] ReadMaterials(Stream stream)
    //    {

    //    }
    //}

    public static class ModelIO
    {
        public static Model ReadUgx(Stream stream)
        {
            Ecf ecf = new Ecf();
            ecf.Read(stream);

            EcfChunk granny = null;
            EcfChunk cached = null;
            EcfChunk vb = null;
            EcfChunk ib = null;
            EcfChunk material = null;

            foreach (EcfChunk chunk in ecf.Chunks)
            {
                if (chunk.ID == 0x703) granny = chunk;
                if (chunk.ID == 0x700) cached = chunk;
                if (chunk.ID == 0x702) vb = chunk;
                if (chunk.ID == 0x701) ib = chunk;
                if (chunk.ID == 0x704) material = chunk;
            }

            if (granny == null ||
                cached == null ||
                vb == null ||
                ib == null ||
                material == null) return null;

            Model model = new Model();
            int[] sectionMaterialIndices;
            ReadCached(
                new MemoryStream(cached.Data),
                new MemoryStream(vb.Data),
                new MemoryStream(ib.Data),
                model,
                out sectionMaterialIndices);
            ReadMaterials(
                new MemoryStream(material.Data),
                model,
                sectionMaterialIndices);

            Vector3 min = Vector3.One * 999999;
            Vector3 max = Vector3.One * -999999;
            foreach (ModelSection s in model.Sections)
            {
                foreach(ModelVertex v in s.Vertices)
                {
                    min = Vector3.Min(min, v.Position);
                    max = Vector3.Max(max, v.Position);
                }
            }
            model.BoundsMin = min;
            model.BoundsMax = max;

            return model;
        }
        public static ModelVertexSpec SpecFromString(string s)
        {
            ModelVertexSpec spec = new ModelVertexSpec();
            for (int i = 0; i < s.Length; i++)
            {
                switch (s[i])
                {
                    case (char)ModelVertexElemUsage.POSITION:
                        spec.HasPosition = true;
                        break;
                    case (char)ModelVertexElemUsage.BASIS:
                    case (char)ModelVertexElemUsage.TANGENT:
                        spec.NumBasis++;
                        i++; //skip index char.
                        break;
                    case (char)ModelVertexElemUsage.NORMAL:
                        spec.HasNormals = true;
                        break;
                    case (char)ModelVertexElemUsage.UV:
                        spec.NumUVs++;
                        i++; //skip index char.
                        break;
                    case (char)ModelVertexElemUsage.SKIN:
                        spec.HasSkin = true;
                        break;
                    case (char)ModelVertexElemUsage.INDEX:
                        break;
                    case (char)ModelVertexElemUsage.BASIS_SCALE:
                        break;
                }
            }
            return spec;
        }

        private static void ReadCached(Stream cached, Stream vb, Stream ib, Model model, out int[] sectionMaterialIndices)
        {
            using (BinaryReader reader = new BinaryReaderEndian(cached, Encoding.ASCII, true, Endianness.Little))
            {
                const uint ver4 = 0xC2340004; 
                const uint ver6 = 0xC2340006;
                uint version = reader.ReadUInt32();

                int rigidBoneIndex = reader.ReadInt32();
                Vector3 boundingSphereCenter = new Vector3(
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle());
                float boundingSphereRadius = reader.ReadSingle();
                Vector3 boundsMin = new Vector3(
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle());
                Vector3 boundsMax = new Vector3(
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle());
                short maxInstances = reader.ReadInt16();
                short instanceIndexMultiplier = reader.ReadInt16();
                short largeGeomBoneIndex = reader.ReadInt16();
                bool allRigid = reader.ReadBoolean();
                bool globalBones = reader.ReadBoolean();
                bool allSkinned = reader.ReadBoolean();
                bool rigidOnly = reader.ReadBoolean();
                /*PAD0*/
                reader.ReadBytes(2);
                /*PAD1 for v6 only*/
                if (version == ver6) reader.ReadBytes(4);

                if (version == ver4) reader.ReadUInt32();
                uint sectionsCount = reader.ReadUInt32();
                reader.ReadUInt32();
                uint sectionsOffset = reader.ReadUInt32();
                if (version == ver6) reader.ReadUInt32();

                sectionMaterialIndices = new int[sectionsCount];

                //read sections
                List<ModelSection> sections = new List<ModelSection>();
                long _pos_headerEnd = cached.Position;
                cached.Position = sectionsOffset;
                for (uint i = 0; i < sectionsCount; i++)
                {
                    ModelSection section = new ModelSection();

                    sectionMaterialIndices[i] = reader.ReadInt32();
                    int accessoryIndex = reader.ReadInt32();
                    int maxBones = reader.ReadInt32();
                    int secRigidBoneIndex = reader.ReadInt32();
                    int indexBufferOffset = reader.ReadInt32() * 2; //number is in indices, so we convert to bytes.
                    int indexCount = reader.ReadInt32() * 3; //number is in triangles, so convert to indices.
                    int vertexBufferOffset = reader.ReadInt32();
                    int vertexBufferSize = reader.ReadInt32();
                    int vertexSize = reader.ReadInt32();
                    int vertexCount = reader.ReadInt32();

                    bool secRigidOnly;

                    if (version == ver6)
                    {
                        secRigidOnly = reader.ReadBoolean();
                        /*PAD0*/
                        reader.ReadBytes(3);
                        float lodMinDistance = reader.ReadSingle();
                        float lodMaxDistance = reader.ReadSingle();
                        float sortingBias = reader.ReadSingle();
                    }

                    /*pad for unused array header*/
                    reader.ReadBytes(16);
                    ulong packOrderOffset = reader.ReadUInt32();

                    if (version == ver4)
                    {
                        /*PAD0*/
                        reader.ReadBytes(4);
                        /*Pack types*/
                        reader.ReadBytes(80);
                        secRigidOnly = reader.ReadBoolean();
                        /*PAD1*/
                        reader.ReadBytes(7);

                        //read pack order string.
                        long _pos_curSection = cached.Position;
                        cached.Position = (long)packOrderOffset;
                        string packOrder = "";
                        char c = reader.ReadChar();
                        while (c != '\0')
                        {
                            packOrder += c;
                            c = reader.ReadChar();
                        }
                        cached.Position = _pos_curSection; //seek back to the end of the section.
                        section.VertexSpec = SpecFromString(packOrder);
                    }

                    //read vertices
                    vb.Position = vertexBufferOffset;
                    section.Vertices = ReadVertices(vb, section.VertexSpec, vertexCount, vertexSize);

                    //read indices
                    ib.Position = indexBufferOffset;
                    section.Indices = ReadIndices(ib, indexCount);

                    sections.Add(section);
                }
                model.Sections = sections.ToArray();
            }
        }
        private static ModelVertex[] ReadVertices(Stream vb, ModelVertexSpec spec, int count, int stride)
        {
            ModelVertex[] ret = new ModelVertex[count];
            using (BinaryReader reader = new BinaryReaderEndian(vb, Encoding.ASCII, true, Endianness.Little))
            {
                long _pos_last = vb.Position;
                for (int i = 0; i < count; i++)
                {
                    ret[i] = new ModelVertex();
                    if (spec.HasPosition)
                    {
                        ret[i].Position = new Vector3(
                        (float)reader.ReadHalf(),
                        (float)reader.ReadHalf(),
                        (float)reader.ReadHalf());

                        reader.ReadHalf(); //extra float to pad to 8 bytes.
                    }

                    if (spec.HasNormals)
                        ret[i].Normal = new Vector3(
                        (float)reader.ReadSingle(),
                        (float)reader.ReadSingle(),
                        (float)reader.ReadSingle());

                    for (int b = 0; b < spec.NumBasis; b++)
                    {
                        /*ret[i].Tangent =*/ new Vector3(
                        (float)reader.ReadSingle(),
                        (float)reader.ReadSingle(),
                        (float)reader.ReadSingle());
                    }

                    if (spec.HasSkin)
                    {
                        /*ret[i].Bone0 = */reader.ReadByte();
                        /*ret[i].Bone1 = */reader.ReadByte();
                        /*ret[i].Bone2 = */reader.ReadByte();
                        /*ret[i].Bone3 = */reader.ReadByte();

                        /*ret[i].Weight0 = */reader.ReadByte();
                        /*ret[i].Weight1 = */reader.ReadByte();
                        /*ret[i].Weight2 = */reader.ReadByte();
                        /*ret[i].Weight3 = */reader.ReadByte();
                    }

                    if (spec.NumUVs > 0)
                        ret[i].Uv0 = new Vector2(
                        (float)reader.ReadHalf(),
                        (float)reader.ReadHalf());

                    if (spec.NumUVs > 1)
                        ret[i].Uv1 = new Vector2(
                        (float)reader.ReadHalf(),
                        (float)reader.ReadHalf());

                    if (spec.NumUVs > 2)
                        ret[i].Uv2 = new Vector2(
                        (float)reader.ReadHalf(),
                        (float)reader.ReadHalf());

                    vb.Position = _pos_last + stride;
                    _pos_last = vb.Position;
                }
            }

            return ret;
        }
        private static ushort[] ReadIndices(Stream ib, int count)
        {
            ushort[] ret = new ushort[count];
            using (BinaryReader reader = new BinaryReaderEndian(ib, Encoding.ASCII, true, Endianness.Little))
            {
                for (int i = 0; i < count; i++)
                {
                    ret[i] = reader.ReadUInt16();
                }
            }
            return ret;
        }

        private static void ReadMaterials(Stream stream, Model model, int[] sectionMaterialIndices)
        {
            XDocument doc = UnpackBinaryDataTree(stream);

            List<ModelMaterial> materials = new List<ModelMaterial>();

            foreach(var m in doc.Element("Materials").Elements("Material"))
            {
                ModelMaterial material = new ModelMaterial();

                var diffuse = m.Element("Maps").Element("diffuse").Element("Map");
                if (diffuse != null)
                {
                    material.SetMap(ModelMaterialMap.Diffuse, diffuse.Attribute("Name").Value, new Vector3(0, 0, 0));
                }

                materials.Add(material);
            }

            for (int i = 0; i < model.Sections.Count(); i++)
            {
                model.Sections[i].Material = materials[sectionMaterialIndices[i]];
            }
        }
        /// <summary>
        /// Unpack a binary data tree from a stream to an xml document.
        /// </summary>
        /// As far as I know this isnt used anywhere else, so it can stay here for now.
        private static XDocument UnpackBinaryDataTree(Stream stream)
        {
            XDocument doc = new XDocument();
            using (BinaryReaderEndian reader = new BinaryReaderEndian(stream, Encoding.ASCII, true, Endianness.Little))
            {
                stream.Position = 12;
                uint numNodes = reader.ReadUInt32() / 8;
                uint numNameValues = reader.ReadUInt32() / 8;
                uint namesBytes = reader.ReadUInt32();
                uint valuesBytes = reader.ReadUInt32();
                stream.Position = 28;

                long _pos_nameValuesStart = (numNodes * 8) + 28;
                long _pos_namesStart = _pos_nameValuesStart + (numNameValues * 8);
                long _pos_valuesStart = _pos_namesStart + namesBytes;
                _pos_valuesStart += 16 - (_pos_valuesStart % 16); //values are aligned to 16 bytes.

                XElement[] nodes = new XElement[numNodes];

                long _pos_lastNode = stream.Position;
                for (int i = 0; i < numNodes; i++)
                {
                    stream.Position = _pos_lastNode;
                    ushort parent = reader.ReadUInt16();

                    nodes[i] = new XElement("PLACEHOLDER");
                    if (parent != 0xFFFF)
                        nodes[parent].Add(nodes[i]);
                    else
                        doc.Add(nodes[i]);

                    reader.ReadUInt16(); //childNodeIndex
                    ushort nvOffset = reader.ReadUInt16();
                    ushort nvCount = reader.ReadByte();
                    reader.ReadByte(); //numChildNodes
                    _pos_lastNode += 8;

                    //seek to name values
                    long _pos_curNV = _pos_nameValuesStart + (nvOffset * 8);
                    for (int n = 0; n < nvCount; n++)
                    {
                        stream.Position = _pos_curNV;
                        uint value = reader.ReadUInt32();
                        ushort nameOffset = reader.ReadUInt16();
                        ushort flags = reader.ReadUInt16();
                        _pos_curNV += 8;

                        int valType = (flags >> 2) & ((1 << 3) - 1);
                        int valSize = 1 << ((flags >> 5) & ((1 << 3) - 1));
                        int valTotalSize = (flags >> 9) & ((1 << 7) - 1);
                        bool valUnsigned = (flags & 1) != 0;
                        long _pos_cur = stream.Position;

                        string valString = "";
                        switch (valType)
                        {
                            case 0: //null
                                break;
                            case 1:
                                if (value == 0) valString = "false";
                                else valString = "true";
                                break;
                            case 2:
                                valString = value.ToString();
                                break;
                            case 3: //float
                                int fcount = valTotalSize / valSize;
                                if (fcount == 1) //bit-casts int value
                                {
                                    float f = BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
                                    valString = f.ToString("0.0000");
                                }
                                else //reads from value data
                                {
                                    float[] floats = new float[fcount];
                                    stream.Position = _pos_valuesStart + value;
                                    for (int f = 0; f < fcount; f++)
                                    {
                                        valString += reader.ReadSingle().ToString("0.0000");
                                        if (f != fcount - 1) valString += ",";
                                    }
                                    stream.Position = _pos_cur;
                                }
                                break;
                            case 4: //string
                                stream.Position = _pos_valuesStart + value;
                                char vc = reader.ReadChar();
                                while (vc != '\0')
                                {
                                    valString += vc;
                                    vc = reader.ReadChar();
                                }
                                stream.Position = _pos_cur;
                                break;
                        }

                        stream.Position = _pos_namesStart + nameOffset;
                        string nameString = "";
                        char c = reader.ReadChar();
                        while (c != '\0')
                        {
                            nameString += c;
                            c = reader.ReadChar();
                        }

                        if (n == 0)
                        {
                            nodes[i].Name = nameString;
                            nodes[i].Value = valString;
                        }
                        else
                        {
                            nodes[i].Add(new XAttribute(nameString, valString));
                        }
                    }
                }
            }
            return doc;
        }
    }
}
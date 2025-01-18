using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Endianness = Chef.Util.Endianness;

namespace Chef.Util
{
    public class EcfChunk
    {
        public ulong ID { get; set; }
        private uint Adler32 { get; set; }
        private byte Flags { get; set; }
        private byte AlignmentLog2 { get; set; }
        private ushort ResourceFlags { get; set; }
        public byte[] Data { get; set; } = default;

        private long _Pos_Offset = -1;
        private long _Pos_Size = -1;

        public bool Read(Stream stream)
        {
            using (BinaryReader r = new BinaryReaderEndian(stream, Encoding.ASCII, true, Endianness.Big))
            {
                ID = r.ReadUInt64();
                uint offs = r.ReadUInt32();
                uint size = r.ReadUInt32();
                Adler32 = r.ReadUInt32();
                Flags = r.ReadByte();
                AlignmentLog2 = r.ReadByte();
                ResourceFlags = r.ReadUInt16();

                long ogpos = stream.Position;
                stream.Position = offs;
                Data = new byte[size];
                stream.Read(Data, 0, Data.Length);
                stream.Position = ogpos;
            }
            return true;
        }
        public bool Write(Stream stream)
        {
            using (BinaryWriter w = new BinaryWriterEndian(stream, Encoding.ASCII, true, Endianness.Big))
            {
                w.Write(ID);
                _Pos_Offset = stream.Position;
                w.Write(0xFFFFFFFF);
                _Pos_Size = stream.Position;
                w.Write(0);
                w.Write(Adler32);
                w.Write(Flags);
                w.Write(AlignmentLog2);
                w.Write(ResourceFlags);
            }
            return true;
        }
        public bool Pack(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriterEndian(stream, Encoding.ASCII, true, Endianness.Big))
            {
                long packPos = writer.BaseStream.Position + writer.BaseStream.Position % 16 /*align to 16 bytes*/;

                writer.BaseStream.Position = _Pos_Offset;
                writer.Write((int)packPos);

                writer.BaseStream.Position = _Pos_Size;
                writer.Write(Data.Length);

                writer.BaseStream.Position = packPos;
                writer.BaseStream.Write(Data, 0, Data.Length);
            }
            return true;
        }
    }


    public class Ecf
    {
        private uint HeaderSignature { get; set; }
        private uint HeaderSize { get; set; }
        private uint HeaderAdler32 { get; set; }
        private uint FileSize { get; set; }
        private ushort Flags { get; set; }
        public uint ID { get; set; }
        private ushort ChunkExtraDataSize { get; set; }
        public List<EcfChunk> Chunks { get; set; } = new List<EcfChunk>();

        private long _Pos_NumChunks = -1;
        private long _Pos_Adler32 = -1;
        private long _Pos_FileSize = -1;

        public bool Read(Stream stream)
        {
            using (BinaryReader r = new BinaryReaderEndian(stream, Encoding.ASCII, true, Endianness.Big))
            {
                HeaderSignature = r.ReadUInt32();
                HeaderSize = r.ReadUInt32();
                _Pos_Adler32 = stream.Position;
                HeaderAdler32 = r.ReadUInt32();
                _Pos_FileSize = stream.Position;
                FileSize = r.ReadUInt32();
                _Pos_NumChunks = stream.Position;
                ushort chunks = r.ReadUInt16();
                Flags = r.ReadUInt16();
                ID = r.ReadUInt32();
                ChunkExtraDataSize = r.ReadUInt16();
                /*PAD*/
                r.ReadBytes(6);

                Chunks = new List<EcfChunk>();
                for (int i = 0; i < chunks; i++)
                {
                    var chunk = new EcfChunk();
                    chunk.Read(stream);
                    Chunks.Add(chunk);
                }
            }
            return true;
        }
        public bool Write(Stream stream)
        {
            using (BinaryWriter w = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                w.Write(HeaderSignature);
                w.Write(HeaderSize);
                w.Write(HeaderAdler32);
                w.Write(FileSize);
                w.Write(Chunks.Count);
                w.Write(Flags);
                w.Write(ID);
                w.Write(ChunkExtraDataSize);
                w.Write(new byte[6] /*PAD*/ );
                foreach (EcfChunk chunk in Chunks)
                    chunk.Write(stream);
            }
            return true;
        }

        public bool Pack(Stream stream)
        {
            foreach (EcfChunk chunk in Chunks)
                chunk.Pack(stream);
            return true;
        }
    }
}

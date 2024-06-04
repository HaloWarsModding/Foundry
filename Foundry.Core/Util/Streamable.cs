using Foundry.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace Foundry.Util
{
    public class StreamableVector3
    {
        public StreamableVector3() { X = 0; Y = 0; Z = 0; }
        public StreamableVector3(float x, float y, float z) { X = x; Y = y; Z = z; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public bool Read(Stream stream, Endianness endianness = Endianness.Little)
        {
            using (BinaryReader reader = new BinaryReaderEndian(stream, Encoding.ASCII, true, endianness))
            {
                X = reader.ReadSingle();
                Y = reader.ReadSingle();
                Z = reader.ReadSingle();
            }
            return true;
        }
        public bool Write(Stream stream, Endianness endianness = Endianness.Little)
        {
            using (BinaryWriter writer = new BinaryWriterEndian(stream, Encoding.ASCII, true, endianness))
            {
                writer.Write(X);
                writer.Write(Y);
                writer.Write(Z);
            }
            return true;
        }

        public static implicit operator Vector3(StreamableVector3 i) => new Vector3(i.X, i.Y, i.X);
        public static implicit operator StreamableVector3(Vector3 i) => new StreamableVector3(i.X, i.Y, i.Z);
    }
    public class StreamableVector4
    {
        public StreamableVector4() { X = 0; Y = 0; Z = 0; W = 0; }
        public StreamableVector4(float x, float y, float z, float w) { X = x; Y = y; Z = z; W = w; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public bool Read(Stream stream, Endianness endianness)
        {
            using (BinaryReader reader = new BinaryReaderEndian(stream, Encoding.ASCII, true, endianness))
            {
                X = reader.ReadSingle();
                Y = reader.ReadSingle();
                Z = reader.ReadSingle();
                W = reader.ReadSingle();
            }
            return true;
        }
        public bool Write(Stream stream, Endianness endianness)
        {
            using (BinaryWriter writer = new BinaryWriterEndian(stream, Encoding.ASCII, true, endianness))
            {
                writer.Write(X);
                writer.Write(Y);
                writer.Write(Z);
                writer.Write(W);
            }
            return true;
        }

        public static implicit operator Vector4(StreamableVector4 i) => new Vector4(i.X, i.Y, i.X, i.W);
        public static implicit operator StreamableVector4(Vector4 i) => new StreamableVector4(i.X, i.Y, i.Z, i.W);
    }
    public class StreamableMatrix3
    {
        public StreamableMatrix3()
        {
            R0 = new StreamableVector3(1, 0, 0);
            R1 = new StreamableVector3(0, 1, 0);
            R2 = new StreamableVector3(0, 0, 1);
        }
        public StreamableMatrix3(StreamableVector3 r0, StreamableVector3 r1, StreamableVector3 r2)
        {
            R0 = r0;
            R1 = r1;
            R2 = r2;
        }

        public StreamableVector3 R0, R1, R2;

        public bool Read(Stream stream, Endianness endianness)
        {
            R0.Read(stream, endianness);
            R1.Read(stream, endianness);
            R2.Read(stream, endianness);
            return true;
        }
        public bool Write(Stream stream, Endianness endianness)
        {
            R0.Write(stream, endianness);
            R1.Write(stream, endianness);
            R2.Write(stream, endianness);
            return true;
        }

        //public static implicit operator Matrix4x4(StreamableMatrix4 i) => new Matrix4x4(i.X, i.Y, i.X);
        //public static implicit operator StreamableMatrix4(Matrix4x4 i) => new StreamableMatrix4(i.X, i.Y, i.Z);
    }
    public class StreamableMatrix4
    {
        public StreamableMatrix4()
        {
            R0 = new StreamableVector4(1, 0, 0, 0);
            R1 = new StreamableVector4(0, 1, 0, 0);
            R2 = new StreamableVector4(0, 0, 1, 0);
            R3 = new StreamableVector4(0, 0, 0, 1);
        }
        public StreamableMatrix4(StreamableVector4 r0, StreamableVector4 r1, StreamableVector4 r2, StreamableVector4 r3)
        {
            R0 = r0;
            R1 = r1;
            R2 = r2;
            R3 = r3;
        }

        public StreamableVector4 R0, R1, R2, R3;


        public bool Read(Stream stream, Endianness endianness)
        {
            R0.Read(stream, endianness);
            R1.Read(stream, endianness);
            R2.Read(stream, endianness);
            R3.Read(stream, endianness);
            return true;
        }
        public bool Write(Stream stream, Endianness endianness)
        {
            R0.Write(stream, endianness);
            R1.Write(stream, endianness);
            R2.Write(stream, endianness);
            R3.Write(stream, endianness);
            return true;
        }

        //public static implicit operator Matrix4x4(StreamableMatrix4 i) => new Matrix4x4(i.X, i.Y, i.X);
        //public static implicit operator StreamableMatrix4(Matrix4x4 i) => new StreamableMatrix4(i.X, i.Y, i.Z);
    }
    public class StreamableInt
    {
        public int Value { get; set; }

        public bool Read(Stream stream, Endianness endianness)
        {
            using (BinaryReader reader = new BinaryReaderEndian(stream, Encoding.ASCII, true, endianness))
                Value = reader.ReadInt32();
            return true;
        }
        public bool Write(Stream stream, Endianness endianness)
        {
            using (BinaryWriter writer = new BinaryWriterEndian(stream, Encoding.ASCII, true, endianness))
                writer.Write(Value);
            return true;
        }

        public static implicit operator int(StreamableInt i) => i.Value;
        public static implicit operator StreamableInt(int i) => i;
    }
}

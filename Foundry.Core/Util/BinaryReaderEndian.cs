using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chef.Util
{
    public class BinaryReaderEndian : BinaryReader
    {
        public Endianness Endianness { get; set; } = Endianness.Little;
        public BinaryReaderEndian(Stream input, Endianness endianness = Endianness.Little) : base(input) { Endianness = endianness; }
        public BinaryReaderEndian(Stream input, Encoding encoding, Endianness endianness = Endianness.Little) : base(input, encoding) { Endianness = endianness; }
        public BinaryReaderEndian(Stream input, Encoding encoding, bool leaveOpen, Endianness endianness = Endianness.Little) : base(input, encoding, leaveOpen) { Endianness = endianness; }

        public override ushort ReadUInt16()
        {
            if (Endianness == Endianness.Big)
                return BinaryPrimitives.ReverseEndianness(base.ReadUInt16());
            else
                return base.ReadUInt16();
        }
        public override uint ReadUInt32()
        {
            if (Endianness == Endianness.Big)
                return BinaryPrimitives.ReverseEndianness(base.ReadUInt32());
            else
                return base.ReadUInt32();
        }
        public override ulong ReadUInt64()
        {
            if (Endianness == Endianness.Big)
                return BinaryPrimitives.ReverseEndianness(base.ReadUInt64());
            else
                return base.ReadUInt64();
        }
        public override short ReadInt16()
        {
            if (Endianness == Endianness.Big)
                return BinaryPrimitives.ReverseEndianness(base.ReadInt16());
            else
                return base.ReadInt16();
        }
        public override int ReadInt32()
        {
            if (Endianness == Endianness.Big)
                return BinaryPrimitives.ReverseEndianness(base.ReadInt32());
            else
                return base.ReadInt32();
        }
        public override long ReadInt64()
        {
            if (Endianness == Endianness.Big)
                return BinaryPrimitives.ReverseEndianness(base.ReadInt64());
            else
                return base.ReadInt64();
        }
        public override float ReadSingle()
        {
            if (Endianness == Endianness.Big)
                return BitConverter.ToSingle(
                    BitConverter.GetBytes(
                        base.ReadSingle()
                        ).Reverse().ToArray()
                    );
            else
                return base.ReadSingle();
        }
        public override double ReadDouble()
        {
            if (Endianness == Endianness.Big)
                return BitConverter.ToDouble(
                    BitConverter.GetBytes(
                        base.ReadDouble()
                        ).Reverse().ToArray()
                    );
            else
                return base.ReadDouble();
        }
    }
}

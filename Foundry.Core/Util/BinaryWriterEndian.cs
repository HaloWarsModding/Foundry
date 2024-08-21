using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chef.Util
{
    public class BinaryWriterEndian : BinaryWriter
    {
        public Endianness Endianness { get; set; }
        public BinaryWriterEndian(Endianness endianness = Endianness.Little) : base() { Endianness = endianness; }
        public BinaryWriterEndian(Stream input, Endianness endianness = Endianness.Little) : base(input) { Endianness = endianness; }
        public BinaryWriterEndian(Stream input, Encoding encoding, Endianness endianness = Endianness.Little) : base(input, encoding) { Endianness = endianness; }
        public BinaryWriterEndian(Stream input, Encoding encoding, bool leaveOpen, Endianness endianness = Endianness.Little) : base(input, encoding, leaveOpen) { Endianness = endianness; }

        public override void Write(short value)
        {
            if (Endianness == Endianness.Big)
                base.Write(BinaryPrimitives.ReverseEndianness(value));
            else
                base.Write(value);
        }
        public override void Write(int value)
        {
            if (Endianness == Endianness.Big)
                base.Write(BinaryPrimitives.ReverseEndianness(value));
            else
                base.Write(value);
        }
        public override void Write(long value)
        {
            if (Endianness == Endianness.Big)
                base.Write(BinaryPrimitives.ReverseEndianness(value));
            else
                base.Write(value);
        }
        public override void Write(ushort value)
        {
            if (Endianness == Endianness.Big)
                base.Write(BinaryPrimitives.ReverseEndianness(value));
            else
                base.Write(value);
        }
        public override void Write(uint value)
        {
            if (Endianness == Endianness.Big)
                base.Write(BinaryPrimitives.ReverseEndianness(value));
            else
                base.Write(value);
        }
        public override void Write(ulong value)
        {
            if (Endianness == Endianness.Big)
                base.Write(BinaryPrimitives.ReverseEndianness(value));
            else
                base.Write(value);
        }
        public override void Write(float value)
        {
            if (Endianness == Endianness.Big)
                base.Write(
                    BitConverter.ToSingle(
                        BitConverter.GetBytes(value).Reverse().ToArray()
                        )
                    );
            else
                base.Write(value);
        }
        public override void Write(double value)
        {
            if (Endianness == Endianness.Big)
                base.Write(
                    BitConverter.ToDouble(
                        BitConverter.GetBytes(value).Reverse().ToArray()
                        )
                    );
            else
                base.Write(value);
        }
    }
}
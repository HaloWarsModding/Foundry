using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chef.Util
{
    public class PackedElement
    {
        private uint Offset { get; set; }

        private long _Pos_Offset = -1;

        public bool Read(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                Offset = reader.ReadUInt32();
            }
            return true;
        }
        public bool Write(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                //values will get written when packed.
                _Pos_Offset = writer.BaseStream.Position;
                writer.Write(0xFFFFFFFF);
                writer.Write(0);
            }
            return true;
        }
        public virtual void Update(Stream stream, int position)
        {
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                long packPos = writer.BaseStream.Position;

                //write values to header
                writer.BaseStream.Position = _Pos_Offset;
                writer.Write(position);
            }
        }
        public string Unpack(Stream stream)
        {
            string value = "";
            using (BinaryReader reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                long original = reader.BaseStream.Position;
                reader.BaseStream.Position = Offset;
                char cur = reader.ReadChar();
                while (cur != '\0')
                {
                    value += cur;
                    cur = reader.ReadChar();
                }
                reader.BaseStream.Position = original;
            }
            return value;
        }
    }
    public class PackedArray
    {
        public bool FlipPadOrder { get; set; } = false;

        private long Offset { get; set; }

        private long _Pos_Offset = -1;
        private long _Pos_Length = -1;

        public bool Read(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                if (FlipPadOrder) reader.ReadBytes(4);
                uint len = reader.ReadUInt32();
                /*always padded*/
                reader.ReadBytes(4);
                Offset = reader.ReadUInt32();
                if (!FlipPadOrder) reader.ReadBytes(4);
            }
            return true;
        }
        public bool Write(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                //values will get written when packed.
                if (FlipPadOrder) writer.Write(new byte[4]);
                _Pos_Length = writer.BaseStream.Position;
                writer.Write(0);
                /*always padded*/
                writer.Write(new byte[4]);
                _Pos_Offset = writer.BaseStream.Position;
                writer.Write(0xFFFFFFFF);
                if (!FlipPadOrder) writer.Write(new byte[4]);
            }
            return true;
        }
        public virtual void Update(Stream stream, int position, int count)
        {
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                //write values to header
                writer.BaseStream.Position = _Pos_Length;
                writer.Write(count);
                writer.BaseStream.Position = _Pos_Offset;
                writer.Write(position);
            }
        }
    }
}

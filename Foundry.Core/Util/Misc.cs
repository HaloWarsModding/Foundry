using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Foundry.util
{
    public enum Endianness
    {
        Little,
        Big
    }

    public static class Misc
	{
        public static float ToFloat16(byte HO, byte LO)
		{
			var intVal = BitConverter.ToInt32(new byte[] { HO, LO, 0, 0 }, 0);

			int mant = intVal & 0x03ff;
			int exp = intVal & 0x7c00;
			if (exp == 0x7c00) exp = 0x3fc00;
			else if (exp != 0)
			{
				exp += 0x1c000;
				if (mant == 0 && exp > 0x1c400)
					return BitConverter.ToSingle(BitConverter.GetBytes((intVal & 0x8000) << 16 | exp << 13 | 0x3ff), 0);
			}
			else if (mant != 0)
			{
				exp = 0x1c400;
				do
				{
					mant <<= 1;
					exp -= 0x400;
				} while ((mant & 0x400) == 0);
				mant &= 0x3ff;
			}
			return BitConverter.ToSingle(BitConverter.GetBytes((intVal & 0x8000) << 16 | (exp | mant) << 13), 0);
		}

        public static Vector3 FromString(string vec)
		{
			vec = vec.Trim();
			string[] elements = vec.Split(",");

			if (elements.Length != 3) return new Vector3(0,0,0);

			float x, y, z;

			bool xGood = float.TryParse(elements[0], out x);
			bool yGood = float.TryParse(elements[1], out y);
			bool zGood = float.TryParse(elements[2], out z);

			if(xGood && yGood && zGood)
			{
				return new Vector3(x, y, z);
			}

			return new Vector3(0, 0, 0);
        }
		public static string ToString(Vector3 vec)
		{
			return string.Format("{0},{1},{2}", vec.X, vec.Y, vec.Z);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Util
{
    public static class DXT
    {
        /// <summary>
        /// Decompress a single DXT5A pixel (64 bits) to 8 R8_UNORM pixels (128 bits).
        /// DXT5A is an 4bpp alpha-only format from the Xbox 360.
        /// Currently a little wonky. Convert to DXT5 instead.
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        /// Implementation references Crunch2: https://github.com/FrozenStormInteractive/Crunch2
        /// and MSDN: https://learn.microsoft.com/en-us/windows/win32/direct3d10/d3d10-graphics-programming-guide-resources-block-compression#bc2
        private static byte[] Unpack_DXT5A_R8UNORM(ulong pixel)
        {
            uint alpha0 = (byte)((pixel >> 56) & 0xFF);
            uint alpha1 = (byte)((pixel >> 48) & 0xFF);

            uint[] alphas = new uint[8];

            if (alphas[0] > alphas[1])
            {
                alphas[0] = alpha0;
                alphas[1] = alpha1;
                alphas[2] = (6 * alpha0 + 1 * alpha1) / 7;
                alphas[3] = (5 * alpha0 + 2 * alpha1) / 7;
                alphas[4] = (4 * alpha0 + 3 * alpha1) / 7;
                alphas[5] = (3 * alpha0 + 4 * alpha1) / 7;
                alphas[6] = (2 * alpha0 + 5 * alpha1) / 7;
                alphas[7] = (1 * alpha0 + 6 * alpha1) / 7;
            }
            else
            {
                alphas[0] = alpha0;
                alphas[1] = alpha1;
                alphas[2] = (4 * alpha0 + 1 * alpha1) / 5;
                alphas[3] = (3 * alpha0 + 2 * alpha1) / 5;
                alphas[4] = (2 * alpha0 + 3 * alpha1) / 5;
                alphas[5] = (1 * alpha0 + 4 * alpha1) / 5;
                alphas[6] = 0;
                alphas[7] = 255;
            }

            byte[] ret = new byte[16];
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    int offs = ((x * 4 + y) * 3);
                    int a = (int)(pixel >> offs) & 0x7;
                    ret[(x * 4) + y] = (byte)alphas[a];
                }
            }
            return ret;
        }
        private static uint GetDXT5ASelector(ulong pixel, uint x, uint y)
        {
            uint bitOfs = (y * 4 + x) * 3;
            uint byteOfs = bitOfs >> 3;

            byte[] selectors = BitConverter.GetBytes(pixel);

            uint word = selectors[byteOfs];
            if (byteOfs != 5)
                word |= (uint)(selectors[byteOfs + 1] << 8);

            return (word >> (int)(bitOfs & 7)) & 7;
        }

        private static byte[] Convert_DXT5A_DXT5(byte[] dxt5a)
        {
            byte[] bc3 = new byte[dxt5a.Length * 2];
            //convert from DXT5A to DXT5 (aka BC3)
            //basically just add some 0'd color data for the first 8 bytes,
            //then add our 8 alpha bytes.
            for (int i = 0; i < dxt5a.Length; i += 8)
            {
                Array.Copy(dxt5a, i, bc3, i * 2, 8);
            }

            return bc3;
        }
    }
}

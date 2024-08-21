using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using KSoft.DDS;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;

namespace Chef.HW1.Map
{
    public class TerrainVisual
    {
        /// <summary>
        /// The actual width of the terrain that you see. Will be aligned to a multiple of 64.
        /// </summary>
        public int Width
        {
            get
            {
                return (int)Math.Sqrt(Positions.Length);
            }
            set
            {
                if (value % 64 != 0)
                    value += 64 - (value % 64); //round to 64 vertices.

                Vector3[] positions = new Vector3[value * value];
                if (Positions != null)
                {
                    int copy = Math.Min(Positions.Length, positions.Length);
                    Array.Copy(Positions, positions, copy);
                }
                Positions = positions;

                Vector3[] normals = new Vector3[value * value];
                if (Normals != null)
                {
                    int copy = Math.Min(Normals.Length, normals.Length);
                    Array.Copy(Normals, normals, copy);
                }
                Normals = normals;

                float[] alphas = new float[value * value];
                if (Alphas != null)
                {
                    int copy = Math.Min(Alphas.Length, alphas.Length);
                    Array.Copy(Alphas, alphas, copy);
                }
                Alphas = alphas;

                for (int i = 0; i < cMaxTextureLayers; i++)
                {
                    float[] splats = new float[value * value];
                    if (TextureAlphas[i] != null)
                    {
                        int copy = Math.Min(TextureAlphas[i].Length, splats.Length);
                        Array.Copy(TextureAlphas[i], splats, copy);
                    }
                    TextureAlphas[i] = splats;
                }
            }
        }

        public int IndexOf(int x, int y)
        {
            if (x >= Width || y >= Width) return -1;
            return x * Width + y;
        }
        public int IndexOf(Point p)
        {
            return IndexOf(p.X, p.Y);
        }
        public Point CoordOf(int index)
        {
            int x = index / Width;
            int y = index % Width;
            return new Point(x, y);
        }

        public Vector3[] Positions { get; private set; }
        public Vector3[] Normals { get; private set; }
        public float[] Alphas { get; private set; }

        public const int cMaxTextureLayers = 16;
        public string[] Textures { get; private set; } = new string[cMaxTextureLayers];
        public float[][] TextureAlphas { get; private set; } = new float[cMaxTextureLayers][];
    }
}
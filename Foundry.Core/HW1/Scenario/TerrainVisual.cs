using Foundry.HW1.Serialization;
using Foundry.Util;
using Foundry.util;
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

namespace Foundry.HW1.Scenario
{
    public struct TerrainVisualVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
    }
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
                value += value % 64; //round to 64 vertices.
                Vector3[] positions = new Vector3[value * value];
                if (Positions != null)
                {
                    int pcopy = Math.Min(Positions.Length, positions.Length);
                    Array.Copy(Positions, positions, pcopy);
                }
                Positions = positions;

                Vector3[] normals = new Vector3[value * value];
                if (Normals != null)
                {
                    int ncopy = Math.Min(Normals.Length, normals.Length);
                    Array.Copy(Normals, normals, ncopy);
                }
                Normals = normals;
            }
        }

        public int IndexOf(int x, int y)
        {
            return (x * Width) + y;
        }
        public int IndexOf(Point p)
        {
            return (p.X * Width) + p.Y;
        }
        public Point CoordOf(int index)
        {
            int x = index / Width;
            int y = index % Width;
            return new Point(x, y);
        }

        public Vector3[] Positions { get; private set; }
        public Vector3[] Normals { get; private set; }
    }
}
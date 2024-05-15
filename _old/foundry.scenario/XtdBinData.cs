using Foundry.Util;
using SharpDX;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Data.Scenario
{
    public class TerrainData
    {
        public int NumXVertices
        {
            get
            {
                return Vertices.Length;
            }
            set
            {
                Vertices = new Vertex[value, value];
                Triangles = new Triangle[value * value * 2];

                //generate triangles
                for (int x = 0; x < value; x += 2)
                {
                    for (int y = 0; y < value; y += 2)
                    {
                        int row1 = (x) * (value + 1);
                        int row2 = (x + 1) * (value + 1);

                        Triangles[(x * value) + y] = new Triangle()
                        {
                            A = row1 + x,
                            B = row1 + x + 1,
                            C = row2 + x + 1
                        };
                        Triangles[(x * value) + y + 1] = new Triangle()
                        {
                            A = row1 + x,
                            B = row2 + x + 1,
                            C = row2 + x
                        };
                    }
                }
            }
        }

        private Vertex[,] Vertices;
        private Triangle[] Triangles;

        public TerrainData()
        {
            Vertices = new Vertex[0, 0];
            Triangles = new Triangle[0];
        }

        public void SetVertex(int x, int y, Vertex value)
        {
            if (x > 0 && y > 0)
            {
                if (x < NumXVertices && y < NumXVertices)
                {
                    Vertices[x, y] = value;
                }
            }
        }

        private const long XTDHeaderId = 0x1111;
        private const long TerrainChunkId = 0x2222;
        private const long AtlasChunkId = 0x8888;
        private const long AOChunkID = 0xCCCC;
        private const long AlphaChunkID = 0xDDDD;
        private const long TessChunkID = 0xAAAA;
        public static TerrainData LoadFile(ScenarioDirectoryItem scenario)
        {
            TerrainData ret = new TerrainData();

            var ecfChunks = ECF.ReadChunks(scenario.XtdFile.FullPath);

            byte[] xtdHeader = ecfChunks[XTDHeaderId][0];
            int thisNumXVerts = BinaryPrimitives.ReverseEndianness(BitConverter.ToInt32(xtdHeader, 4));
            int thisNumXChunks = BinaryPrimitives.ReverseEndianness(BitConverter.ToInt32(xtdHeader, 8));

            //sets the terrain size.
            ret.NumXVertices = thisNumXVerts;

            byte[] atlas = ecfChunks[AtlasChunkId][0];
            Vector3 posCompMin = new Vector3(
                BitConverter.ToSingle(atlas.Skip(0).Take(4).Reverse().ToArray(), 0),
                BitConverter.ToSingle(atlas.Skip(4).Take(4).Reverse().ToArray(), 0),
                BitConverter.ToSingle(atlas.Skip(8).Take(4).Reverse().ToArray(), 0));
            Vector3 posCompRange = new Vector3(
                BitConverter.ToSingle(atlas.Skip(16).Take(4).Reverse().ToArray(), 0),
                BitConverter.ToSingle(atlas.Skip(20).Take(4).Reverse().ToArray(), 0),
                BitConverter.ToSingle(atlas.Skip(24).Take(4).Reverse().ToArray(), 0));

            const int positionsOffset = 32;
            const uint kBitMask10 = (1 << 10) - 1;
            const float kBitMask10Rcp = 1.0f / kBitMask10;
            for (int i = 0; i < thisNumXVerts * thisNumXVerts; i++)
            {
                uint v = BitConverter.ToUInt32(atlas, (i * 4) + positionsOffset);

                uint x = (v >> 20) & kBitMask10;
                uint y = (v >> 10) & kBitMask10;
                uint z = (v >> 00) & kBitMask10;
                float fx = (x * kBitMask10Rcp * posCompRange.X) - posCompMin.X;
                float fy = (y * kBitMask10Rcp * posCompRange.Y) - posCompMin.Y;
                float fz = (z * kBitMask10Rcp * posCompRange.Z) - posCompMin.Z;

                int row = i / (thisNumXVerts);
                int col = i % (thisNumXVerts);
                //row and col order is intentional based on objects.
                ret.SetVertex(row, col, new Vertex()
                {
                    Position = new Vector3(fx, fy, fz),
                    Normal = new Vector3(0, 0, 0)
                });
            }

            return ret;
        }
    }
}

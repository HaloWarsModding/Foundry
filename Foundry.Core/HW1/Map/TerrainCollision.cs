using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Chef.HW1.Map
{
    public class TerrainVisualAABB
    {
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }
        public int XVertStart { get; set; }
        public int ZVertStart { get; set; }
        public int XVertCount { get; set; }
        public int ZVertCount { get; set; }
    }

    public class TerrainCollisionInfo
    {
        public Vector3 Position { get; set; }
        public int i0 { get; set; }
        public int i1 { get; set; }
        public int i2 { get; set; }
    }

    public static class TerrainCollision
    {
        public static TerrainVisualAABB[] CalcAABBs(TerrainVisual vis)
        {
            const int chunkWidth = 64; //should be a divisor of 64 (visual chunk width).
            int chunkCount = vis.Width / chunkWidth;

            TerrainVisualAABB[] aabbs = new TerrainVisualAABB[chunkCount * chunkCount];

            for (int cx = 0; cx < chunkCount; cx++)
            {
                for (int cy = 0; cy < chunkCount; cy++)
                {
                    int ci = cx * chunkCount + cy;
                    aabbs[ci] = new TerrainVisualAABB()
                    {
                        XVertStart = cx * chunkWidth,
                        ZVertStart = cy * chunkWidth,
                        XVertCount = chunkWidth,
                        ZVertCount = chunkWidth,
                        Min = new Vector3(999999, 999999, 999999),
                        Max = new Vector3(-999999,-999999,-999999),
                    };

                    TerrainVisualAABB aabb = aabbs[ci]; //for easier aliasing.
                    for (int vx = aabb.XVertStart; vx < aabb.XVertStart + chunkWidth + 1; vx++)
                    {
                        for (int vy = aabb.ZVertStart; vy < aabb.ZVertStart + chunkWidth + 1; vy++)
                        {
                            int vi = vis.IndexOf(vx, vy);
                            if (vi != -1)
                            {
                                aabb.Min = Vector3.Min(aabb.Min, vis.Positions[vi]);
                                aabb.Max = Vector3.Max(aabb.Max, vis.Positions[vi]);
                            }
                        }
                    }
                }
            }

            return aabbs;
        }
        public static TerrainCollisionInfo CollidingIndices(TerrainVisualAABB[] aabbs, TerrainVisual vis, Vector3 start, Vector3 end)
        {
            foreach (var aabb in aabbs)
            {
                Vector3 pos;
                if (!CommonCollision.TestRayAABB(aabb.Min, aabb.Max, start, end, out pos)) 
                    continue; //early out if were not near

                //List<TerrainCollisionInfo> ret = new List<TerrainCollisionInfo>();
                for (int vx = aabb.XVertStart; vx < aabb.XVertStart + aabb.XVertCount - 1; vx++)
                {
                    for (int vz = aabb.ZVertStart; vz < aabb.ZVertStart + aabb.ZVertCount - 1; vz++)
                    {
                        int i00 = vis.IndexOf(vx, vz);
                        int i01 = vis.IndexOf(vx, vz + 1);
                        int i02 = vis.IndexOf(vx + 1, vz);
                        Vector3 ci0;
                        if (CommonCollision.TestRayTriangle(
                            Matrix4x4.Identity,
                            vis.Positions[i00],
                            vis.Positions[i01],
                            vis.Positions[i02],
                            start, end, out ci0))
                        {
                            return new TerrainCollisionInfo()
                            {
                                i0 = i00,
                                i1 = i01,
                                i2 = i02,
                                Position = ci0
                            };
                        }

                        int i10 = vis.IndexOf(vx, vz + 1);
                        int i11 = vis.IndexOf(vx + 1, vz);
                        int i12 = vis.IndexOf(vx + 1, vz + 1);
                        Vector3 ci1;
                        if (CommonCollision.TestRayTriangle(
                            Matrix4x4.Identity,
                            vis.Positions[i10],
                            vis.Positions[i11],
                            vis.Positions[i12],
                            start, end, out ci1))
                        {
                            return new TerrainCollisionInfo()
                            {
                                i0 = i10,
                                i1 = i11,
                                i2 = i12,
                                Position = ci1
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}

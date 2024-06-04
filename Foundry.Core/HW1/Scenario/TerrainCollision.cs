using Foundry.Core.HW1;
using Foundry.HW1.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.UI.WinForms
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

    public static class TerrainCollision
    {
        public static TerrainVisualAABB[] CalcAABBs(TerrainVisual vis)
        {
            const int chunkWidth = 64;
            int chunkCount = vis.Width / chunkWidth;

            TerrainVisualAABB[] aabbs = new TerrainVisualAABB[chunkCount * chunkCount];

            for (int cx = 0; cx < chunkCount; cx++)
            {
                for (int  cy = 0; cy < chunkCount; cy++)
                {
                    int ci = (cx * chunkCount) + cy;
                    aabbs[ci] = new TerrainVisualAABB()
                    {
                        XVertStart = cx * chunkWidth,
                        ZVertStart = cy * chunkWidth,
                        XVertCount = chunkWidth,
                        ZVertCount = chunkWidth,
                        Min = new Vector3(),
                        Max = new Vector3(),
                    };

                    TerrainVisualAABB aabb = aabbs[ci]; //for easier aliasing.
                    for (int vx = aabb.XVertStart; vx < aabb.XVertStart + chunkWidth; vx++)
                    {
                        for (int vy = aabb.ZVertStart; vy < aabb.ZVertStart + chunkWidth; vy++)
                        {
                            int vi = vis.IndexOf(vx, vy);
                            aabb.Min = Vector3.Min(aabb.Min, vis.Positions[vi]);
                            aabb.Max = Vector3.Max(aabb.Max, vis.Positions[vi]);
                        }
                    }
                }
            }

            return aabbs;
        }
        public static TerrainVisualAABB FirstRayCollision(TerrainVisualAABB[] aabbs, Vector3 start, Vector3 end)
        {
            foreach (var aabb in aabbs)
            {
                //ray-aabb intersection impl borrowed from: https://www.gamedev.net/forums/topic/338987-aabb---line-segment-intersection-test/
                Vector3 d = (end - start) * 0.5f;
                Vector3 e = (aabb.Max - aabb.Min) * 0.5f;
                Vector3 c = start + d - (aabb.Min + aabb.Max) * 0.5f;
                Vector3 ad = Vector3.Abs(d);

                if (Math.Abs(c[0]) > e[0] + ad[0]) continue;
                if (Math.Abs(c[1]) > e[1] + ad[1]) continue;
                if (Math.Abs(c[2]) > e[2] + ad[2]) continue;
                if (Math.Abs(d[1] * c[2] - d[2] * c[1]) > e[1] * ad[2] + e[2] * ad[1] + float.Epsilon) continue;
                if (Math.Abs(d[2] * c[0] - d[0] * c[2]) > e[2] * ad[0] + e[0] * ad[2] + float.Epsilon) continue;
                if (Math.Abs(d[0] * c[1] - d[1] * c[0]) > e[0] * ad[1] + e[1] * ad[0] + float.Epsilon) continue;
                return aabb;
            }
            return null;
        }
        public static int[] CollidingIndices(TerrainVisualAABB aabb, TerrainVisual vis, Vector3 start, Vector3 end)
        {
            List<int> ret = new List<int>();
            for(int vx = aabb.XVertStart; vx < aabb.XVertStart + aabb.XVertCount - 1; vx++)
            {
                for (int vz = aabb.ZVertStart; vz < aabb.ZVertStart + aabb.ZVertCount - 1; vz++)
                {
                    int i00 = vis.IndexOf(vx, vz);
                    int i01 = vis.IndexOf(vx, vz + 1);
                    int i02 = vis.IndexOf(vx + 1, vz);

                    int i10 = vis.IndexOf(vx, vz + 1);
                    int i11 = vis.IndexOf(vx + 1, vz);
                    int i12 = vis.IndexOf(vx + 1, vz + 1);

                    if (CommonCollision.IntersectsAt(
                        vis.Positions[i00],
                        vis.Positions[i01],
                        vis.Positions[i02],
                        start, end) != float.NegativeInfinity)
                    {
                        ret.Add(i00);
                        ret.Add(i01);
                        ret.Add(i02);
                    }

                    if (CommonCollision.IntersectsAt(
                        vis.Positions[i10],
                        vis.Positions[i11],
                        vis.Positions[i12],
                        start, end) != float.NegativeInfinity)
                    {
                        ret.Add(i10);
                        ret.Add(i11);
                        ret.Add(i12);
                    }
                }
            }
            return ret.ToArray();
        }
    }
}

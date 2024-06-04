using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Core.HW1
{
    public static class CommonCollision
    {
        public static float IntersectsAt(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 start, Vector3 end)
        {
            Vector3 dir = end - start;
            Vector3 v0v1 = Vector3.Subtract(v1, v0);
            Vector3 v0v2 = Vector3.Subtract(v2, v0);

            Vector3 pvec = Vector3.Cross(dir, v0v2);

            float det = Vector3.Dot(v0v1, pvec);

            if (det < 0.000001)
                return float.NegativeInfinity;

            float invDet = 1.0f / det;

            Vector3 tvec = Vector3.Subtract(start, v0);

            float u = Vector3.Dot(tvec, pvec) * invDet;

            if (u < 0 || u > 1)
                return float.NegativeInfinity;

            Vector3 qvec = Vector3.Cross(tvec, v0v1);


            float v = Vector3.Dot(dir, qvec) * invDet;

            if (v < 0 || u + v > 1)
                return float.NegativeInfinity;

            return Vector3.Dot(v0v2, qvec) * invDet;
        }
    }
}

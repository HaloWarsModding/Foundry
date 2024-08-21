using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Chef.HW1
{
    public class CollisionTreeAABB
    {
        public Vector3 Position { get; set; }
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }
    }
    public class CollisionTreeNode
    {
        public CollisionTreeAABB Bounds { get; set; } = new CollisionTreeAABB();
        public List<CollisionTreeNode> Children { get; set; } = new List<CollisionTreeNode>();
        public object Tag { get; set; } = null;
    }

    public static class CommonCollision
    {
        public static bool TestRayTriangle(Matrix4x4 transform, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 start, Vector3 end, out Vector3 hit)
        {
            hit = Vector3.Zero;

            v0 = Vector3.Transform(v0, transform);
            v1 = Vector3.Transform(v1, transform);
            v2 = Vector3.Transform(v2, transform);

            Vector3 rayDirection = Vector3.Normalize(end - start);

            // Find vectors for two edges sharing v0
            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;

            // Begin calculating determinant - also used to calculate u parameter
            Vector3 pvec = Vector3.Cross(rayDirection, edge2);

            // If determinant is near zero, ray lies in plane of triangle
            float det = Vector3.Dot(edge1, pvec);

            if (det > -float.Epsilon && det < float.Epsilon)
                return false;

            float invDet = 1.0f / det;

            // Calculate distance from v0 to ray origin
            Vector3 tvec = start - v0;

            // Calculate u parameter and test bound
            float u = Vector3.Dot(tvec, pvec) * invDet;
            if (u < 0.0f || u > 1.0f)
                return false;

            // Prepare to test v parameter
            Vector3 qvec = Vector3.Cross(tvec, edge1);

            // Calculate v parameter and test bound
            float v = Vector3.Dot(rayDirection, qvec) * invDet;
            if (v < 0.0f || u + v > 1.0f)
                return false;

            // Calculate t, ray intersects triangle
            float t = Vector3.Dot(edge2, qvec) * invDet;

            if (t > float.Epsilon) // ray intersection
            {
                hit = start + t * rayDirection;
                return true;
            }

            return false;
        }
        public static bool TestRayTriangles(Matrix4x4 transform, Vector3[] vertices, ushort[] indices, Vector3 start, Vector3 end, out Vector3 hit)
        {
            hit = Vector3.Zero;
            for(int i = 0; i < indices.Length; i+= 3)
            {
                if (TestRayTriangle(
                    transform,
                    vertices[indices[i + 0]],
                    vertices[indices[i + 1]],
                    vertices[indices[i + 2]],
                    start,
                    end,
                    out hit))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool TestRayPlane(Matrix4x4 transform, Vector3 start, Vector3 end, Vector3 plane_pos, Vector3 plane_dir, out Vector3 hit)
        {
            hit = Vector3.Zero;

            Vector3 lineDir = end - start;
            Vector3 planeNormal = Vector3.Normalize(plane_dir);
            float denominator = Vector3.Dot(planeNormal, lineDir);

            // Check if the line is parallel to the plane
            if (Math.Abs(denominator) < float.Epsilon)
            {
                return false; // No intersection, the line is parallel to the plane
            }

            float t = Vector3.Dot(plane_pos - start, planeNormal) / denominator;

            // Check if the intersection point is within the line segment
            if (t < 0 || t > 1)
            {
                return false; // No intersection within the line segment
            }

            hit = start + t * lineDir;
            return true;
        }
        public static bool TestRayPlane(Matrix4x4 transform, Vector3 start, Vector3 end, Vector3 plane_pos, Vector3 plane_dir, float ext_x, float ext_y, out Vector3 hit)
        {
            hit = Vector3.Zero;

            Vector3 lineDir = end - start;
            Vector3 planeNormal = Vector3.Normalize(plane_dir);
            float denominator = Vector3.Dot(planeNormal, lineDir);

            // Check if the line is parallel to the plane
            if (Math.Abs(denominator) < float.Epsilon)
            {
                return false; // No intersection, the line is parallel to the plane
            }

            float t = Vector3.Dot(plane_pos - start, planeNormal) / denominator;

            // Check if the intersection point is within the line segment
            if (t < 0 || t > 1)
            {
                return false; // No intersection within the line segment
            }

            hit = start + t * lineDir;

            // Define the plane's local X and Y axes
            Vector3 planeXAxis, planeYAxis;

            if (Math.Abs(planeNormal.X) > Math.Abs(planeNormal.Y))
            {
                planeXAxis = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, planeNormal));
            }
            else
            {
                planeXAxis = Vector3.Normalize(Vector3.Cross(Vector3.UnitX, planeNormal));
            }

            planeYAxis = Vector3.Cross(planeNormal, planeXAxis);

            // Calculate the local coordinates of the intersection point
            Vector3 localIntersection = hit - plane_pos;
            float localX = Vector3.Dot(localIntersection, planeXAxis);
            float localY = Vector3.Dot(localIntersection, planeYAxis);

            // Check if the intersection point is within the extents of the plane section
            if (Math.Abs(localX) <= ext_x / 2 && Math.Abs(localY) <= ext_y / 2)
            {
                return true; // Intersection is within the plane section
            }

            return false; // Intersection is outside the plane section
        }
        public static bool TestRayAABB(Vector3 min, Vector3 max, Vector3 start, Vector3 end, out Vector3 hit)
        {
            hit = Vector3.Zero;

            Vector3 rayDirection = Vector3.Normalize(end - start);
            Vector3 invDir = new Vector3(1 / rayDirection.X, 1 / rayDirection.Y, 1 / rayDirection.Z);

            float t1 = (min.X - start.X) * invDir.X;
            float t2 = (max.X - start.X) * invDir.X;
            float t3 = (min.Y - start.Y) * invDir.Y;
            float t4 = (max.Y - start.Y) * invDir.Y;
            float t5 = (min.Z - start.Z) * invDir.Z;
            float t6 = (max.Z - start.Z) * invDir.Z;

            float tmin = Math.Max(Math.Max(Math.Min(t1, t2), Math.Min(t3, t4)), Math.Min(t5, t6));
            float tmax = Math.Min(Math.Min(Math.Max(t1, t2), Math.Max(t3, t4)), Math.Max(t5, t6));

            // If tmax < 0, ray is intersecting AABB, but the whole AABB is behind us
            if (tmax < 0)
            {
                return false;
            }

            // If tmin > tmax, ray doesn't intersect AABB
            if (tmin > tmax)
            {
                return false;
            }

            // tmin is the intersection distance from start
            if (tmin < 0) // If tmin < 0, the intersection point is before the start of the ray
            {
                hit = start + tmax * rayDirection;
            }
            else
            {
                hit = start + tmin * rayDirection;
            }

            return true;
        }
        public static bool TestRayOBB(Vector3 start, Vector3 end, Vector3 box_pos, Vector3 box_dir, Vector3 box_exts, out Vector3 hit)
        {
            //prevent matrix degenerecy.
            Vector3 forward = box_dir;
            Vector3 up =
                forward == Vector3.UnitY || forward == -Vector3.UnitY 
                ? Vector3.UnitZ 
                : Vector3.UnitY;

            // Convert the ray from world space to local space of the OBB
            Matrix4x4 transform = Matrix4x4.CreateWorld(box_pos, -forward, up);
            Matrix4x4 transformInv;
            Matrix4x4.Invert(transform, out transformInv);
            Vector3 localStart = Vector3.Transform(start, transformInv);
            Vector3 localEnd = Vector3.Transform(end, transformInv);


            // Test the ray against the AABB in the OBB's local space
            Vector3 localMin = -box_exts;
            Vector3 localMax = box_exts;

            if (TestRayAABB(localMin, localMax, localStart, localEnd, out Vector3 localHit))
            {
                // Transform the hit point back to world space
                hit = Vector3.Transform(localHit, transform);
                return true;
            }
            else
            {
                hit = Vector3.Zero;
                return false;
            }
        }
        public static bool TestRayLine(Vector3 start, Vector3 end, Vector3 line_start, Vector3 line_end, float radius, out Vector3 hit)
        {
            hit = Vector3.Zero;

            Vector3 rayDir = Vector3.Normalize(end - start);
            Vector3 lineDir = line_end - line_start;
            Vector3 lineToRayStart = start - line_start;

            float lineLengthSquared = lineDir.LengthSquared();
            if (lineLengthSquared == 0)
            {
                return false; // The line segment is just a point
            }

            float t1 = Vector3.Dot(lineDir, lineToRayStart);
            float t2 = Vector3.Dot(lineDir, rayDir);
            float t3 = Vector3.Dot(lineDir, lineDir);

            float s = (t1 * t2 - Vector3.Dot(lineToRayStart, rayDir) * t3) / (t3 - t2 * t2);

            // Clamp s to [0, 1] to stay on the segment
            s = Math.Clamp(s, 0, 1);

            // Find the closest point on the line segment to the ray
            Vector3 closestPointOnLine = line_start + s * lineDir;

            // Find the vector from the ray origin to the closest point on the line
            Vector3 rayToPoint = closestPointOnLine - start;

            // Project this vector onto the ray direction
            float t = Vector3.Dot(rayToPoint, rayDir);

            // If t is negative, the closest point on the ray is before the start point
            if (t < 0)
            {
                return false;
            }

            // Find the closest point on the ray
            Vector3 closestPointOnRay = start + t * rayDir;

            // Check if the distance between the closest points is within the radius
            float distanceSquared = (closestPointOnRay - closestPointOnLine).LengthSquared();

            if (distanceSquared <= radius * radius)
            {
                hit = closestPointOnRay;
                return true;
            }

            return false;
        }

        public static Vector3 ClosestPointRayRay(Vector3 start, Vector3 end, Vector3 b_start, Vector3 b_end)
        {
            // Normalize the ray direction
            Vector3 dir = Vector3.Normalize(start-end);

            // Direction vector of the line
            Vector3 lineDirection = b_end - b_start;
            lineDirection = Vector3.Normalize(lineDirection);

            // Vector from line start to ray origin
            Vector3 lineToRay = start - b_start;

            // Calculate the projection of lineToRay onto the line direction
            float projectionLength = Vector3.Dot(lineToRay, lineDirection);
            Vector3 projection = lineDirection * projectionLength;

            // Closest point on the line
            Vector3 closestPointOnLine = b_start + projection;

            // Calculate the vector from ray origin to the closest point on the line
            Vector3 rayToLine = closestPointOnLine - start;

            // Projection of rayToLine onto the ray direction
            float t = Vector3.Dot(rayToLine, dir);

            // Closest point on the ray to the line
            Vector3 closestPointOnRay = start + dir * t;

            return closestPointOnRay;
        }
        public static Vector3 ClosestPointRayLine(Vector3 start, Vector3 end, Vector3 line_pos, Vector3 line_nrm)
        {
            // Calculate the ray direction
            Vector3 rayDirection = Vector3.Normalize(end - start);

            // Calculate the vector from the line position to the ray start
            Vector3 lineToRay = start - line_pos;

            // Calculate the projection of lineToRay onto the normal vector
            float projectionLength = Vector3.Dot(lineToRay, line_nrm);
            Vector3 projection = line_nrm * projectionLength;

            // Calculate the point on the plane closest to the ray start
            Vector3 closestPointOnPlane = start - projection;

            // Calculate the vector from ray start to the closest point on the plane
            Vector3 rayToPlane = closestPointOnPlane - start;

            // Projection of rayToPlane onto the ray direction
            float t = Vector3.Dot(rayToPlane, rayDirection);

            // Closest point on the ray to the plane
            Vector3 closestPointOnRay = start + rayDirection * t;

            return closestPointOnRay;
        }

        public static CollisionTreeNode CreateTree(IEnumerable<CollisionTreeAABB> aabbs, int divs)
        {
            Vector3 absMin = new Vector3(99999, 99999, 99999);
            Vector3 absMax = new Vector3(-99999, -99999, -99999);

            foreach(var aabb in aabbs)
            {
                absMin = Vector3.Min(absMin, aabb.Min + aabb.Position);
                absMax = Vector3.Max(absMax, aabb.Max + aabb.Position);
            }

            CollisionTreeNode root = new CollisionTreeNode()
            {
                Bounds = new CollisionTreeAABB() { Min = absMin, Max = absMax },
            };

            List<CollisionTreeNode> leaves = [root];

            //divide up our leaf nodes.
            for(int i = 0; i < divs; i++)
            {
                List<CollisionTreeNode> newLeaves = new List<CollisionTreeNode>();
                foreach(var node in leaves)
                {
                    Vector3 min = node.Bounds.Min;
                    Vector3 max = node.Bounds.Max;
                    Vector3 center = Vector3.Lerp(node.Bounds.Min, node.Bounds.Max, 0.5f);

                    CollisionTreeAABB[] boxes = [
                        new CollisionTreeAABB() { Min = min, Max = center },
                        new CollisionTreeAABB() { Min = new Vector3(center.X, min.Y, min.Z), Max = new Vector3(max.X, center.Y, center.Z) },
                        new CollisionTreeAABB() { Min = new Vector3(min.X, center.Y, min.Z), Max = new Vector3(center.X, max.Y, center.Z) },
                        new CollisionTreeAABB() { Min = new Vector3(min.X, min.Y, center.Z), Max = new Vector3(center.X, center.Y, max.Z) },
                        new CollisionTreeAABB() { Min = new Vector3(center.X, center.Y, min.Z), Max = new Vector3(max.X, max.Y, center.Z) },
                        new CollisionTreeAABB() { Min = new Vector3(center.X, min.Y, center.Z), Max = new Vector3(max.X, center.Y, max.Z) },
                        new CollisionTreeAABB() { Min = new Vector3(min.X, center.Y, center.Z), Max = new Vector3(center.X, max.Y, max.Z) },
                        new CollisionTreeAABB() { Min = center, Max = max },
                    ];

                    foreach(var box in boxes)
                    {
                        var child = new CollisionTreeNode() { Bounds = box };
                        node.Children.Add(child);
                        newLeaves.Add(child);
                    }
                }
                leaves = newLeaves;
            }

            foreach (var aabb in aabbs)
            {
                foreach(var leaf in leaves)
                {
                    Vector3 min = aabb.Position + aabb.Min;
                    Vector3 max = aabb.Position + aabb.Max;

                    //is this aabb inside this leaf?
                    if ( min.X >= leaf.Bounds.Min.X && max.X <= leaf.Bounds.Max.X &&
                        min.Y >= leaf.Bounds.Min.Y && max.Y <= leaf.Bounds.Max.Y &&
                        min.Z >= leaf.Bounds.Min.Z && max.Z <= leaf.Bounds.Max.Z)
                    {
                        leaf.Children.Add(new CollisionTreeNode()
                        {
                            Bounds = aabb,
                        });
                    }
                }
            }

            return root;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="plane_pos"></param>
        /// <param name="plane_dir"></param>
        /// <returns></returns>
        /// ChatGPT prompt: "write me c# using system.numerics to project a vector3 point onto a plane given plane_pos and plane_dir".
        public static Vector3 ProjectPointOnPlane(Vector3 point, Vector3 plane_pos, Vector3 plane_dir)
        {
            // Normalize the plane direction vector
            Vector3 planeNormal = Vector3.Normalize(plane_dir);

            // Calculate the vector from the point to the plane position
            Vector3 pointToPlane = point - plane_pos;

            // Calculate the distance from the point to the plane along the plane normal
            float distance = Vector3.Dot(pointToPlane, planeNormal);

            // Project the point onto the plane
            Vector3 projectedPoint = point - distance * planeNormal;

            return projectedPoint;
        }
    }
}

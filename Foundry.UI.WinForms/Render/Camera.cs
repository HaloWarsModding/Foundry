using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Chef.Win.Render
{
    public class Camera //TODO: this is an orbit cam only right now
    {
        public float Distance { get; set; } = 100;
        public float Yaw { get; set; } = 0; //degrees
        public float Pitch { get; set; } = 0; //degrees
        public Vector3 Target { get; set; } = new Vector3(0, 15, 0);
        public float Width { get; set; }
        public float Height { get; set; }
        public float Ratio { get { return Width / Height; } }

        public Vector3 Right
        {
            get
            {
                return Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
            }
        }
        public Vector3 Forward
        {
            get
            {
                float rad = (float)Math.PI / 180;
                float xzlen = (float)Math.Cos(Pitch * rad);
                return Vector3.Normalize(new Vector3(
                        xzlen * (float)Math.Cos(Yaw * rad),
                        (float)Math.Sin(Pitch * rad),
                        xzlen * (float)Math.Sin(-Yaw * rad)
                    ));
            }
        }
        public Vector3 Up
        {
            get
            {
                return Vector3.Normalize(Vector3.Cross(Forward, Right));
            }
        }
        public Vector3 Pos
        {
            get
            {
                return Target + Vector3.Normalize(Forward) * -Distance;
            }
        }

        public Matrix4x4 ViewMatrix
        {
            get
            {
                return Matrix4x4.CreateLookAtLeftHanded(Pos, Target, Vector3.UnitY);
            }
        }
        public Matrix4x4 ProjectionMatrix
        {
            get
            {
                return Matrix4x4.CreatePerspectiveFieldOfViewLeftHanded(1.57f, Ratio, .01f, 10000.0f);
            }
        }

        public Vector3 ScreenPointToWorldPos(float screenX, float screenY)
        {
            float clipSpaceX = screenX / Width * 2 - 1;
            float clipSpaceY = 1 - screenY / Height * 2;
            Vector3 clipSpaceCoord = new Vector3(clipSpaceX, clipSpaceY, 0);

            Matrix4x4 invProj, invView;
            bool s1 = Matrix4x4.Invert(ProjectionMatrix, out invProj);
            bool s2 = Matrix4x4.Invert(ViewMatrix, out invView);

            Vector3 viewSpaceCoord = Vector3.Transform(clipSpaceCoord, invProj);
            Vector3 worldSpaceCoord = Vector3.Transform(viewSpaceCoord, invView);

            return worldSpaceCoord;
        }
        public Vector3 ScreenPointToWorldDir(float screenX, float screenY)
        {
            return ScreenPointToWorldPos(screenX, screenY) - Pos;
        }

        public void Move(float rotDegY, float rotDegZ, float distance, float panScreenX, float panScreenY)
        {
            Yaw += rotDegY;
            while (Yaw > 360) Yaw -= 360;
            while (Yaw < 0) Yaw += 360;

            Pitch += rotDegZ;
            Pitch = Math.Clamp(Pitch, -89.5f, 89.5f);

            float panFactor = Distance / 100;
            Target += Right * panScreenX / panFactor;
            Target += Up * panScreenY / panFactor;

            Distance += distance;
            Distance = Math.Clamp(Distance, 1, 10000);
        }
    }
}

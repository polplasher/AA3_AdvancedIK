using System;

namespace Utility
{
    public struct Quaternion
    {
        // Fields
        public float x, y, z, w;

        // Constructor
        public Quaternion(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }

        // Predefined Quaternions
        public static Quaternion Identity => new(0, 0, 0, 1);

        #region Operators

        // Arithmetic operators

        // Hamilton (a * b)
        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            return new Quaternion(
                a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
                a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
                a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w,
                a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z
            );
        }

        // Cast operators
        public static explicit operator Quaternion(UnityEngine.Quaternion v) => new(v.x, v.y, v.z, v.w);
        public static explicit operator UnityEngine.Quaternion(Quaternion v) => new(v.x, v.y, v.z, v.w);

        #endregion

        #region Functions

        public Quaternion Conjugate() => new(-x, -y, -z, w);

        public Quaternion Normalized()
        {
            float n = (float)Math.Sqrt(x * x + y * y + z * z + w * w);
            if (n < 1e-8f) return Identity;
            return new Quaternion(x / n, y / n, z / n, w / n);
        }

        // q * (0,v) * q*
        public Vector3 Rotate(Vector3 v)
        {
            Quaternion qv = new(v.x, v.y, v.z, 0f);
            Quaternion res = this * qv * Conjugate();
            return new Vector3(res.x, res.y, res.z);
        }

        public static Quaternion AxisAngle(Vector3 axis, float angleRad)
        {
            float m = axis.Magnitude();
            if (m < 1e-8f) return Identity;
            float nx = axis.x / m, ny = axis.y / m, nz = axis.z / m;
            float half = angleRad * 0.5f;
            float s = (float)Math.Sin(half);
            float c = (float)Math.Cos(half);
            return new Quaternion(nx * s, ny * s, nz * s, c);
        }

        public static Quaternion Inverse(Quaternion q)
        {
            return q.Conjugate().Normalized();
        }

        public static Quaternion FromYawPitchRoll(float yaw, float pitch, float roll)
        {
            Quaternion qYaw   = AxisAngle(Vector3.Up,    yaw);
            Quaternion qPitch = AxisAngle(Vector3.Right, pitch);
            Quaternion qRoll  = AxisAngle(Vector3.Forward, roll);
            return qYaw * qPitch * qRoll;
        }

        #endregion
    }
}
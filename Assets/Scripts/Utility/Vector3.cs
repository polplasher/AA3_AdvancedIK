using UnityEngine;

namespace Utility
{
    public struct Vector3
    {
        // Fields
        public float x, y, z;

        // Constructor
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        #region Operators

        // Arithmetic operators
        public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vector3 operator -(Vector3 a) => new(-a.x, -a.y, -a.z);
        public static Vector3 operator *(Vector3 a, float s) => new(a.x * s, a.y * s, a.z * s);
        public static Vector3 operator *(float s, Vector3 a) => a * s;
        public static Vector3 operator /(Vector3 a, float s) => new(a.x / s, a.y / s, a.z / s);

        // Tuple helpers (optional, useful in pure C#)
        public static implicit operator Vector3((float x, float y, float z) t) => new(t.x, t.y, t.z);
        public static implicit operator (float x, float y, float z)(Vector3 v) => (v.x, v.y, v.z);

        #endregion

        #region Predefined Vectors

        public static Vector3 Zero => new(0, 0, 0);
        public static Vector3 One => new(1, 1, 1);
        public static Vector3 Up => new(0, 1, 0);
        public static Vector3 Down => new(0, -1, 0);
        public static Vector3 Left => new(-1, 0, 0);
        public static Vector3 Right => new(1, 0, 0);
        public static Vector3 Forward => new(0, 0, 1);
        public static Vector3 Backward => new(0, 0, -1);

        #endregion

        #region Functions

        public static float Dot(Vector3 a, Vector3 b) => a.x * b.x + a.y * b.y + a.z * b.z;

        public static Vector3 Cross(Vector3 a, Vector3 b) => new(
            a.y * b.z - a.z * b.y,
            a.z * b.x - a.x * b.z,
            a.x * b.y - a.y * b.x
        );

        public float SqrMagnitude() => x * x + y * y + z * z;

        public float Magnitude() => (float)System.Math.Sqrt(SqrMagnitude());

        public Vector3 Normalized()
        {
            float m = Magnitude();
            return m > 1e-8f ? this / m : Zero;
        }

        public static float Distance(Vector3 a, Vector3 b) => (a - b).Magnitude();

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t) => a + (b - a) * t;

        public override string ToString() => $"({x:F3},{y:F3},{z:F3})";

        public override int GetHashCode() => (x, y, z).GetHashCode();

        public override bool Equals(object o) =>
            o is Vector3 v && (this - v).SqrMagnitude() < 1e-10f;

        public static bool operator ==(Vector3 a, Vector3 b) => (a - b).SqrMagnitude() < 1e-10f;
        public static bool operator !=(Vector3 a, Vector3 b) => !(a == b);

        // Conversion from UnityEngine.Vector3 to Utility.Vector3
        public static implicit operator Vector3(UnityEngine.Vector3 v)
            => new Vector3(v.x, v.y, v.z);


        // Conversion from Utility.Vector3 to UnityEngine.Vector3
        public static implicit operator UnityEngine.Vector3(Vector3 v)
            => new UnityEngine.Vector3(v.x, v.y, v.z);

        #endregion
    }
}

namespace Utility
{
    // Lightweight Vector2 replacement for projects that prefer a minimal dependency.
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        // Unity-ish aliases (so porting is mostly search/replace)
        public static Vector2 Zero => new(0f, 0f);
        public static Vector2 One => new(1f, 1f);
        public static Vector2 Up => new(0f, 1f);
        public static Vector2 Right => new(1f, 0f);

        public static Vector2 zero => Zero;
        public static Vector2 one => One;
        public static Vector2 up => Up;
        public static Vector2 right => Right;

        public float sqrMagnitude => x * x + y * y;
        public float magnitude => MathLite.Sqrt(sqrMagnitude);

        public Vector2 normalized
        {
            get
            {
                float m = magnitude;
                if (m > 1e-6f) return this / m;
                return Zero;
            }
        }

        public void Normalize()
        {
            float m = magnitude;
            if (m > 1e-6f)
            {
                x /= m; y /= m;
            }
            else { x = 0f; y = 0f; }
        }

        public static float Dot(Vector2 a, Vector2 b) => a.x * b.x + a.y * b.y;

        public static float Distance(Vector2 a, Vector2 b)
        {
            return MathLite.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
        }

        public static Vector2 ClampMagnitude(Vector2 v, float maxLength)
        {
            float sq = v.sqrMagnitude;
            float maxSq = maxLength * maxLength;
            if (sq > maxSq && sq > 1e-12f)
            {
                float m = MathLite.Sqrt(sq);
                return v / m * maxLength;
            }
            return v;
        }

        // Return angle in radians from +X to the vector (like atan2)
        public float AngleRad()
        {
            return MathLite.Atan2(y, x);
        }

        // Angle in degrees
        public float AngleDeg()
        {
            return AngleRad() * MathLite.Rad2Deg;
        }

        // Operators
        public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.x + b.x, a.y + b.y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.x - b.x, a.y - b.y);
        public static Vector2 operator -(Vector2 a) => new(-a.x, -a.y);
        public static Vector2 operator *(Vector2 a, float s) => new(a.x * s, a.y * s);
        public static Vector2 operator *(float s, Vector2 a) => a * s;
        public static Vector2 operator /(Vector2 a, float s) => new(a.x / s, a.y / s);

        public override string ToString() => "(" + x + ", " + y + ")";
    }
}
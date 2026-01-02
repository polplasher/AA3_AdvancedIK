namespace Utility
{
    public static class MathLite
    {
        public const float PI = 3.14159265358979323846f;

        // Match Unity-style constants for convenient porting
        public const float Deg2Rad = PI / 180f;
        public const float Rad2Deg = 180f / PI;

        public static float Sin(float a) => (float)System.Math.Sin(a);
        public static float Cos(float a) => (float)System.Math.Cos(a);
        public static float Acos(float a) => (float)System.Math.Acos(a);
        public static float Sqrt(float a) => (float)System.Math.Sqrt(a);
        public static float Abs(float a) => System.Math.Abs(a);

        public static float Max(float a, float b) => a > b ? a : b;
        public static float Min(float a, float b) => a < b ? a : b;
        public static int Max(int a, int b) => a > b ? a : b;
        public static int Min(int a, int b) => a < b ? a : b;

        public static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            return v > max ? max : v;
        }

        public static int Clamp(int v, int min, int max)
        {
            if (v < min) return min;
            return v > max ? max : v;
        }

        public static float Clamp01(float v) => Clamp(v, 0f, 1f);

        /// <summary>
        /// Unity-like rounding: ties go away from zero (unlike default System.Math.Round).
        /// </summary>
        public static int RoundToInt(float v)
        {
            return v >= 0f
                ? (int)System.Math.Floor(v + 0.5f)
                : (int)System.Math.Ceiling(v - 0.5f);
        }

        public static float Atan2(float y, float x) => (float)System.Math.Atan2(y, x);

        // Rotate point around pivot by radians (counter-clockwise positive)
        public static Vector2 RotateAround(Vector2 point, Vector2 pivot, float radians)
        {
            float s = Sin(radians);
            float c = Cos(radians);
            Vector2 p = point - pivot;
            Vector2 pr = new(p.x * c - p.y * s, p.x * s + p.y * c);
            return new Vector2(pivot.x + pr.x, pivot.y + pr.y);
        }

        // Signed angle in radians between two 2D vectors (from -> to), range [-PI, PI]
        public static float SignedAngleRad(Vector2 from, Vector2 to)
        {
            float magFrom = Sqrt(from.x * from.x + from.y * from.y);
            float magTo = Sqrt(to.x * to.x + to.y * to.y);
            const float eps = 1e-6f;
            if (magFrom < eps || magTo < eps)
                return 0f;

            Vector2 fn = new(from.x / magFrom, from.y / magFrom);
            Vector2 tn = new(to.x / magTo, to.y / magTo);

            float cross = fn.x * tn.y - fn.y * tn.x;
            float dot = fn.x * tn.x + fn.y * tn.y;
            return Atan2(cross, dot);
        }
    }
}
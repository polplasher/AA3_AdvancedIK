namespace Utility
{
    public static class MathLite
    {
        public const float PI = 3.14159265358979323846f;
        public static float Sin(float a) => (float)System.Math.Sin(a);
        public static float Cos(float a) => (float)System.Math.Cos(a);
        public static float Sqrt(float a) => (float)System.Math.Sqrt(a);
        public static float Abs(float a) => System.Math.Abs(a);

        public static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            return v > max ? max : v;
        }

        public static float Deg2Rad(float deg) => deg * (PI / 180f);
        public static float Rad2Deg(float rad) => rad * (180f / PI);
        public static float Atan2(float y, float x) => (float)System.Math.Atan2(y, x);
    }
}
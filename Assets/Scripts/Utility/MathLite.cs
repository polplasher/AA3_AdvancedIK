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
        { if (v < min) return min; if (v > max) return max; return v; }
        public static float Deg2Rad(float deg) => deg * (PI / 180f);
        public static float Rad2Deg(float rad) => rad * (180f / PI);
        public static float Atan2(float y, float x) => (float)System.Math.Atan2(y, x);

        public static float SmoothDamp(
            float current,
            float target,
            ref float currentVelocity,
            float smoothTime,
            float maxSpeed,
            float deltaTime)
        {
            if (smoothTime < 1e-4f) smoothTime = 1e-4f;

            float omega = 2f / smoothTime;
            float x = omega * deltaTime;
            float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);

            float change = current - target;
            float originalTo = target;

            float maxChange = maxSpeed * smoothTime;
            change = Clamp(change, -maxChange, maxChange);
            target = current - change;

            float temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;

            float output = target + (change + temp) * exp;

            // Avoid overshoot
            if ((originalTo - current > 0f) == (output > originalTo))
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }

            return output;
        }
    }
}

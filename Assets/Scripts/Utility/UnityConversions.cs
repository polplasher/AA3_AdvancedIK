namespace Utility
{
    public static class UnityConversions
    {
        public static Utility.Vector2 ToUtility(this UnityEngine.Vector2 v) => new(v.x, v.y);
        public static UnityEngine.Vector2 ToUnity(this Utility.Vector2 v) => new(v.x, v.y);
    }
}

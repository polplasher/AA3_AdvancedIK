using UnityEngine;

namespace Utility
{
    public static class UnityConversions
    {
        public static Utility.Vector2 ToUtility(this Vector2 v)
            => new Utility.Vector2(v.x, v.y);

        public static Vector2 ToUnity(this Utility.Vector2 v)
            => new Vector2(v.x, v.y);
    }
}

using UnityEngine;

using UVec2 = UnityEngine.Vector2;
using UVec3 = UnityEngine.Vector3;
using Math = Utility.MathLite;

namespace Lasers
{
    public class OscillatingLaser2D : MonoBehaviour
    {
        [Header("Laser")]
        [SerializeField] private float maxDistance = 20f;
        [SerializeField] private LayerMask hitMask;       
        [SerializeField] private bool hitTriggers = false;
        [SerializeField] private float lineZ = 0f;         
        [SerializeField] private string armTag = "PlayerArm"; 

        [Header("Oscillation (degrees)")]
        [SerializeField] private float baseAngleDeg = 0f;
        [SerializeField] private float amplitudeDeg = 35f;
        [SerializeField] private float frequencyHz = 0.5f;
        [SerializeField] private float phaseOffsetSeconds = 0f;

        [Header("Direction (Inspector uses UnityEngine.Vector2)")]
        [SerializeField] private UVec2 localBaseDir = UVec2.right;

        private LineRenderer lr;

        private void Awake()
        {
            lr = GetComponent<LineRenderer>();
            if (lr == null) lr = gameObject.AddComponent<LineRenderer>();

            lr.positionCount = 2;
            lr.useWorldSpace = true;
        }

        private void Update()
        {
            //calcula el angulo oscilante
            float t = Time.time + phaseOffsetSeconds;
            float angleDeg = baseAngleDeg + Math.Sin(t * Math.PI * 2f * frequencyHz) * amplitudeDeg;

            Utility.Vector2 baseDirUtility = localBaseDir.ToUtility().normalized;
            Utility.Vector2 dirLocalUtility = RotateDeg(baseDirUtility, angleDeg);

            UVec2 dirLocalUnity = dirLocalUtility.ToUnity().normalized;
            UVec2 dirWorldUnity = (UVec2)transform.TransformDirection(dirLocalUnity);

            UVec2 start = (UVec2)transform.position;
            UVec2 end = start + dirWorldUnity * maxDistance;

            var filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = hitMask,
                useTriggers = hitTriggers
            };

            RaycastHit2D[] hits = new RaycastHit2D[1];
            int count = Physics2D.Raycast(start, dirWorldUnity, filter, hits, maxDistance);

            if (count > 0 && hits[0].collider != null)
            {
                end = hits[0].point;

                if (hits[0].collider.gameObject.layer == LayerMask.NameToLayer("LaserBlocker"))
                {
                    var destructible = hits[0].collider.GetComponentInParent<DestructibleTilemapWall>();
                    if (destructible != null)
                        destructible.CarveHoleAtHit(hits[0].point, 1); 
                }

            }

            lr.SetPosition(0, new UVec3(start.x, start.y, lineZ));
            lr.SetPosition(1, new UVec3(end.x, end.y, lineZ));
        }

        private static Utility.Vector2 RotateDeg(Utility.Vector2 v, float degrees)
        {
            float rad = degrees * Math.Deg2Rad;
            float cos = Math.Cos(rad);
            float sin = Math.Sin(rad);

            return new Utility.Vector2(
                v.x * cos - v.y * sin,
                v.x * sin + v.y * cos
            );
        }

        public void SetPhaseSeconds(float seconds) => phaseOffsetSeconds = seconds;
    }

    internal static class Vector2Conversions
    {
        public static Utility.Vector2 ToUtility(this UVec2 v) => new Utility.Vector2(v.x, v.y);
        public static UVec2 ToUnity(this Utility.Vector2 v) => new UVec2(v.x, v.y);
    }
}

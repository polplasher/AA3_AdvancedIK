using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
using UVec2 = UnityEngine.Vector2;
using UVec3 = UnityEngine.Vector3;
using Math = Utility.MathLite;

namespace Lasers
{
    public class OscillatingLaser : MonoBehaviour
    {
        [Header("Laser")] [SerializeField] private float maxDistance = 20f;
        [SerializeField] private bool hitTriggers;
        private const string ArmTag = "Player";

        [Header("Oscillation (degrees)")] [SerializeField]
        private float baseAngleDeg;

        [SerializeField] private float amplitudeDeg = 35f;
        [SerializeField] private float frequencyHz = 0.5f;
        [SerializeField] private float phaseOffsetSeconds;

        [Header("Direction (Inspector uses UnityEngine.Vector2)")] [SerializeField]
        private UVec2 localBaseDir = UVec2.right;

        private LineRenderer lr;

        private void Awake()
        {
            lr = GetComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.useWorldSpace = true;
        }

        private void Update()
        {
            // Calculate the oscillating angle
            float t = Time.time + phaseOffsetSeconds;
            float angleDeg = baseAngleDeg + Math.Sin(t * Math.PI * 2f * frequencyHz) * amplitudeDeg;

            Utility.Vector2 baseDirUtility = localBaseDir.ToUtility().normalized;
            Utility.Vector2 dirLocalUtility = RotateDeg(baseDirUtility, angleDeg);

            UVec2 dirLocalUnity = dirLocalUtility.ToUnity().normalized;
            UVec2 dirWorldUnity = transform.TransformDirection(dirLocalUnity);

            UVec2 start = transform.position;
            UVec2 end = start + dirWorldUnity * maxDistance;

            ContactFilter2D filter = new()
            {
                useLayerMask = true,
                layerMask = Physics2D.GetLayerCollisionMask(0),
                useTriggers = hitTriggers
            };

            var hits = new RaycastHit2D[1];
            int count = Physics2D.Raycast(start, dirWorldUnity, filter, hits, maxDistance);

            if (count > 0 && hits[0].collider)
            {
                end = hits[0].point;

                // Verify if the hit collider belongs to the player
                if (IsPlayerHit(hits[0].collider))
                {
                    Scene currentScene = SceneManager.GetActiveScene();
                    SceneManager.LoadScene(currentScene.buildIndex);
                }
            }

            lr.SetPosition(0, new UVec3(start.x, start.y));
            lr.SetPosition(1, new UVec3(end.x, end.y));
        }

        private bool IsPlayerHit(Collider2D hitCollider)
        {
            if (!hitCollider)
                return false;

            // Verificar si tiene el tag de brazo del jugador
            if (!string.IsNullOrEmpty(ArmTag) && hitCollider.CompareTag(ArmTag))
                return true;

            // Verificar si el objeto o su padre tiene un IKController
            IKController ikController = hitCollider.GetComponentInParent<IKController>();
            if (ikController)
                return true;

            return false;
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

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Draw base direction
            Utility.Vector2 baseDirUtility = localBaseDir.ToUtility().normalized;
            Utility.Vector2 dirLocalUtility = RotateDeg(baseDirUtility, baseAngleDeg);
            UVec2 dirLocalUnity = dirLocalUtility.ToUnity().normalized;
            UVec2 dirWorldUnity = transform.TransformDirection(dirLocalUnity);

            UVec3 origin = transform.position;

            // Dibujar la dirección base (amarillo)
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(origin, dirWorldUnity * maxDistance);

            // Dibujar el rango de oscilación (cian semi-transparente)
            Gizmos.color = new Color(0f, 1f, 1f, 0.3f);

            // Ángulo máximo (superior)
            Utility.Vector2 dirMaxUtility = RotateDeg(baseDirUtility, baseAngleDeg + amplitudeDeg);
            UVec2 dirMaxUnity = dirMaxUtility.ToUnity().normalized;
            UVec2 dirMaxWorld = transform.TransformDirection(dirMaxUnity);
            Gizmos.DrawRay(origin, dirMaxWorld * maxDistance);

            // Ángulo mínimo (inferior)
            Utility.Vector2 dirMinUtility = RotateDeg(baseDirUtility, baseAngleDeg - amplitudeDeg);
            UVec2 dirMinUnity = dirMinUtility.ToUnity().normalized;
            UVec2 dirMinWorld = transform.TransformDirection(dirMinUnity);
            Gizmos.DrawRay(origin, dirMinWorld * maxDistance);

            // Dibujar un arco para mostrar el rango de oscilación
            DrawArc(origin, dirMinWorld, dirMaxWorld, maxDistance);

            // Dibujar posiciones intermedias del láser (verde claro)
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
            const int steps = 8;
            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                float angle = baseAngleDeg + (t * 2f - 1f) * amplitudeDeg;
                Utility.Vector2 dirUtility = RotateDeg(baseDirUtility, angle);
                UVec2 dirUnity = dirUtility.ToUnity().normalized;
                UVec2 dirWorld = transform.TransformDirection(dirUnity);
                Gizmos.DrawRay(origin, dirWorld * maxDistance);
            }

            // Dibujar el origen
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(origin, 0.1f);
        }

        private void DrawArc(UVec3 center, UVec2 from, UVec2 to, float radius)
        {
            // Dibujar líneas que conectan los extremos del arco
            const int segments = 20;
            float angleFrom = Mathf.Atan2(from.y, from.x);
            float angleTo = Mathf.Atan2(to.y, to.x);
            float angleDiff = Mathf.DeltaAngle(angleFrom * Mathf.Rad2Deg, angleTo * Mathf.Rad2Deg);

            UVec3 prevPoint = center + (UVec3)(from.normalized * radius);
            for (int i = 1; i <= segments; i++)
            {
                float t = (float)i / segments;
                float angle = angleFrom + (angleDiff * Mathf.Deg2Rad * t);
                UVec2 dir = new(Mathf.Cos(angle), Mathf.Sin(angle));
                UVec3 point = center + (UVec3)(dir * radius);
                Gizmos.DrawLine(prevPoint, point);
                prevPoint = point;
            }
        }
#endif
    }
}
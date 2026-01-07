using UnityEngine;

public class CircleOscillator : MonoBehaviour
{
    [Header("Circle Movement")] [SerializeField]
    private Vector2 centerOffset = Vector2.zero;

    [SerializeField] private float orbitRadius = 2f;
    [SerializeField] private float rotationSpeed = 50f;

    [Header("Gizmos")] [SerializeField] private Color gizmoColor = Color.cyan;
    [SerializeField] private int gizmoSegments = 32;

    private Vector2 startPosition;
    private float currentAngle;

    private void Start()
    {
        startPosition = transform.position;
        currentAngle = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        currentAngle += rotationSpeed * Time.deltaTime * Mathf.Deg2Rad;

        Vector2 center = startPosition + centerOffset;
        Vector2 offset = new(
            Mathf.Cos(currentAngle) * orbitRadius,
            Mathf.Sin(currentAngle) * orbitRadius
        );

        transform.position = center + offset;
    }

    private void OnDrawGizmos()
    {
        Vector2 center = Application.isPlaying
            ? startPosition + centerOffset
            : (Vector2)transform.position + centerOffset;

        Gizmos.color = gizmoColor;

        // Draw center point
        Gizmos.DrawWireSphere(center, 0.1f);

        // Draw orbit circle
        Vector3 previousPoint = center + new Vector2(orbitRadius, 0);
        for (int i = 1; i <= gizmoSegments; i++)
        {
            float angle = (i / (float)gizmoSegments) * Mathf.PI * 2f;
            Vector3 currentPoint = center + new Vector2(
                Mathf.Cos(angle) * orbitRadius,
                Mathf.Sin(angle) * orbitRadius
            );

            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        // Draw line from center to current position
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.5f);
        Gizmos.DrawLine(center, transform.position);
    }
}
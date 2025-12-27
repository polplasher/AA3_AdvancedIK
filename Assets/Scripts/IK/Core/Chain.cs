using Unity.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class Chain : MonoBehaviour
{
    [Header("Chain"), SerializeField] private Transform[] joints;

    [Header("Computed (read-only)"), ReadOnly, SerializeField]
    private float[] lengths;

    [ReadOnly, SerializeField] private float totalLength;

    public Transform[] Joints => joints;
    public int JointCount => joints?.Length ?? 0;
    public int SegmentCount => Mathf.Max(0, JointCount - 1);
    public float TotalLength => totalLength;

    public bool IsValid()
    {
        if (joints == null || joints.Length < 2)
        {
            return false;
        }

        foreach (Transform t in joints)
        {
            if (!t)
            {
                return false;
            }
        }

        return true;
    }

    public void Rebuild()
    {
        if (!IsValid()) return;

        lengths = new float[SegmentCount];
        totalLength = 0f;

        for (int i = 0; i < SegmentCount; i++)
        {
            float len = Vector2.Distance(joints[i].position, joints[i + 1].position);
            // Avoid 0 lengths that break IK
            len = Mathf.Max(0.0001f, len);

            lengths[i] = len;
            totalLength += len;
        }
    }

    public float GetLength(int segmentIndex)
    {
        if (lengths == null || segmentIndex < 0 || segmentIndex >= lengths.Length)
            return 0f;
        return lengths[segmentIndex];
    }

    public Vector2[] GetPositions()
    {
        var pos = new Vector2[JointCount];
        for (int i = 0; i < JointCount; i++)
            pos[i] = joints[i].position;
        return pos;
    }

    public void SetPositions(Vector2[] positions)
    {
        if (positions == null || positions.Length != JointCount) return;

        // Important: We do NOT depend on a parent-child hierarchy
        // Each joint is an independent Transform in world space
        for (int i = 0; i < JointCount; i++)
            joints[i].position = positions[i];
    }
}
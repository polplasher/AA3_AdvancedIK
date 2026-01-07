using UnityEngine;
using Vec2 = Utility.Vector2;
using Math = Utility.MathLite;

[DisallowMultipleComponent]
public sealed class Chain : MonoBehaviour
{
    [SerializeField] private Transform[] joints;

    public Transform[] Joints => joints;
    public int JointCount => joints?.Length ?? 0;
    public int SegmentCount => Math.Max(0, JointCount - 1);
    public float TotalLength { get; private set; }
    private float[] lengths;

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
        TotalLength = 0f;

        for (int i = 0; i < SegmentCount; i++)
        {
            // We only use XY for IK math
            Vector3 a = joints[i].position;
            Vector3 b = joints[i + 1].position;

            float len = Vec2.Distance(new Vec2(a.x, a.y), new Vec2(b.x, b.y));
            // Avoid 0 lengths that break IK
            len = Math.Max(0.0001f, len);

            lengths[i] = len;
            TotalLength += len;
        }
    }

    public float GetLength(int segmentIndex)
    {
        if (lengths == null || segmentIndex < 0 || segmentIndex >= lengths.Length)
            return 0f;
        return lengths[segmentIndex];
    }

    public Vec2[] GetPositions()
    {
        var pos = new Vec2[JointCount];
        for (int i = 0; i < JointCount; i++)
        {
            Vector3 p = joints[i].position;
            pos[i] = new Vec2(p.x, p.y);
        }
        return pos;
    }

    public void SetPositions(Vec2[] positions)
    {
        if (positions == null || positions.Length != JointCount) return;

        // Important: We do NOT depend on a parent-child hierarchy
        // Each joint is an independent Transform in world space
        for (int i = 0; i < JointCount; i++)
        {
            Vector3 cur = joints[i].position; // keep Z as-is
            joints[i].position = new Vector3(positions[i].x, positions[i].y, cur.z);
        }
    }
}
using UnityEngine;

[DisallowMultipleComponent]
public sealed class Chain2D : MonoBehaviour
{
    [Header("Chain")]
    [Tooltip("Joints in order from root (0) to end-effector (n-1). No parenting required")]
    [SerializeField] private Transform[] joints;

    [Header("Computed (read-only)")]
    [SerializeField] private float[] _lengths;
    [SerializeField] private float _totalLength;

    public Transform[] Joints => joints;
    public int JointCount => joints != null ? joints.Length : 0;
    public int SegmentCount => Mathf.Max(0, JointCount - 1);
    public float TotalLength => _totalLength;

    public bool IsValid(out string error)
    {
        if (joints == null || joints.Length < 2)
        {
            error = "Chain2D: Need at least 2 joints";
            return false;
        }

        for (int i = 0; i < joints.Length; i++)
        {
            if (joints[i] == null)
            {
                error = $"Chain2D: Joint at index {i} is null";
                return false;
            }
        }

        error = string.Empty;
        return true;
    }

    public void Rebuild()
    {
        if (!IsValid(out _)) return;

        _lengths = new float[SegmentCount];
        _totalLength = 0f;

        for (int i = 0; i < SegmentCount; i++)
        {
            float len = Vector2.Distance(joints[i].position, joints[i + 1].position);
            // Avoid 0 lengths that break IK
            len = Mathf.Max(0.0001f, len);

            _lengths[i] = len;
            _totalLength += len;
        }
    }

    public float GetLength(int segmentIndex)
    {
        if (_lengths == null || segmentIndex < 0 || segmentIndex >= _lengths.Length)
            return 0f;
        return _lengths[segmentIndex];
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        // In the editor, it recalculates when you touch something
        if (joints != null && joints.Length >= 2)
            Rebuild();
    }
#endif
}

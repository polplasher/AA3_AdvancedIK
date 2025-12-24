using UnityEngine;

[DisallowMultipleComponent]
public sealed class IKController2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Chain2D chain;
    [SerializeField] private Transform target;

    [Tooltip("Optional: if assigned, end-effector will be moved using Rigidbody2D.MovePosition for stable trigger physics")]
    [SerializeField] private Rigidbody2D endEffectorBody;
    [Header("Algorithm")]
    [SerializeField] private IKAlgorithm2D algorithm = IKAlgorithm2D.FABRIK;

    [Header("Settings")]
    [Min(1)] [SerializeField] private int maxIterations = 12;
    [Min(0.0001f)] [SerializeField] private float epsilon = 0.02f;
    [Header("Runtime (read-only)")]
    [SerializeField] private IKResult2D _lastResult;

    private Vector2[] _positions;

    public Chain2D Chain => chain;
    public Transform Target => target;
    public Rigidbody2D EndEffectorBody => endEffectorBody;

    public IKAlgorithm2D Algorithm
    {
        get => algorithm;
        set => algorithm = value;
    }

    public int MaxIterations
    {
        get => maxIterations;
        set => maxIterations = Mathf.Max(1, value);
    }

    public float Epsilon
    {
        get => epsilon;
        set => epsilon = Mathf.Max(0.0001f, value);
    }
    
    public IKResult2D LastResult => _lastResult;

    private void Awake()
    {
        if (chain != null)
            chain.Rebuild();

        _positions = null;
    }

    private void FixedUpdate()
    {
        if (chain == null || target == null) return;
        if (!chain.IsValid(out _)) return;

        // Rebuild if the user repositions joints at runtime
        if (chain.SegmentCount > 0 && (chain.TotalLength <= 0.0001f))
            chain.Rebuild();

        var settings = new IKSettings2D
        {
            maxIterations = maxIterations,
            epsilon = epsilon
        };

        Vector2 tar = target.position;

        // We work by positions in world space (without hierarchy)
        _positions = chain.GetPositions();

        _lastResult = algorithm switch
        {
            IKAlgorithm2D.FABRIK => FabrikSolver2D.Solve(chain, tar, settings, ref _positions),
            IKAlgorithm2D.CCD => CCDSolver2D.Solve(chain, tar, settings, ref _positions),
            _ => _lastResult
        };

        // We apply to transforms
        // Important: To ensure reliable effector triggers, we move the effector with MovePosition if there is a Rigidbody2D
        if (endEffectorBody != null && chain.JointCount >= 2)
        {
            int end = chain.JointCount - 1;

            // Set joints except the effector
            for (int i = 0; i < end; i++)
                chain.Joints[i].position = _positions[i];

            // Effector by physics
            endEffectorBody.MovePosition(_positions[end]);
        }
        else
        {
            chain.SetPositions(_positions);
        }
    }
}

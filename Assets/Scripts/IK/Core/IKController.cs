using UnityEngine;

[DisallowMultipleComponent]
public sealed class IKController : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Chain chain;

    [SerializeField] private Transform target;

    [Tooltip(
        "Optional: if assigned, end-effector will be moved using Rigidbody2D.MovePosition for stable trigger physics")]
    [SerializeField]
    private Rigidbody2D endEffectorBody;

    [Header("Algorithm")] [SerializeField] private IKAlgorithm algorithm = IKAlgorithm.FABRIK;

    [Header("Settings")] [Min(1)] [SerializeField]
    private int maxIterations = 12;

    [Min(0.0001f)] [SerializeField] private float epsilon = 0.02f;

    [Header("Runtime (read-only)")] private IKResult2D lastResult;

    private readonly CCDSolver ccdSolver = new();

    private Vector2[] positions;

    public IKAlgorithm Algorithm
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

    public IKResult2D LastResult => lastResult;

    private void Awake()
    {
        if (chain != null)
            chain.Rebuild();

        positions = null;
    }

    private void FixedUpdate()
    {
        if (chain == null || target == null) return;
        if (!chain.IsValid()) return;

        // Rebuild if the user repositions joints at runtime
        if (chain.SegmentCount > 0 && chain.TotalLength <= 0.0001f)
            chain.Rebuild();

        var settings = new IKSettings2D
        {
            MaxIterations = maxIterations,
            Epsilon = epsilon
        };

        Vector2 tar = target.position;

        // We work by positions in world space (without hierarchy)
        positions = chain.GetPositions();

        // Solve based on selected algorithm
        // Each solver manages its own internal state if needed
        lastResult = algorithm switch
        {
            IKAlgorithm.FABRIK => FabrikSolver.Solve(chain, tar, settings, ref positions),
            IKAlgorithm.CCD => ccdSolver.Solve(chain, tar, settings, ref positions),
            _ => lastResult
        };

        // Apply positions to transforms
        // Important: To ensure reliable effector triggers, we move the effector with MovePosition if there is a Rigidbody2D
        if (endEffectorBody && chain.JointCount >= 2)
        {
            int end = chain.JointCount - 1;

            // Set joints except the effector
            for (int i = 0; i < end; i++)
                chain.Joints[i].position = positions[i];

            // Effector by physics
            endEffectorBody.MovePosition(positions[end]);
        }
        else
        {
            chain.SetPositions(positions);
        }
    }
}
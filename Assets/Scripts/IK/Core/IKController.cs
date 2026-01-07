using UnityEngine;
using Vec2 = Utility.Vector2;
using Math = Utility.MathLite;

[DisallowMultipleComponent]
public sealed class IKController : MonoBehaviour
{
    [Header("Settings")] public IKAlgorithm algorithm;
    [Min(1), SerializeField] private int maxIterations = 12;
    [Range(0.0001f, 20f), SerializeField] private float epsilon = 0.02f;

    [Header("References"), SerializeField] private Chain chain;
    [field: SerializeField] public Transform Target { get; set; }

    [Tooltip(
        "Optional: if assigned, end-effector will be moved using Rigidbody2D.MovePosition for stable trigger physics")]
    [SerializeField]
    private Rigidbody2D endEffectorBody;

    private Vec2[] positions;

    public int MaxIterations
    {
        get => maxIterations;
        set => maxIterations = Math.Max(1, value);
    }

    public float Epsilon
    {
        get => epsilon;
        set => epsilon = Math.Max(0.0001f, value);
    }

    public IKResult LastResult { get; private set; }

    private void Awake()
    {
        chain.Rebuild();

        positions = null;
    }

    private void FixedUpdate()
    {
        if (!chain.IsValid()) return;

        // Rebuild if the user repositions joints at runtime
        if (chain.SegmentCount > 0 && chain.TotalLength <= 0.0001f)
            chain.Rebuild();

        IKSettings settings = new()
        {
            MaxIterations = maxIterations,
            Epsilon = epsilon
        };

        Vector3 tp = Target.position;
        Vec2 tar = new(tp.x, tp.y);

        // We work by positions in world space (without hierarchy)
        positions = chain.GetPositions();

        // Solve based on selected algorithm
        // Each solver manages its own internal state if needed
        LastResult = algorithm switch
        {
            IKAlgorithm.FABRIK => FabrikSolver.Solve(chain, tar, settings, ref positions),
            IKAlgorithm.CCD => CCDSolver.Solve(chain, tar, settings, ref positions),
            _ => LastResult
        };

        // Apply positions to transforms
        // Important: To ensure reliable effector triggers, we move the effector with MovePosition if there is a Rigidbody2D
        if (endEffectorBody && chain.JointCount >= 2)
        {
            int end = chain.JointCount - 1;

            // Set joints except the effector (keep Z)
            for (int i = 0; i < end; i++)
            {
                Vector3 cur = chain.Joints[i].position;
                chain.Joints[i].position = new Vector3(positions[i].x, positions[i].y, cur.z);
            }

            // Effector by physics (XY only)
            endEffectorBody.MovePosition(new Vector2(positions[end].x, positions[end].y));
        }
        else
        {
            chain.SetPositions(positions);
        }
    }
}
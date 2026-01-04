using Vec2 = Utility.Vector2;

public static class FabrikSolver
{
    /// <summary>
    /// FABRIK solver implementation.
    /// Uses lambda interpolation method for maintaining link distances.
    /// </summary>
    public static IKResult Solve(Chain chain, Vec2 target, IKSettings settings, ref Vec2[] positions)
    {
        if (!chain.IsValid())
            return new IKResult(0, float.PositiveInfinity, false);

        int n = chain.JointCount;
        if (positions == null || positions.Length != n)
            positions = chain.GetPositions();

        Vec2 rootPos = positions[0];

        // Unreachable case: stretch towards the target
        if (Vec2.Distance(rootPos, target) > chain.TotalLength)
        {
            for (int i = 0; i < n - 1; i++)
            {
                Vec2 dir = (target - positions[i]).normalized;
                positions[i + 1] = positions[i] + dir * chain.GetLength(i);
            }
            return new IKResult(1, Vec2.Distance(positions[n - 1], target), false);
        }

        // Main FABRIK loop
        int iterations = 0;
        float error = Vec2.Distance(positions[n - 1], target);

        while (iterations < settings.MaxIterations && error > settings.Epsilon)
        {
            iterations++;

            // Forward pass: from end effector to root
            positions[n - 1] = target;
            for (int i = n - 2; i >= 0; i--)
            {
                float lambda = chain.GetLength(i) / Vec2.Distance(positions[i], positions[i + 1]);
                positions[i] += (1 - lambda) * (positions[i + 1] - positions[i]);
            }

            // Backward pass: from root to end effector
            positions[0] = rootPos;
            for (int i = 1; i < n; i++)
            {
                float lambda = chain.GetLength(i - 1) / Vec2.Distance(positions[i - 1], positions[i]);
                positions[i] += (1 - lambda) * (positions[i - 1] - positions[i]);
            }

            error = Vec2.Distance(positions[n - 1], target);
        }

        return new IKResult(iterations, error, error <= settings.Epsilon);
    }
}
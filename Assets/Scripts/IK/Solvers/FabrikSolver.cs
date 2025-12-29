using Vec2 = Utility.Vector2;

public static class FabrikSolver
{
    public static IKResult Solve(Chain chain, Vec2 target, IKSettings settings, ref Vec2[] positions)
    {
        if (!chain.IsValid())
            return new IKResult(0, float.PositiveInfinity, false);

        int n = chain.JointCount;
        if (positions == null || positions.Length != n)
            positions = chain.GetPositions();

        Vec2 rootPos = positions[0];
        float distRootToTarget = Vec2.Distance(rootPos, target);

        // Unreachable case: reach towards the target
        if (distRootToTarget > chain.TotalLength)
        {
            for (int i = 0; i < n - 1; i++)
            {
                float len = chain.GetLength(i);
                Vec2 dir = (target - positions[i]).normalized;
                positions[i + 1] = positions[i] + dir * len;
            }

            float err = Vec2.Distance(positions[n - 1], target);
            return new IKResult(1, err, err <= settings.Epsilon);
        }

        int iterations = 0;
        float error = Vec2.Distance(positions[n - 1], target);

        while (iterations < settings.MaxIterations && error > settings.Epsilon)
        {
            iterations++;

            // Forward reaching: effector to target
            positions[n - 1] = target;
            for (int i = n - 2; i >= 0; i--)
            {
                float len = chain.GetLength(i);
                Vec2 dir = (positions[i] - positions[i + 1]).normalized;
                positions[i] = positions[i + 1] + dir * len;
            }

            // Backward reaching: root returns to its original position
            positions[0] = rootPos;
            for (int i = 0; i < n - 1; i++)
            {
                float len = chain.GetLength(i);
                Vec2 dir = (positions[i + 1] - positions[i]).normalized;
                positions[i + 1] = positions[i] + dir * len;
            }

            error = Vec2.Distance(positions[n - 1], target);
        }

        return new IKResult(iterations, error, error <= settings.Epsilon);
    }
}

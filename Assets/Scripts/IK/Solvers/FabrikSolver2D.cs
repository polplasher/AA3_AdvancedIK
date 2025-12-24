using UnityEngine;

public static class FabrikSolver2D
{
    public static IKResult2D Solve(Chain2D chain, Vector2 target, IKSettings2D settings, ref Vector2[] positions)
    {
        if (!chain.IsValid(out _))
            return new IKResult2D(0, float.PositiveInfinity, false);

        int n = chain.JointCount;
        if (positions == null || positions.Length != n)
            positions = chain.GetPositions();

        Vector2 rootPos = positions[0];
        float distRootToTarget = Vector2.Distance(rootPos, target);

        // Unreachable case: reach towards the target
        if (distRootToTarget > chain.TotalLength)
        {
            for (int i = 0; i < n - 1; i++)
            {
                float len = chain.GetLength(i);
                Vector2 dir = (target - positions[i]).normalized;
                positions[i + 1] = positions[i] + dir * len;
            }

            float err = Vector2.Distance(positions[n - 1], target);
            return new IKResult2D(1, err, err <= settings.epsilon);
        }

        int iterations = 0;
        float error = Vector2.Distance(positions[n - 1], target);

        while (iterations < settings.maxIterations && error > settings.epsilon)
        {
            iterations++;

            // Forward reaching: effector to target
            positions[n - 1] = target;
            for (int i = n - 2; i >= 0; i--)
            {
                float len = chain.GetLength(i);
                Vector2 dir = (positions[i] - positions[i + 1]).normalized;
                positions[i] = positions[i + 1] + dir * len;
            }

            // Backward reaching: root returns to its original position
            positions[0] = rootPos;
            for (int i = 0; i < n - 1; i++)
            {
                float len = chain.GetLength(i);
                Vector2 dir = (positions[i + 1] - positions[i]).normalized;
                positions[i + 1] = positions[i] + dir * len;
            }

            error = Vector2.Distance(positions[n - 1], target);
        }

        return new IKResult2D(iterations, error, error <= settings.epsilon);
    }
}

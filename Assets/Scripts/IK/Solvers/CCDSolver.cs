using Vec2 = Utility.Vector2;
using Math = Utility.MathLite;

public class CCDSolver
{
    /// <summary>
    /// CCD solver implementation.
    /// Processes joints from end to root, rotating each joint to point the end effector toward the target.
    /// </summary>
    public static IKResult Solve(Chain chain, Vec2 target, IKSettings settings, ref Vec2[] positions)
    {
        if (!chain.IsValid())
            return new IKResult(0, float.PositiveInfinity, false);

        int n = chain.JointCount;
        if (positions == null || positions.Length != n)
            positions = chain.GetPositions();

        // Unreachable case: stretch towards the target
        float distRootToTarget = Vec2.Distance(positions[0], target);
        if (distRootToTarget > chain.TotalLength)
        {
            for (int i = 0; i < n - 1; i++)
            {
                Vec2 dir = (target - positions[i]).normalized;
                positions[i + 1] = positions[i] + dir * chain.GetLength(i);
            }
            return new IKResult(1, Vec2.Distance(positions[n - 1], target), false);
        }

        // Main CCD loop
        int iterations = 0;
        float error = Vec2.Distance(positions[n - 1], target);

        while (iterations < settings.MaxIterations && error > settings.Epsilon)
        {
            iterations++;

            // Process joints from end to root
            for (int jointIndex = n - 2; jointIndex >= 0; jointIndex--)
            {
                Vec2 currentJoint = positions[jointIndex];
                Vec2 endEffector = positions[n - 1];

                // Get vectors from current joint to end effector and target
                Vec2 toEndEffector = (endEffector - currentJoint).normalized;
                Vec2 toTarget = (target - currentJoint).normalized;

                // Calculate rotation angle
                float angle = Math.Acos(Math.Clamp(Vec2.Dot(toEndEffector, toTarget), -1f, 1f));
                
                // Determine rotation direction (2D cross product Z component)
                float cross = toEndEffector.x * toTarget.y - toEndEffector.y * toTarget.x;
                if (cross < 0) angle = -angle;

                // Rotate all joints from current+1 to end around current joint
                for (int i = jointIndex + 1; i < n; i++)
                {
                    positions[i] = Math.RotateAround(positions[i], currentJoint, angle);
                }
            }

            error = Vec2.Distance(positions[n - 1], target);
        }

        return new IKResult(iterations, error, error <= settings.Epsilon);
    }
}
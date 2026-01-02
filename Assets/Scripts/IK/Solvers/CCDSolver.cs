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

        int iterations = 0;
        float error = Vec2.Distance(positions[n - 1], target);

        while (iterations < settings.MaxIterations && error > settings.Epsilon)
        {
            iterations++;

            // Process joints from end to root
            for (int jointIndex = n - 2; jointIndex >= 0; jointIndex--)
            {
                Vec2 currentJoint = positions[jointIndex];

                // Get vectors from current joint to end effector and target
                Vec2 toEndEffector = (positions[n - 1] - currentJoint).normalized;
                Vec2 toTarget = (target - currentJoint).normalized;

                // Skip if vectors are too small
                if (toEndEffector.magnitude < 1e-6f || toTarget.magnitude < 1e-6f)
                    continue;

                // Calculate rotation angle
                float dotProduct = Vec2.Dot(toEndEffector, toTarget);
                dotProduct = Math.Clamp(dotProduct, -1.0f, 1.0f);
                float angle = Math.Acos(dotProduct);

                // Determine rotation direction using cross product (in 2D, check Z component)
                // In 2D: cross product Z = x1*y2 - y1*x2
                float crossZ = toEndEffector.x * toTarget.y - toEndEffector.y * toTarget.x;
                if (crossZ < 0)
                    angle = -angle;

                // Rotate all joints from current+1 to end effector around current joint
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
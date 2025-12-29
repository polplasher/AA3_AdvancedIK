using Vec2 = Utility.Vector2;
using Math = Utility.MathLite;

public class CCDSolver
{
    /// <summary>
    /// Internal bounce depth for the single CCD chain in the scene.
    /// </summary>
    private int bounceDepth;

    /// <summary>
    /// CCD solver with bounce mechanism - progressively processes joints in a cycling pattern.
    /// 
    /// This variant implements a "bouncing" iteration strategy where instead of processing
    /// all joints in every iteration, it cycles through progressively larger subsets:
    /// - Call 1: Process only the end-most joint
    /// - Call 2: Process the last 2 joints
    /// - Call 3: Process the last 3 joints
    /// - ...and so on until all joints are processed, then the cycle repeats
    /// </summary>
    public IKResult Solve(Chain chain, Vec2 target, IKSettings settings, ref Vec2[] positions)
    {
        if (!chain.IsValid())
            return new IKResult(0, float.PositiveInfinity, false);

        int n = chain.JointCount;
        if (positions == null || positions.Length != n)
            positions = chain.GetPositions();

        // Use the single BounceDepth for the scene
        int frameDepth = bounceDepth;

        // Ensure bounceDepth is in valid range
        frameDepth = Math.Clamp(frameDepth, 0, n - 2);

        int iterations = 0;
        float error = Vec2.Distance(positions[n - 1], target);

        while (iterations < settings.MaxIterations && error > settings.Epsilon)
        {
            iterations++;

            // Calculate the starting joint index based on bounce depth
            int startJoint = n - 2 - frameDepth;

            // Ensure we don't go below root (joint 0)
            startJoint = Math.Max(0, startJoint);

            // Traverse joints from startJoint to the root in this bounce cycle
            for (int i = startJoint; i >= 0; i--)
            {
                Vec2 jointPos = positions[i];
                Vec2 toEff = positions[n - 1] - jointPos;
                Vec2 toTar = target - jointPos;

                float toEffMag = toEff.magnitude;
                float toTarMag = toTar.magnitude;

                if (toEffMag < 1e-6f || toTarMag < 1e-6f)
                    continue;

                // Signed angle in 2D (Z)
                float angle = Math.SignedAngleRad(toEff, toTar);

                // Rotate all downstream points around joint i
                for (int j = i + 1; j < n; j++)
                    positions[j] = Math.RotateAround(positions[j], jointPos, angle);
            }

            error = Vec2.Distance(positions[n - 1], target);
        }

        // Update bounce depth for next call (cycles from 0 to n-2)
        int maxBounceDepth = Math.Max(0, n - 2);
        bounceDepth = frameDepth + 1 > maxBounceDepth ? 0 : frameDepth + 1;

        return new IKResult(iterations, error, error <= settings.Epsilon);
    }
}
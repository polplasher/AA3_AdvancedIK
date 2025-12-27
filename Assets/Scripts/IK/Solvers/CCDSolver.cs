using UnityEngine;

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
    public IKResult Solve(Chain chain, Vector2 target, IKSettings settings, ref Vector2[] positions)
    {
        if (!chain.IsValid())
            return new IKResult(0, float.PositiveInfinity, false);

        int n = chain.JointCount;
        if (positions == null || positions.Length != n)
            positions = chain.GetPositions();

        // Use the single BounceDepth for the scene
        int frameDepth = bounceDepth;

        // Ensure bounceDepth is in valid range
        frameDepth = Mathf.Clamp(frameDepth, 0, n - 2);

        // Convert UnityEngine.Vector2[] to Utility.Vector2[] for calculations
        Utility.Vector2[] uPos = new Utility.Vector2[n];
        for (int i = 0; i < n; i++)
            uPos[i] = new Utility.Vector2(positions[i].x, positions[i].y);

        Utility.Vector2 uTarget = new(target.x, target.y);

        int iterations = 0;
        float error = Utility.Vector2.Distance(uPos[n - 1], uTarget);

        while (iterations < settings.MaxIterations && error > settings.Epsilon)
        {
            iterations++;

            // Calculate the starting joint index based on bounce depth
            int startJoint = n - 2 - frameDepth;

            // Ensure we don't go below root (joint 0)
            startJoint = Mathf.Max(0, startJoint);

            // Traverse joints from startJoint to the root in this bounce cycle
            for (int i = startJoint; i >= 0; i--)
            {
                Utility.Vector2 jointPos = uPos[i];
                Utility.Vector2 toEff = uPos[n - 1] - jointPos;
                Utility.Vector2 toTar = uTarget - jointPos;

                float toEffMag = toEff.magnitude;
                float toTarMag = toTar.magnitude;

                if (toEffMag < 1e-6f || toTarMag < 1e-6f)
                    continue;

                // Signed angle in 2D (Z)
                float angle = Utility.MathLite.SignedAngleRad(toEff, toTar);

                // Rotate all downstream points around joint i
                for (int j = i + 1; j < n; j++)
                    uPos[j] = Utility.MathLite.RotateAround(uPos[j], jointPos, angle);
            }

            error = Utility.Vector2.Distance(uPos[n - 1], uTarget);
        }

        // Write back converted positions to the Unity array
        for (int i = 0; i < n; i++)
            positions[i] = new Vector2(uPos[i].x, uPos[i].y);

        // Update bounce depth for next call (cycles from 0 to n-2)
        int maxBounceDepth = Mathf.Max(0, n - 2);
        bounceDepth = frameDepth + 1 > maxBounceDepth ? 0 : frameDepth + 1;

        return new IKResult(iterations, error, error <= settings.Epsilon);
    }
}
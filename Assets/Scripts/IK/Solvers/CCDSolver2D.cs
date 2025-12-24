using UnityEngine;

public static class CCDSolver2D
{
    public static IKResult2D Solve(Chain2D chain, Vector2 target, IKSettings2D settings, ref Vector2[] positions)
    {
        if (!chain.IsValid(out _))
            return new IKResult2D(0, float.PositiveInfinity, false);

        int n = chain.JointCount;
        if (positions == null || positions.Length != n)
            positions = chain.GetPositions();

        int iterations = 0;
        float error = Vector2.Distance(positions[n - 1], target);

        while (iterations < settings.maxIterations && error > settings.epsilon)
        {
            iterations++;

            // Traverse joints from the second-to-last to the root
            for (int i = n - 2; i >= 0; i--)
            {
                Vector2 jointPos = positions[i];
                Vector2 toEff = positions[n - 1] - jointPos;
                Vector2 toTar = target - jointPos;

                float toEffMag = toEff.magnitude;
                float toTarMag = toTar.magnitude;

                if (toEffMag < 1e-6f || toTarMag < 1e-6f)
                    continue;

                // Signed angle in 2D (Z)
                float angle = SignedAngleRad(toEff, toTar);

                // We rotate all the "downstream" points around the joint i
                for (int j = i + 1; j < n; j++)
                    positions[j] = RotateAround(positions[j], jointPos, angle);
            }

            error = Vector2.Distance(positions[n - 1], target);
        }

        return new IKResult2D(iterations, error, error <= settings.epsilon);
    }

    private static Vector2 RotateAround(Vector2 point, Vector2 pivot, float radians)
    {
        float s = Mathf.Sin(radians);
        float c = Mathf.Cos(radians);
        Vector2 p = point - pivot;
        Vector2 pr = new Vector2(p.x * c - p.y * s, p.x * s + p.y * c);
        return pivot + pr;
    }

    private static float SignedAngleRad(Vector2 from, Vector2 to)
    {
        from.Normalize();
        to.Normalize();
        float cross = from.x * to.y - from.y * to.x;
        float dot = Vector2.Dot(from, to);
        return Mathf.Atan2(cross, dot);
    }
}

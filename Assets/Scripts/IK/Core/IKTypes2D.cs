using UnityEngine;

public enum IKAlgorithm2D
{
    FABRIK,
    CCD
}

public struct IKSettings2D
{
    public int maxIterations;
    public float epsilon;

    public static IKSettings2D Default => new IKSettings2D
    {
        maxIterations = 12,
        epsilon = 0.02f
    };
}

public struct IKResult2D
{
    public int iterationsUsed;
    public float finalError;
    public bool reached;

    public IKResult2D(int it, float err, bool reachedTarget)
    {
        iterationsUsed = it;
        finalError = err;
        reached = reachedTarget;
    }
}

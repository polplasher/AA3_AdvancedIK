public enum IKAlgorithm
{
    FABRIK,
    CCD
}

public struct IKSettings2D
{
    public int MaxIterations;
    public float Epsilon;
}

public struct IKResult2D
{
    public readonly int IterationsUsed;
    public readonly float FinalError;
    public readonly bool Reached;

    public IKResult2D(int it, float err, bool reachedTarget)
    {
        IterationsUsed = it;
        FinalError = err;
        Reached = reachedTarget;
    }
}
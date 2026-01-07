public enum IKAlgorithm
{
    FABRIK,
    CCD
}

public struct IKSettings
{
    public int MaxIterations;
    public float Epsilon;
}

public struct IKResult
{
    public readonly int IterationsUsed;
    public readonly float FinalError;
    public readonly bool Reached;

    public IKResult(int it, float err, bool reachedTarget)
    {
        IterationsUsed = it;
        FinalError = err;
        Reached = reachedTarget;
    }
}
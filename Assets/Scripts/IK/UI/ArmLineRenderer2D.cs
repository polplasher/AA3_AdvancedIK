using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public sealed class ArmLineRenderer2D : MonoBehaviour
{
    [SerializeField] private Chain2D chain;
    [SerializeField] private bool updateInLateUpdate = true;

    private LineRenderer _lr;

    private void Awake()
    {
        _lr = GetComponent<LineRenderer>();
    }

    private void LateUpdate()
    {
        if (!updateInLateUpdate) return;
        Draw();
    }

    private void Update()
    {
        if (updateInLateUpdate) return;
        Draw();
    }

    private void Draw()
    {
        if (chain == null || !chain.IsValid(out _)) return;

        _lr.positionCount = chain.JointCount;
        for (int i = 0; i < chain.JointCount; i++)
            _lr.SetPosition(i, chain.Joints[i].position);
    }
}

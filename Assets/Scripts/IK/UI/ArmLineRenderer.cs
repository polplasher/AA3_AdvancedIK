using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public sealed class ArmLineRenderer : MonoBehaviour
{
    [SerializeField] private Chain chain;

    private LineRenderer lr;

    private void Awake() => lr = GetComponent<LineRenderer>();
    private void Update() => Draw();

    private void Draw()
    {
        if (!chain || !chain.IsValid()) return;

        lr.positionCount = chain.JointCount;
        for (int i = 0; i < chain.JointCount; i++)
            lr.SetPosition(i, chain.Joints[i].position);
    }
}
using UnityEngine;

[DisallowMultipleComponent]
public sealed class TargetMouse2D : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private bool holdLeftMouse = true;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        if (holdLeftMouse && !Input.GetMouseButton(0)) return;
        if (cam == null) return;

        Vector3 p = cam.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0f;
        transform.position = p;
    }
}

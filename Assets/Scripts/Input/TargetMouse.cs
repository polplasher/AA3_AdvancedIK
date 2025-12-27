using UnityEngine;

[DisallowMultipleComponent]
public sealed class TargetMouse : MonoBehaviour
{
    [SerializeField] private bool holdLeftMouse = true;
    private Camera mainCamera;

    private void Awake() => mainCamera = Camera.main;

    private void Update()
    {
        if (holdLeftMouse && !Input.GetMouseButton(0)) return;

        Vector3 p = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0f;
        transform.position = p;
    }
}
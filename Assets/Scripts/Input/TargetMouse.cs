using UnityEngine;

[DisallowMultipleComponent]
public sealed class TargetMouse : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake() => mainCamera = Camera.main;

    private void Update()
    {
        Vector3 p = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0f;
        transform.position = p;
    }
}
using UnityEngine;
using UnityEngine.Events;

public sealed class ButtonTrigger2D : MonoBehaviour
{
    [Tooltip("Tag of end-effector collider, e.g. EndEffector")]
    [SerializeField] private string endEffectorTag = "EndEffector";

    [SerializeField] private UnityEvent onPressed;

    private bool _pressed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_pressed) return;
        if (!other.CompareTag(endEffectorTag)) return;

        _pressed = true;
        onPressed?.Invoke();
    }

    public void ResetButton()
    {
        _pressed = false;
    }
}

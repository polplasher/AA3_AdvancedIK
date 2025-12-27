using UnityEngine;
using UnityEngine.Events;

public sealed class ButtonTrigger : MonoBehaviour
{
    [Tooltip("Tag of end-effector collider, e.g. EndEffector")] [SerializeField]
    private string endEffectorTag = "EndEffector";

    [SerializeField] private UnityEvent onPressed;

    private bool pressed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pressed) return;
        if (!other.CompareTag(endEffectorTag)) return;

        pressed = true;
        onPressed?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(endEffectorTag)) return;
        pressed = false;
    }
}
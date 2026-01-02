using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Button : MonoBehaviour
{
    [SerializeField] private float pressThreshold = 0.5f;
    [SerializeField] private UnityEvent onPressedUnityEvent;
    public event Action OnPressed;

    private const string EffectorTag = "EndEffector";
    public bool IsPressed { get; private set; }
    private bool isPressing;
    private float pressTime;
    private SpriteRenderer spriteRenderer;

    private void Start() => spriteRenderer = GetComponent<SpriteRenderer>();

    private void Update()
    {
        if (IsPressed) return;

        if (isPressing)
        {
            pressTime += Time.deltaTime;
            if (pressTime >= pressThreshold)
            {
                spriteRenderer.color = Color.green;
                IsPressed = true;
                OnPressed?.Invoke();
                onPressedUnityEvent?.Invoke();
            }
        }
        else
        {
            // reset
            pressTime = 0f;
            spriteRenderer.color = Color.white;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(EffectorTag))
        {
            isPressing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(EffectorTag))
        {
            isPressing = false;
        }
    }
}
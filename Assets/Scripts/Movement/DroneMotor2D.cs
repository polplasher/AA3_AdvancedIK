using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class DroneMotor2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float deceleration = 30f;

    [Header("Physics")]
    [SerializeField] private float linearDragWhenNoInput = 6f;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void FixedUpdate()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input = Vector2.ClampMagnitude(input, 1f);

        Vector2 v = _rb.linearVelocity;

        if (input.sqrMagnitude > 0.0001f)
        {
            // Accelerate towards target speed
            Vector2 desired = input * maxSpeed;
            Vector2 delta = desired - v;

            Vector2 accel = Vector2.ClampMagnitude(delta, acceleration * Time.fixedDeltaTime);
            v += accel;

            _rb.linearDamping = 0f;
        }
        else
        {
            // Smooth braking
            float speed = v.magnitude;
            float drop = deceleration * Time.fixedDeltaTime;
            speed = Mathf.Max(0f, speed - drop);
            v = (v.sqrMagnitude > 0.0001f) ? v.normalized * speed : Vector2.zero;

            _rb.linearDamping = linearDragWhenNoInput;
        }

        _rb.linearVelocity = Vector2.ClampMagnitude(v, maxSpeed);
    }
}

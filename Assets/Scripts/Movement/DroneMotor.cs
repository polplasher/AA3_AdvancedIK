using UnityEngine;
using Utility;
using Vec2 = Utility.Vector2;
using Math = Utility.MathLite;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class DroneMotor : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float deceleration = 30f;

    [Header("Physics")]
    [SerializeField] private float linearDragWhenNoInput = 6f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void FixedUpdate()
    {
        Vec2 input = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input = Vec2.ClampMagnitude(input, 1f);

        Vec2 v = rb.linearVelocity.ToUtility();

        if (input.sqrMagnitude > 0.0001f)
        {
            // Accelerate towards target speed
            Vec2 desired = input * maxSpeed;
            Vec2 delta = desired - v;

            Vec2 accel = Vec2.ClampMagnitude(delta, acceleration * Time.fixedDeltaTime);
            v += accel;

            rb.linearDamping = 0f;
        }
        else
        {
            // Smooth braking
            float speed = v.magnitude;
            float drop = deceleration * Time.fixedDeltaTime;
            speed = Math.Max(0f, speed - drop);
            v = v.sqrMagnitude > 0.0001f ? v.normalized * speed : Vec2.zero;

            rb.linearDamping = linearDragWhenNoInput;
        }

        rb.linearVelocity = Vec2.ClampMagnitude(v, maxSpeed).ToUnity();
    }
}

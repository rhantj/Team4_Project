using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Joysticks")]
    [SerializeField] private Joystick fixedJoystick;
    [SerializeField] private DynamicJoystick dynamicJoystick;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        Vector2 input = fixedJoystick.Direction;
        if (input == Vector2.zero)
            input = dynamicJoystick.Direction;

        Vector3 moveDir = new Vector3(input.x, 0f, input.y) * moveSpeed;
        rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);

        if (input != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(input.x, 0f, input.y));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.15f);
        }

        // 애니메이션 - 움직이면 1, 멈추면 0
        animator?.SetFloat("Speed", input.magnitude);
    }
}
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Joystick")]
    [SerializeField] private Joystick joystick;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody rb;
    private Vector3 moveDirection;
    public bool IsMoving => moveDirection.sqrMagnitude > 0.01f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) return;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        GetInput();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GetInput()
    {
        if (joystick != null)
        {
            Vector2 input = joystick.Direction;
            moveDirection = new Vector3(input.x, 0f, input.y);
        }
    }

    private void Move()
    {
        if (rb == null) return;
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Vector3 movement = moveDirection * moveSpeed * Time.fixedDeltaTime;
            transform.position += movement;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;
      
        float speed = moveDirection.magnitude * moveSpeed;
        animator.SetFloat("Speed", speed);
    }
}
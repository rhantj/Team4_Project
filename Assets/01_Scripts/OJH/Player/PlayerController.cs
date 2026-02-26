using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Joysticks")]
    [SerializeField] private Joystick fixedJoystick;
    [SerializeField] private DynamicJoystick dynamicJoystick;

    private Rigidbody rb; // Rigidbody2D → Rigidbody

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector2 input = fixedJoystick.Direction;

        if (input == Vector2.zero)
            input = dynamicJoystick.Direction;

        // 3D 이동 (X, Z축으로 이동)
        Vector3 moveDir = new Vector3(input.x, 0f, input.y) * moveSpeed;
        rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);

        // 이동 방향으로 캐릭터 회전
        if (input != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(input.x, 0f, input.y));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.15f);
        }
    }
}

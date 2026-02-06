using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // 플레이어

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -10f); // 카메라 위치 오프셋
    [SerializeField] private float smoothSpeed = 5f; // 부드러운 정도 (높을수록 빠름)

    [Header("Rotation")]
    [SerializeField] private float lookAtHeight = 0f; // 플레이어의 어느 높이를 볼지

    private void LateUpdate()
    {
        if (target == null)
        {
     
            return;
        }

        // 목표 위치 = 플레이어 위치 + 오프셋
        Vector3 desiredPosition = target.position + offset;

        // 부드럽게 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // 플레이어 바라보기
        Vector3 lookAtTarget = target.position + Vector3.up * lookAtHeight;
        transform.LookAt(lookAtTarget);
    }
}
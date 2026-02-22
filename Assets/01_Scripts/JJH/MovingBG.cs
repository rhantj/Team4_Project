using UnityEngine;

public class MovingBG : MonoBehaviour
{
    [SerializeField] private float maxMoveDistance = 20f;
    [SerializeField] private float smoothTime = 0.3f;

    Vector3 startPos;
    Vector3 velocity = Vector3.zero;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        Vector3 targetPos = startPos + new Vector3(mouseX, mouseY, 0f) * maxMoveDistance;

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref velocity, smoothTime);
    }

}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DynamicJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Dynamic Joystick Components")]
    [SerializeField] private RectTransform dynamicJoystick;
    [SerializeField] private RectTransform dynamicBackground;
    [SerializeField] private RectTransform dynamicHandle;

    [Header("Fixed Joystick Reference")]
    [SerializeField] private RectTransform joystick;
    [SerializeField] private RectTransform fixedJoystickArea; // 고정 조이스틱 영역
    [SerializeField] private float fixedJoystickRadius = 150f; // 고정 조이스틱 반경

    [Header("Settings")]
    [SerializeField] private float handleRange = 50f;

    private Vector2 inputVector = Vector2.zero;
    private Canvas canvas;
    private Camera cam;
    private bool isActive = false;

    public Vector2 Direction => inputVector;
    public float Horizontal => inputVector.x;
    public float Vertical => inputVector.y;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            cam = canvas.worldCamera;
            if (cam == null)
            {
                cam = Camera.main;
            }
        }

        dynamicJoystick.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (fixedJoystickArea != null)
        {
            Vector2 fixedLocalPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                cam,
                out fixedLocalPoint
            );

            float distance = Vector2.Distance(fixedLocalPoint, fixedJoystickArea.anchoredPosition);

            // 고정 조이스틱 영역이면 무시
            if (distance <= fixedJoystickRadius)
            {
                return;
            }
        }

        //터치 위치를 Canvas 로컬 좌표로 변환
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            cam,
            out localPoint
        );

        // 동적 조이스틱 활성화
        // 고정 조이스틱 비활성화
        isActive = true;
        dynamicJoystick.gameObject.SetActive(true);
        joystick.gameObject.SetActive(false);
        dynamicBackground.anchoredPosition = localPoint;

        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isActive) return;

        Vector2 backgroundScreenPos = RectTransformUtility.WorldToScreenPoint(cam, dynamicBackground.position);
        Vector2 radius = dynamicBackground.sizeDelta / 2;
        inputVector = (eventData.position - backgroundScreenPos) / (radius * canvas.scaleFactor);

        if (inputVector.magnitude > 1f)
        {
            inputVector = inputVector.normalized;
        }

        dynamicHandle.anchoredPosition = inputVector * handleRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isActive) return;

        // 동적 조이스틱 비활성화
        // 고정 조이스틱 활성화
        inputVector = Vector2.zero;
        dynamicHandle.anchoredPosition = Vector2.zero;
        dynamicJoystick.gameObject.SetActive(false);
        joystick.gameObject.SetActive(true);
        isActive = false;
    }
}
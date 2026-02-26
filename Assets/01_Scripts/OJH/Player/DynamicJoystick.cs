using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Components")]
    [SerializeField] private RectTransform dynamicBackground;
    [SerializeField] private RectTransform dynamicHandle;

    [Header("Fixed Joystick 제외 영역")]
    [SerializeField] private RectTransform fixedJoystickBackground; // JoystickBackground 연결
    [SerializeField] private float fixedJoystickRadius = 100f;
    [Header("Fixed Joystick Reference")]
    [SerializeField] private GameObject fixedJoystick; // JoystickBackground 연결
    [Header("Settings")]
    [SerializeField] private float handleRange = 50f;



    private Vector2 inputVector = Vector2.zero;
    private Canvas canvas;
    private bool isActive = false;

    public Vector2 Direction => inputVector;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        dynamicBackground.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 고정 조이스틱 영역인지 체크 (스크린 좌표 기준)
        Vector2 fixedJoystickScreenPos = RectTransformUtility.WorldToScreenPoint(null, fixedJoystickBackground.position);
        float distance = Vector2.Distance(eventData.position, fixedJoystickScreenPos);
        float scaledRadius = fixedJoystickRadius * canvas.scaleFactor;

        // 고정 조이스틱 영역이면 무시
        if (distance <= scaledRadius) return;

        // 다이나믹 조이스틱 활성화
        isActive = true;
        dynamicBackground.gameObject.SetActive(true);
        fixedJoystick.SetActive(false); // 고정 조이스틱 비활성화 
        // 터치 위치에 배경 생성
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            null,
            out localPoint
        );
        dynamicBackground.anchoredPosition = localPoint;

        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isActive) return;

        Vector2 backgroundScreenPos = RectTransformUtility.WorldToScreenPoint(null, dynamicBackground.position);
        Vector2 radius = dynamicBackground.sizeDelta / 2;
        inputVector = (eventData.position - backgroundScreenPos) / (radius * canvas.scaleFactor);

        if (inputVector.magnitude > 1f)
            inputVector = inputVector.normalized;

        dynamicHandle.anchoredPosition = inputVector * handleRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isActive) return;
        isActive = false;
        inputVector = Vector2.zero;
        dynamicHandle.anchoredPosition = Vector2.zero;
        dynamicBackground.gameObject.SetActive(false);
        fixedJoystick.SetActive(true); // 고정 조이스틱 다시 활성화
    }
}
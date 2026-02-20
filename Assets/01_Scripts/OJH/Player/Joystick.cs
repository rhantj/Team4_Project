using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHandle;
    [SerializeField] private float handleRange = 100f;

    private Vector2 inputVector = Vector2.zero;
    private Canvas canvas;
    private Camera cam;

    public Vector2 Direction => inputVector;

    private void Start()
    {
        // Canvas 찾기
        canvas = GetComponentInParent<Canvas>();

        // Canvas RenderMode에 따라 카메라 설정
        if (canvas != null)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                cam = canvas.worldCamera;
            }
            else if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                cam = null;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = Vector2.zero;

        // RectTransformUtility로 정확한 로컬 포지션 계산
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground,
            eventData.position,
            cam, // Canvas 타입에 맞는 카메라 사용
            out position))
        {
            // 정규화 (-1 ~ 1 범위로)
            position.x = (position.x / joystickBackground.sizeDelta.x);
            position.y = (position.y / joystickBackground.sizeDelta.y);

            inputVector = new Vector2(position.x * 2, position.y * 2);
            inputVector = Vector2.ClampMagnitude(inputVector, 1f);

            // 핸들 위치 업데이트
            joystickHandle.anchoredPosition = new Vector2(
                inputVector.x * handleRange,
                inputVector.y * handleRange
            );
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
    }
}
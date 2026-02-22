using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Joystick Components")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    [Header("Settings")]
    [SerializeField] private float handleRange = 100f;
    [SerializeField] private bool fixedPosition = true;

    private Vector2 inputVector = Vector2.zero;
    private Vector2 joystickPosition = Vector2.zero;
    private Canvas canvas;
    private Camera cam;

    // 조이스틱 입력 값 반환
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
        }

        // 고정 위치가 아니면 처음에 숨김
        if (!fixedPosition)
        {
            background.gameObject.SetActive(false);
        }

        joystickPosition = background.anchoredPosition;
    }

    // 터치/클릭 시작
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!fixedPosition)
        {
            background.gameObject.SetActive(true);
            background.position = eventData.position;
        }

        OnDrag(eventData);
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta / 2;

        inputVector = (eventData.position - position) / (radius * canvas.scaleFactor);

        // 입력 값 제한
        if (inputVector.magnitude > 1f)
        {
            inputVector = inputVector.normalized;
        }

        // 핸들 위치 업데이트
        handle.anchoredPosition = inputVector * handleRange;
    }

    // 터치/클릭 종료
    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        if (!fixedPosition)
        {
            background.gameObject.SetActive(false);
            background.anchoredPosition = joystickPosition;
        }
    }
}
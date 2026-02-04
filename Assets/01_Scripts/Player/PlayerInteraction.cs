using UnityEngine;
using System;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask landmarkLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("UI References")]
    [SerializeField] private GameObject interactionPromptUI; // "F키로 건설" UI

    private Landmark nearbyLandmark = null;
    private Collider[] hitBuffer = new Collider[5];
    private float checkTimer = 0f;
    private const float CHECK_INTERVAL = 0.2f;

    // 이벤트
    public event Action<Landmark> OnLandmarkInteracted;

    private void Update()
    {
        CheckNearbyLandmark();
        HandleInput();
    }

    private void CheckNearbyLandmark()
    {
        checkTimer += Time.deltaTime;

        if (checkTimer < CHECK_INTERVAL)
            return;

        checkTimer = 0f;

        // 주변 랜드마크 검색
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            interactionRange,
            hitBuffer,
            landmarkLayer
        );

        Landmark previousLandmark = nearbyLandmark;

        // 가장 가까운 랜드마크 찾기
        if (hitCount > 0)
        {
            nearbyLandmark = FindClosestLandmark(hitCount);
        }
        else
        {
            nearbyLandmark = null;
        }

        // UI 상태 업데이트
        if (previousLandmark != nearbyLandmark)
        {
            UpdateInteractionUI();
        }
    }

    private Landmark FindClosestLandmark(int hitCount)
    {
        float closestDistance = float.MaxValue;
        Landmark closest = null;

        for (int i = 0; i < hitCount; i++)
        {
            Landmark landmark = hitBuffer[i].GetComponent<Landmark>();

            if (landmark != null && landmark.CanInteract())
            {
                float distance = Vector3.Distance(
                    transform.position,
                    hitBuffer[i].transform.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = landmark;
                }
            }
        }

        return closest;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(interactKey) && nearbyLandmark != null)
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (nearbyLandmark == null)
            return;



        // 이벤트 발생 (UI 매니저에서 구독)
        OnLandmarkInteracted?.Invoke(nearbyLandmark);

        // 또는 직접 UI 열기
        // UIManager.Instance.OpenLandmarkBuildUI(nearbyLandmark);
    }

    private void UpdateInteractionUI()
    {
        if (interactionPromptUI != null)
        {
            bool shouldShow = nearbyLandmark != null;
            interactionPromptUI.SetActive(shouldShow);

            // UI 위치를 랜드마크 위로 (선택)
            if (shouldShow)
            {
                UpdatePromptPosition();
            }
        }
    }

    private void UpdatePromptPosition()
    {
        if (nearbyLandmark == null || interactionPromptUI == null)
            return;

        // 랜드마크 위에 UI 표시
        Vector3 worldPos = nearbyLandmark.transform.position + Vector3.up * 3f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        interactionPromptUI.transform.position = screenPos;
    }

    private void OnDrawGizmosSelected()
    {
        // 상호작용 범위 시각화
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
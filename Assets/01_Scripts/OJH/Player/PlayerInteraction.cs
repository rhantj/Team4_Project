using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask landmarkLayer;
    [SerializeField] private LayerMask buildAreaLayer; // BuildArea 감지용
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("UI References")]
    [SerializeField] private GameObject interactionPromptUI;
    [SerializeField] private GameObject buildAreaPromptUI; // BuildArea용 UI
    [SerializeField] private TextMeshProUGUI buildProgressText; // "건설 중..." 표시

    [Header("Audio/Visual Feedback")]
    [SerializeField] private AudioClip interactSound;
    [SerializeField] private ParticleSystem interactEffect;

    private Inventory inventory;
    private Landmark nearbyLandmark = null;
    private BuildArea nearbyBuildArea = null; // BuildArea
    private Building nearbyBuilding = null; // Building

    private Collider[] hitBuffer = new Collider[5];
    private float checkTimer = 0f;
    private float lastInteractTime = 0f;
    [SerializeField] private float interactCooldown = 0.5f;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        CheckNearbyObjects(); // Landmark와 BuildArea 모두 체크
        HandleInput();
    }

    private void CheckNearbyObjects()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer < 0.2f)
            return;
        checkTimer = 0f;

        // Landmark 체크
        CheckNearbyLandmark();

        // BuildArea 체크
        CheckNearbyBuildArea();

        // Building 체크
        CheckNearbyBuilding();

        UpdateInteractionUI();
    }

    private void CheckNearbyLandmark()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            interactionRange,
            hitBuffer,
            landmarkLayer
        );

        if (hitCount > 0)
        {
            nearbyLandmark = FindClosestLandmark(hitCount);
        }
        else
        {
            nearbyLandmark = null;
        }
    }

    private void CheckNearbyBuildArea()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            interactionRange,
            hitBuffer,
            buildAreaLayer
        );

        if (hitCount > 0)
        {
            nearbyBuildArea = hitBuffer[0].GetComponent<BuildArea>();
        }
        else
        {
            nearbyBuildArea = null;
        }
    }

    private void CheckNearbyBuilding()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            interactionRange,
            hitBuffer,
            buildAreaLayer // Building도 같은 레이어 
        );

        if (hitCount > 0)
        {
            nearbyBuilding = hitBuffer[0].GetComponent<Building>();
        }
        else
        {
            nearbyBuilding = null;
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
        if (!Input.GetKeyDown(interactKey)) return;
        if (Time.time < lastInteractTime + interactCooldown) return;

        // 우선순위: Landmark > Building > BuildArea
        if (nearbyLandmark != null)
        {
            InteractWithLandmark();
        }
        else if (nearbyBuilding != null)
        {
            InteractWithBuilding();
        }

        lastInteractTime = Time.time;
    }

    private void InteractWithLandmark()
    {
        if (nearbyLandmark == null || inventory == null)
            return;

        bool success = nearbyLandmark.TryUpgrade(inventory);

        if (success)
        {
            PlayFeedback();
            Debug.Log("건설 성공!");
        }
        else
        {
            Debug.Log("건설 실패! 재료가 부족합니다.");
        }
    }

    private void InteractWithBuilding()
    {
        // Building의 InputItems 코루틴이 자동으로 실행됨
        // 플레이어가 영역에 있으면 아이템을 자동으로 전달
        Debug.Log("건설 중인 건물 착공 중...");
    }

    private void PlayFeedback()
    {
        if (interactSound != null)
            AudioSource.PlayClipAtPoint(interactSound, transform.position);

        if (interactEffect != null && nearbyLandmark != null)
            Instantiate(interactEffect, nearbyLandmark.transform.position, Quaternion.identity);
    }

    private void UpdateInteractionUI()
    {
        // Landmark 상호작용 UI
        if (interactionPromptUI != null)
        {
            bool shouldShow = nearbyLandmark != null;
            interactionPromptUI.SetActive(shouldShow);
        }

        // BuildArea UI (BuildArea 시스템 자동 처리)
        if (buildAreaPromptUI != null)
        {
            bool shouldShow = nearbyBuildArea != null;
            buildAreaPromptUI.SetActive(shouldShow);
        }

        // Building 진행 상태 표시
        if (buildProgressText != null && nearbyBuilding != null)
        {
            buildProgressText.gameObject.SetActive(true);
            buildProgressText.text = "건설 중... (F키로 재료 추가)";
        }
        else if (buildProgressText != null)
        {
            buildProgressText.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Landmark 범위
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        // BuildArea 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
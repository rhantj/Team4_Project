using UnityEngine;
using System.Collections;

public class ResourceNode : MonoBehaviour, ICollectable
{
    [Header("Resource Settings")]
    [SerializeField] private ResourceType resourceType = ResourceType.Wood;
    [SerializeField] private int resourceAmount = 1;
    [SerializeField] private float harvestTime = 1f;

    [Header("Harvest Limits")]
    [SerializeField] private int maxHarvestCount = 10;
    private int currentHarvestCount = 0;

    [Header("Item Data")]
    [SerializeField] private ResourceItemData itemData;

    [Header("Visual")]
    [SerializeField] private GameObject visualModel;
    [SerializeField] private ParticleSystem harvestEffect;

    [Header("UI Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private float respawnTime = 5f;

    [Header("UI References")]
    [SerializeField] private bool usePlayerUI = true; // 플레이어 UI 사용 여부
    [SerializeField] private bool useWorldSpaceUI = true; // 월드 스페이스 UI 사용 여부

    private ResourceNodeUI playerUI;
    private WorldSpaceResourceUI worldSpaceUI;

    private bool isBeingHarvested = false;
    private bool isDepleted = false;
    private Inventory currentInventory;
    private Transform playerTransform;
    private bool isPlayerNearby = false;

    private void Start()
    {
        // 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;

            // 플레이어 UI
            if (usePlayerUI)
            {
                playerUI = player.GetComponentInChildren<ResourceNodeUI>();
                if (playerUI == null)
                {
                    Debug.LogWarning("Player에 ResourceNodeUI가 없습니다!");
                }
            }
        }

        // 월드 스페이스 UI
        if (useWorldSpaceUI)
        {
            worldSpaceUI = GetComponentInChildren<WorldSpaceResourceUI>();
            if (worldSpaceUI == null)
            {
                Debug.LogWarning("WorldSpaceResourceUI가 없습니다!");
            }
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool wasNearby = isPlayerNearby;
        isPlayerNearby = distance <= interactionDistance && !isDepleted;

        // 플레이어가 범위에 들어왔을 때
        if (isPlayerNearby && !wasNearby)
        {
            // 플레이어 UI 표시
            if (usePlayerUI && playerUI != null)
            {
                playerUI.ShowResourceInfo(this);
                playerUI.ShowLoadingBar(harvestTime);
            }

            // 월드 스페이스 UI 표시
            if (useWorldSpaceUI && worldSpaceUI != null)
            {
                worldSpaceUI.ShowResourceInfo();
                worldSpaceUI.ShowLoadingBar(harvestTime);
            }
        }
        // 플레이어가 범위를 벗어났을 때
        else if (!isPlayerNearby && wasNearby)
        {
            // 플레이어 UI 숨김
            if (usePlayerUI && playerUI != null)
            {
                playerUI.HideUI();
            }

            // 월드 스페이스 UI 숨김
            if (useWorldSpaceUI && worldSpaceUI != null)
            {
                worldSpaceUI.HideUI();
            }
        }
    }

    public bool CanCollect()
    {
        return !isBeingHarvested && !isDepleted && currentHarvestCount < maxHarvestCount;
    }

    public void Collect()
    {
        if (currentInventory != null)
        {
            StartCoroutine(HarvestCoroutine(currentInventory));
        }
    }

    public ResourceData GetResourceData()
    {
        return new ResourceData
        {
            resourceType = resourceType,
            resourceName = itemData.m_SoItemName,
            amount = resourceAmount
        };
    }

    public void SetInventory(Inventory inventory)
    {
        currentInventory = inventory;
    }

    public string GetHarvestInfo()
    {
        return $"{currentHarvestCount}/{maxHarvestCount}";
    }

    public int GetRemainingHarvests()
    {
        return maxHarvestCount - currentHarvestCount;
    }

    public int GetMaxHarvestCount()
    {
        return maxHarvestCount;
    }

    private IEnumerator HarvestCoroutine(Inventory playerInventory)
    {
        isBeingHarvested = true;

        if (harvestEffect != null)
            harvestEffect.Play();

        // 전체 시간 계산
        float totalTime = (currentHarvestCount == 0) ? harvestTime : (respawnTime + harvestTime);

        // 로딩 바 업데이트
        float elapsedTime = 0f;
        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / totalTime;

            // 플레이어 UI 업데이트
            if (usePlayerUI && playerUI != null)
            {
                playerUI.UpdateLoadingBar(progress);
            }

            // 월드 스페이스 UI 업데이트
            if (useWorldSpaceUI && worldSpaceUI != null)
            {
                worldSpaceUI.UpdateLoadingBar(progress);
            }

            yield return null;
        }

        // 수집 완료!
        currentHarvestCount++;

        if (playerInventory != null && itemData != null)
        {
            for (int i = 0; i < resourceAmount; i++)
            {
                playerInventory.AddItem(itemData);
            }
        }

        // 수집 완료 후 로딩바 리셋 및 UI 업데이트
        if (usePlayerUI && playerUI != null)
        {
            playerUI.ResetLoadingBar();
            if (isPlayerNearby)
            {
                playerUI.UpdateUI();
            }
        }

        if (useWorldSpaceUI && worldSpaceUI != null)
        {
            worldSpaceUI.ResetLoadingBar();
            if (isPlayerNearby)
            {
                worldSpaceUI.UpdateUI();
            }
        }

        if (currentHarvestCount >= maxHarvestCount)
        {
            isDepleted = true;
            if (visualModel != null)
                visualModel.SetActive(false);

            // 고갈되었을 때 UI 숨김
            if (usePlayerUI && playerUI != null)
            {
                playerUI.HideUI();
            }

            if (useWorldSpaceUI && worldSpaceUI != null)
            {
                worldSpaceUI.HideUI();
            }
        }
        else
        {
            // 다음 수집 대기
            isBeingHarvested = false;

            if (visualModel != null)
                visualModel.SetActive(true);
        }
    }

    [ContextMenu("Reset Harvest Count")]
    private void ResetHarvestCount()
    {
        currentHarvestCount = 0;
        isDepleted = false;
        isBeingHarvested = false;
        if (visualModel != null)
            visualModel.SetActive(true);

        if (usePlayerUI && playerUI != null && isPlayerNearby)
        {
            playerUI.UpdateUI();
        }

        if (useWorldSpaceUI && worldSpaceUI != null && isPlayerNearby)
        {
            worldSpaceUI.UpdateUI();
        }
    }

    private void OnDestroy()
    {
        if (isPlayerNearby)
        {
            if (usePlayerUI && playerUI != null)
            {
                playerUI.HideUI();
            }

            if (useWorldSpaceUI && worldSpaceUI != null)
            {
                worldSpaceUI.HideUI();
            }
        }
    }
}
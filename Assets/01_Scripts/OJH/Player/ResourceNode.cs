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

    private bool isBeingHarvested = false;
    private bool isDepleted = false;
    private Inventory currentInventory;
    private Transform playerTransform;
    private ResourceNodeUI resourceUI;
    private bool isPlayerNearby = false;

    private void Start()
    {
        // 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;

            // 플레이어의 ResourceNodeUI 찾기
            resourceUI = player.GetComponentInChildren<ResourceNodeUI>();
            if (resourceUI == null)
            {
                Debug.LogWarning("Player에 ResourceNodeUI가 없습니다!");
            }
        }
    }

    private void Update()
    {
        if (playerTransform == null || resourceUI == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool wasNearby = isPlayerNearby;
        isPlayerNearby = distance <= interactionDistance && !isDepleted;

        // 플레이어가 범위에 들어왔을 때
        if (isPlayerNearby && !wasNearby)
        {
            resourceUI.ShowResourceInfo(this);
        }
        // 플레이어가 범위를 벗어났을 때
        else if (!isPlayerNearby && wasNearby)
        {
            resourceUI.HideUI();
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


        yield return new WaitForSeconds(harvestTime);

        currentHarvestCount++;

        // UI 업데이트
        if (resourceUI != null && isPlayerNearby)
        {
            resourceUI.UpdateUI();
        }


        if (playerInventory != null && itemData != null)
        {
            for (int i = 0; i < resourceAmount; i++)
            {
                playerInventory.AddItem(itemData);
            }
        }

        if (currentHarvestCount >= maxHarvestCount)
        {
            isDepleted = true;
            if (visualModel != null)
                visualModel.SetActive(false);

            // UI 숨기기
            if (resourceUI != null)
            {
                resourceUI.HideUI();
            }


        }
        else
        {
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        isBeingHarvested = false;

        if (visualModel != null)
            visualModel.SetActive(true);


    }

    [ContextMenu("Reset Harvest Count")]
    private void ResetHarvestCount()
    {
        currentHarvestCount = 0;
        isDepleted = false;
        isBeingHarvested = false;
        if (visualModel != null)
            visualModel.SetActive(true);

        if (resourceUI != null && isPlayerNearby)
        {
            resourceUI.UpdateUI();
        }


    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 UI 숨기기
        if (resourceUI != null && isPlayerNearby)
        {
            resourceUI.HideUI();
        }
    }
}
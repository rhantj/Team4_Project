using UnityEngine;
using System.Collections;

public class ResourceNode : MonoBehaviour, ICollectable
{
    private Coroutine harvestCoroutine;

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
    [SerializeField] private bool usePlayerUI = true;
    [SerializeField] private bool useWorldSpaceUI = true;

    private ResourceNodeUI playerUI;
    private WorldSpaceResourceUI worldSpaceUI;

    private bool isBeingHarvested = false;
    private bool isDepleted = false;
    private Inventory currentInventory;
    private Transform playerTransform;
    private bool isPlayerNearby = false;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;

            if (usePlayerUI)
            {
                playerUI = player.GetComponentInChildren<ResourceNodeUI>();
                if (playerUI == null)
                    Debug.LogWarning("Player에 ResourceNodeUI가 없습니다!");
            }
        }

        if (useWorldSpaceUI)
        {
            worldSpaceUI = GetComponentInChildren<WorldSpaceResourceUI>();
            if (worldSpaceUI == null)
                Debug.LogWarning("WorldSpaceResourceUI가 없습니다!");
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool wasNearby = isPlayerNearby;
        isPlayerNearby = distance <= interactionDistance && !isDepleted;

        if (isPlayerNearby && !wasNearby)
        {
            if (usePlayerUI && playerUI != null)
            {
                playerUI.ShowResourceInfo(this);
                playerUI.ShowLoadingBar(harvestTime);
            }

            if (useWorldSpaceUI && worldSpaceUI != null)
            {
                worldSpaceUI.ShowResourceInfo();
                worldSpaceUI.ShowLoadingBar(harvestTime);
            }
        }
        else if (!isPlayerNearby && wasNearby)
        {
            if (usePlayerUI && playerUI != null)
                playerUI.HideUI();

            if (useWorldSpaceUI && worldSpaceUI != null)
                worldSpaceUI.HideUI();
            CancelHarvest();
        }
    }
    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);

        currentHarvestCount = 0;
        isDepleted = false;
        isBeingHarvested = false;
        harvestCoroutine = null;

        if (visualModel != null)
            visualModel.SetActive(true);
    }
    public bool CanCollect()
    {
        return !isBeingHarvested && !isDepleted && currentHarvestCount < maxHarvestCount;
    }

    public bool IsBeingHarvested()
    {
        return isBeingHarvested;
    }

    public void Collect()
    {
        if (isBeingHarvested) // 이미 수집 중이면 무시
        {
            //Debug.Log("이미 수집 중 - Collect 무시!");
            return;
        }

        if (currentInventory != null)
        {
            isBeingHarvested = true;
            harvestCoroutine = StartCoroutine(HarvestCoroutine(currentInventory));
        }
    }
    public void CancelHarvest()
    {
        StopAllCoroutines();
        harvestCoroutine = null;
        isBeingHarvested = false;

        if (harvestEffect != null)
            harvestEffect.Stop();

        if (usePlayerUI && playerUI != null)
            playerUI.ResetLoadingBar();

        if (useWorldSpaceUI && worldSpaceUI != null)
            worldSpaceUI.ResetLoadingBar();

        //Debug.Log("수집 중단!");
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
        if (harvestEffect != null)
            harvestEffect.Play();

        float totalTime = (currentHarvestCount == 0) ? harvestTime : (respawnTime + harvestTime);
        float elapsedTime = 0f;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / totalTime;

            if (usePlayerUI && playerUI != null)
                playerUI.UpdateLoadingBar(progress);

            if (useWorldSpaceUI && worldSpaceUI != null)
                worldSpaceUI.UpdateLoadingBar(progress);

            yield return null;
        }

        // 수집 완료
        currentHarvestCount++;

        if (playerInventory != null && itemData != null)
        {
            for (int i = 0; i < resourceAmount; i++)
            {
                playerInventory.AddItem(itemData);
            }
        }

        if (usePlayerUI && playerUI != null)
        {
            playerUI.ResetLoadingBar();
            if (isPlayerNearby)
                playerUI.UpdateUI();
        }

        if (useWorldSpaceUI && worldSpaceUI != null)
        {
            worldSpaceUI.ResetLoadingBar();
            if (isPlayerNearby)
                worldSpaceUI.UpdateUI();
        }

        if (currentHarvestCount >= maxHarvestCount)
        {
            isDepleted = true;
            if (visualModel != null)
                visualModel.SetActive(false);

            if (usePlayerUI && playerUI != null)
                playerUI.HideUI();

            if (useWorldSpaceUI && worldSpaceUI != null)
                worldSpaceUI.HideUI();
            StartCoroutine(RespawnCoroutine()); 
        }
        else
        {
            isBeingHarvested = false;
            harvestCoroutine = null;

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
            playerUI.UpdateUI();

        if (useWorldSpaceUI && worldSpaceUI != null && isPlayerNearby)
            worldSpaceUI.UpdateUI();
    }

    private void OnDestroy()
    {
        if (isPlayerNearby)
        {
            if (usePlayerUI && playerUI != null)
                playerUI.HideUI();

            if (useWorldSpaceUI && worldSpaceUI != null)
                worldSpaceUI.HideUI();
        }
    }
}
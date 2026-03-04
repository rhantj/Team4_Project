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
    [SerializeField] private float interactionDistance = 5f;
    [SerializeField] private float respawnTime = 5f;

    [Header("UI References")]
    [SerializeField] private bool usePlayerUI = true;
    [SerializeField] private bool useWorldSpaceUI = true;


    [Header("Range Indicator")]
    [SerializeField] private bool showRangeCircle = true;
    [SerializeField] private Color rangeColor = new Color(1f, 1f, 0f, 0.5f);
    [SerializeField] private int circleSegments = 36;

    private LineRenderer rangeCircle;


    private ResourceNodeUI playerUI;

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
            currentInventory = player.GetComponent<Inventory>();

            // collectionRange 동기화
            var collector = player.GetComponent<PlayerResourceCollector>();
            if (collector != null)
                interactionDistance = collector.CollectionRange;

            if (usePlayerUI)
            {
                playerUI = player.GetComponentInChildren<ResourceNodeUI>();
                if (playerUI == null)
                    Debug.LogWarning("Player에 ResourceNodeUI가 없습니다!");
            }
        }

    

        if (showRangeCircle)
            CreateRangeCircle();
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool wasNearby = isPlayerNearby;
        isPlayerNearby = distance <= interactionDistance && !isDepleted;

        if (isPlayerNearby && !wasNearby)
        {
            bool isFull = currentInventory != null && currentInventory.IsFull; // 수정

            if (usePlayerUI && playerUI != null)
            {
                playerUI.ShowResourceInfo(this);
                if (!isFull)
                    playerUI.ShowLoadingBar(harvestTime);
            }

        }
        else if (!isPlayerNearby && wasNearby)
        {
            if (usePlayerUI && playerUI != null)
                playerUI.HideUI();

       
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
        if (currentInventory != null)
        {
            if (currentInventory.IsFull) return false; // IsFull만
        }
        return !isBeingHarvested && !isDepleted && currentHarvestCount < maxHarvestCount;
    }
    public bool IsBeingHarvested()
    {
        return isBeingHarvested;
    }

    public void Collect()
    {
        if (isBeingHarvested) return;

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

    public ResourceItemData GetItemData() => itemData; // 추가

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

    float elapsedTime = 0f;

    while (elapsedTime < harvestTime)
    {
        elapsedTime += Time.deltaTime;
        float progress = elapsedTime / harvestTime;

        if (usePlayerUI && playerUI != null)
            playerUI.UpdateLoadingBar(progress);

 

        yield return null;
    }
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


        if (currentHarvestCount >= maxHarvestCount)
        {
            isDepleted = true;
            if (visualModel != null)
                visualModel.SetActive(false);

            if (usePlayerUI && playerUI != null)
                playerUI.HideUI();

    
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

    }

    private void OnDestroy()
    {
        if (isPlayerNearby)
        {
            if (usePlayerUI && playerUI != null)
                playerUI.HideUI();


        }
    }
    private void CreateRangeCircle()
    {
        GameObject circleObj = new GameObject("RangeCircle");
        circleObj.transform.SetParent(transform);
        circleObj.transform.localPosition = Vector3.zero;

        rangeCircle = circleObj.AddComponent<LineRenderer>();
        rangeCircle.loop = true;
        rangeCircle.useWorldSpace = false;
        rangeCircle.widthMultiplier = 0.05f;
        rangeCircle.positionCount = circleSegments;

        // 머티리얼 설정
        rangeCircle.material = new Material(Shader.Find("Sprites/Default"));
        rangeCircle.startColor = rangeColor;
        rangeCircle.endColor = rangeColor;

        // 원 그리기
        for (int i = 0; i < circleSegments; i++)
        {
            float angle = 2f * Mathf.PI * i / circleSegments;
            float x = Mathf.Cos(angle) * interactionDistance;
            float z = Mathf.Sin(angle) * interactionDistance;
            rangeCircle.SetPosition(i, new Vector3(x, 0.05f, z));
        }
    }
}
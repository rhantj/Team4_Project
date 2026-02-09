using UnityEngine;
using System.Collections;

public class ResourceNode : MonoBehaviour, ICollectable // ICollectable
{
    [Header("Resource Settings")]
    [SerializeField] private ResourceType resourceType = ResourceType.Wood;
    [SerializeField] private int resourceAmount = 3;
    [SerializeField] private float harvestTime = 2f;

    [Header("Item Data")]
    [SerializeField] private ResourceItemData itemData;

    [Header("Visual")]
    [SerializeField] private GameObject visualModel;
    [SerializeField] private ParticleSystem harvestEffect;

    private bool isBeingHarvested = false;
    private bool isDepleted = false;

    private Inventory currentInventory; // 현재 채집 중인 플레이어


    public bool CanCollect()
    {
        return !isBeingHarvested && !isDepleted;
    }

    public void Collect()
    {
        // StartHarvest 대신 Collect 호출
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

    // 외부에서 인벤토리 설정
    public void SetInventory(Inventory inventory)
    {
        currentInventory = inventory;
    }

    private IEnumerator HarvestCoroutine(Inventory playerInventory)
    {
        isBeingHarvested = true;

        if (harvestEffect != null)
            harvestEffect.Play();

        Debug.Log(itemData.m_SoItemName + " 채집 중...");

        yield return new WaitForSeconds(harvestTime);

        Debug.Log(itemData.m_SoItemName + " x" + resourceAmount + " 획득!");

        if (playerInventory != null && itemData != null)
        {
            for (int i = 0; i < resourceAmount; i++)
            {
                playerInventory.AddItem(itemData);
            }
        }

        isDepleted = true;

        if (visualModel != null)
            visualModel.SetActive(false);

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        isDepleted = false;
        isBeingHarvested = false;

        if (visualModel != null)
            visualModel.SetActive(true);

        Debug.Log(itemData.m_SoItemName + " 리스폰!");
    }
}
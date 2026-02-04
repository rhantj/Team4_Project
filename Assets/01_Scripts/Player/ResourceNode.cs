using UnityEngine;
using System.Collections;

public class ResourceNode : MonoBehaviour
{
    [Header("Resource Settings")]
    [SerializeField] private ResourceType resourceType = ResourceType.Wood;
    [SerializeField] private string resourceName = "나무";
    [SerializeField] private int resourceAmount = 3;
    [SerializeField] private float harvestTime = 2f;

    [Header("Visual")]
    [SerializeField] private GameObject visualModel;
    [SerializeField] private ParticleSystem harvestEffect;

    private bool isBeingHarvested = false;
    private bool isDepleted = false;

    public bool CanHarvest => !isBeingHarvested && !isDepleted;
    public ResourceType ResourceType => resourceType;

    public void StartHarvest(ResourceStack playerStack) // 파라미터 추가!
    {
        if (!CanHarvest) return;
        StartCoroutine(HarvestCoroutine(playerStack));
    }

    private IEnumerator HarvestCoroutine(ResourceStack playerStack)
    {
        isBeingHarvested = true;

        if (harvestEffect != null)
            harvestEffect.Play();

        

        yield return new WaitForSeconds(harvestTime);



        // 플레이어 등에 나무 쌓기!
        if (playerStack != null && resourceType == ResourceType.Wood)
        {
            playerStack.AddWood(resourceAmount);
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


    }

    public ResourceData GetResourceData()
    {
        return new ResourceData
        {
            resourceType = resourceType,
            resourceName = resourceName,
            amount = resourceAmount
        };
    }
}
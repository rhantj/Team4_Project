using UnityEngine;
using System.Collections.Generic;

public class Landmark : MonoBehaviour
{
    [Header("Landmark Info")]
    [SerializeField] private string landmarkName = "경복궁";
    [SerializeField] private int maxLevel = 5;
    [SerializeField] private int currentLevel = 0;

    [Header("Visual")]
    [SerializeField] private List<GameObject> levelModels; // 레벨별 모델

    [Header("Build Requirements")]
    [SerializeField] private List<LandmarkLevel> levelRequirements;

    public string LandmarkName => landmarkName;
    public int CurrentLevel => currentLevel;
    public int MaxLevel => maxLevel;
    public bool IsComplete => currentLevel >= maxLevel;

    private void Start()
    {
        UpdateVisual();
    }

    public bool CanInteract()
    {
        // 완성되지 않은 랜드마크만 상호작용 가능
        return !IsComplete;
    }

    public bool CanUpgrade()
    {
        if (IsComplete)
            return false;

        // 다음 레벨 건설 가능 여부 체크
        LandmarkLevel nextLevel = levelRequirements[currentLevel];

        // 인벤토리 체크 (나중에 구현)
        // return InventoryManager.Instance.HasResources(nextLevel.requirements);

        return true;
    }

    public void Upgrade()
    {
        if (!CanUpgrade())
        {
    
            return;
        }

        LandmarkLevel nextLevel = levelRequirements[currentLevel];

        // 자원 소모 (나중에 구현)
        // InventoryManager.Instance.ConsumeResources(nextLevel.requirements);

        currentLevel++;
        UpdateVisual();

   

        if (IsComplete)
        {
            OnLandmarkComplete();
        }
    }

    private void UpdateVisual()
    {
        // 모든 모델 끄기
        foreach (var model in levelModels)
        {
            if (model != null)
                model.SetActive(false);
        }

        // 현재 레벨 모델만 켜기
        if (currentLevel > 0 && currentLevel <= levelModels.Count)
        {
            levelModels[currentLevel - 1].SetActive(true);
        }
    }

    private void OnLandmarkComplete()
    {
    

        // 완성 이벤트 발생
        // GameManager.Instance.OnLandmarkComplete(this);
    }

    public LandmarkLevel GetCurrentRequirements()
    {
        if (currentLevel >= levelRequirements.Count)
            return null;

        return levelRequirements[currentLevel];
    }
}

[System.Serializable]
public class LandmarkLevel
{
    public int level;
    public string description;
    public List<ResourceRequirement> requirements;
}

[System.Serializable]
public class ResourceRequirement
{
    public ResourceType resourceType;
    public int amount;
}
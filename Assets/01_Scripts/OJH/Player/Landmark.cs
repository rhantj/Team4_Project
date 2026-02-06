using UnityEngine;
using System.Collections.Generic;

public class Landmark : MonoBehaviour
{
    [Header("Landmark Info")]
    [SerializeField] private string landmarkName = "경복궁";
    [SerializeField] private int maxLevel = 5;
    [SerializeField] private int currentLevel = 0;

    [Header("Level Models")]
    [SerializeField] private List<GameObject> levelModels;

    [Header("Upgrade Requirements")]
    [SerializeField] private ResourceItemData requiredItemData; // 필요한 아이템
    [SerializeField] private int itemsPerLevel = 5; // 레벨당 필요 개수

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
        return !IsComplete;
    }

    // F키 눌렀을 때
    public bool TryUpgrade(Inventory playerInventory)
    {
        if (IsComplete)
        {
            Debug.Log(landmarkName + " 이미 완성됨!");
            return false;
        }

        // 인벤토리에 필요한 아이템이 있는지 체크
        int itemCount = CountItemsInInventory(playerInventory);

        if (itemCount < itemsPerLevel)
        {
            Debug.Log($"아이템 부족! 필요: {itemsPerLevel}, 현재: {itemCount}");
            return false;
        }

        // RemoveItem 메서드 사용! 
        for (int i = 0; i < itemsPerLevel; i++)
        {
            playerInventory.RemoveItem(requiredItemData);
        }

        currentLevel++;
        UpdateVisual();

        Debug.Log($"{landmarkName} Lv.{currentLevel} 완성!");

        if (IsComplete)
        {
            OnLandmarkComplete();
        }

        return true;
    }

    // 인벤토리에서 특정 아이템 개수 세기
    private int CountItemsInInventory(Inventory inventory)
    {
        int count = 0;

        // Reflection으로 private 필드 접근
        var field = typeof(Inventory).GetField("m_StackItem",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            List<GameObject> stackItem = (List<GameObject>)field.GetValue(inventory);

            foreach (GameObject item in stackItem)
            {
                if (item.name == requiredItemData.m_ItemName)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void UpdateVisual()
    {
        foreach (var model in levelModels)
        {
            if (model != null)
                model.SetActive(false);
        }

        if (currentLevel > 0 && currentLevel <= levelModels.Count)
        {
            levelModels[currentLevel - 1].SetActive(true);
        }
    }

    private void OnLandmarkComplete()
    {
        Debug.Log($" {landmarkName} 완성! 스테이지 클리어!");
    }

    public int GetRequiredItems()
    {
        if (IsComplete) return 0;
        return itemsPerLevel;
    }

    public float GetProgress()
    {
        return (float)currentLevel / maxLevel;
    }
}
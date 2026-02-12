using UnityEngine;
using TMPro;

public class ResourceNodeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI resourceNameText;
    [SerializeField] private TextMeshProUGUI countText;

    private ResourceNode currentNode;

    private void Start()
    {
        HideUI();
    }

    public void ShowResourceInfo(ResourceNode node)
    {
        if (node == null) return;

        currentNode = node;
        uiPanel.SetActive(true);

        ResourceData data = node.GetResourceData();

        // 자원 타입에 따라 이름 변경
        string displayName = data.resourceName;
        if (data.resourceType == ResourceType.Wood) // Log일 때
        {
            displayName = "Tree";
        }

        resourceNameText.text = displayName;

        int remaining = node.GetRemainingHarvests();
        int max = node.GetMaxHarvestCount();
        countText.text = $"{max-remaining}/{max}";

        // 색상 변경
        if (remaining == 0)
        {
            countText.color = Color.red;
        }
        else if (remaining <= max / 2)
        {
            countText.color = Color.yellow;
        }
        else
        {
            countText.color = Color.green;
        }
    }

    public void HideUI()
    {
        currentNode = null;
        uiPanel.SetActive(false);
    }

    public void UpdateUI()
    {
        if (currentNode != null)
        {
            ShowResourceInfo(currentNode);
        }
    }
}
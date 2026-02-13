using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceNodeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI resourceNameText;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("Loading Bar")]
    [SerializeField] private Slider loadingSlider;

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

        string displayName = data.resourceName;
        if (data.resourceType == ResourceType.Wood)
        {
            displayName = "Tree";
        }

        resourceNameText.text = displayName;

        int remaining = node.GetRemainingHarvests();
        int max = node.GetMaxHarvestCount();
        countText.text = $"{max - remaining}/{max}";

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
        HideLoadingBar();
    }

    public void UpdateUI()
    {
        if (currentNode != null)
        {
            ShowResourceInfo(currentNode);
        }
    }

    public void ShowLoadingBar(float duration)
    {
        if (loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(true);
            loadingSlider.value = 0f;
        }
    }

    public void ResetLoadingBar()
    {
        if (loadingSlider != null)
        {
            loadingSlider.value = 0f;
        }
    }

    public void UpdateLoadingBar(float progress)
    {
        if (loadingSlider != null)
        {
            loadingSlider.value = progress;
        }
    }

    public void HideLoadingBar()
    {
        if (loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(false);
        }
    }
}
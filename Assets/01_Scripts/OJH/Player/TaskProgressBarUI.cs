using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskProgressBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI resourceNameText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Slider progressSlider;

    private ResourceNode currentNode;
    private Inventory playerInventory;

    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerInventory = player.GetComponent<Inventory>();

        uiPanel.SetActive(false);
    }

    public void ShowResourceInfo(ResourceNode node)
    {
        if (node == null) return;
        currentNode = node;
        uiPanel.SetActive(true);

        ResourceData data = node.GetResourceData();
        string displayName = data.resourceType == ResourceType.Wood ? "Tree" : data.resourceName;
        resourceNameText.text = displayName;

        UpdateCount();

        int max = playerInventory != null ? playerInventory.m_Capacity : 0;
        int count = playerInventory != null ? playerInventory.m_ItemCount : 0;

        if (count >= max)
            HideLoadingBar();
        else
            ShowLoadingBar();
    }

    public void UpdateUI()
    {
        if (currentNode != null)
            ShowResourceInfo(currentNode);
    }

    private void UpdateCount()
    {
        int max = playerInventory != null ? playerInventory.m_Capacity : 0;
        int count = playerInventory != null ? playerInventory.m_ItemCount : 0;

        countText.text = $"{count}/{max}";

        if (count >= max)
            countText.color = Color.red;
        else if (count >= max / 2)
            countText.color = Color.yellow;
        else
            countText.color = Color.blue;
    }

    public void ShowLoadingBar()
    {
        if (progressSlider != null)
        {
            progressSlider.value = 0f;
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.gameObject.SetActive(true);
        }
    }

    public void UpdateProgress(float progress)
    {
        if (progressSlider != null)
            progressSlider.value = Mathf.Clamp01(progress);
    }

    public void ResetLoadingBar()
    {
        if (progressSlider != null)
            progressSlider.value = 0f;
    }

    public void HideLoadingBar()
    {
        if (progressSlider != null)
            progressSlider.gameObject.SetActive(false);
    }

    public void HideUI()
    {
        currentNode = null;
        uiPanel.SetActive(false);
        HideLoadingBar();
    }

    public bool IsVisible => uiPanel.activeSelf;
}
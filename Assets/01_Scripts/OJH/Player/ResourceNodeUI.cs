using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceNodeUI : MonoBehaviour
{
  
    [Header("UI References")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI resourceNameText;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("Loading Bar")]
    [SerializeField] private Slider loadingSlider;

    [Header("Follow Target")]
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);
    [SerializeField] private float updateInterval = 0.05f;

    private Camera mainCamera;
    private Inventory playerInventory;
    private ResourceNode currentNode;
    private RectTransform uiPanelRect;
    private float nextUpdateTime;

    private void Start()
    {
        mainCamera = Camera.main;
        uiPanelRect = uiPanel.GetComponent<RectTransform>();
        HideUI();

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<Inventory>();
            if (followTarget == null)
                followTarget = player.transform;
        }
    }

    private void LateUpdate()
    {
        if (!uiPanel.activeSelf || followTarget == null || mainCamera == null) return;
        if (Time.time < nextUpdateTime) return;
        nextUpdateTime = Time.time + updateInterval;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(followTarget.position + offset);

        if (screenPos.z < 0)
        {
            uiPanel.SetActive(false);
            return;
        }

        uiPanelRect.position = screenPos;
    }
    public void ShowResourceInfo(ResourceNode node)
    {
        if (node == null) return;
        currentNode = node;
        uiPanel.SetActive(true);
        ResourceData data = node.GetResourceData();
        string displayName = data.resourceName;
        if (data.resourceType == ResourceType.Wood)
            displayName = "Tree";
        resourceNameText.text = displayName;

        // 수정: 노드별 max → 인벤토리 전체 용량
        int max = playerInventory != null ? playerInventory.m_Capacity : 0;
        int invenCount = playerInventory != null ? playerInventory.m_ItemCount : 0;
        countText.text = $"{invenCount}/{max}";

        if (invenCount >= max)
        {
            countText.color = Color.red;
            HideLoadingBar();
        }
        else if (invenCount >= max / 2)
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
            ShowResourceInfo(currentNode);
    }

    public void ShowLoadingBar(float duration)
    {
        if (loadingSlider != null)
        {
            loadingSlider.value = 0f;
            loadingSlider.minValue = 0f;
            loadingSlider.maxValue = 1f;
            loadingSlider.gameObject.SetActive(true);
        }
    }

    public void ResetLoadingBar()
    {
        if (loadingSlider != null)
            loadingSlider.value = 0f;
    }

    public void UpdateLoadingBar(float progress)
    {
        if (loadingSlider != null)
            loadingSlider.value = progress;
    }

    public void HideLoadingBar()
    {
        if (loadingSlider != null)
            loadingSlider.gameObject.SetActive(false);
    }
}
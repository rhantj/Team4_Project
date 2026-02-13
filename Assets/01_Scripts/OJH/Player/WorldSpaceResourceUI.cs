using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldSpaceResourceUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI resourceNameText;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("Loading Bar")]
    [SerializeField] private Slider loadingSlider;

    [Header("Settings")]
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 2.5f, 0);

    private ResourceNode parentNode;
    private Camera mainCamera;
    private Transform parentTransform; // 부모(나무) Transform

    private void Start()
    {
        mainCamera = Camera.main;
        parentNode = GetComponentInParent<ResourceNode>();

        if (parentNode == null)
        {
            Debug.LogError("WorldSpaceResourceUI: parentNode를 찾을 수 없습니다!");
        }
        else
        {
            parentTransform = parentNode.transform;
        }

        // Canvas 설정
        if (worldCanvas != null)
        {
            worldCanvas.renderMode = RenderMode.WorldSpace;
            worldCanvas.worldCamera = mainCamera;
        }
        else
        {
            Debug.LogError("WorldSpaceResourceUI: worldCanvas가 할당되지 않았습니다!");
        }

        HideUI();
    }

    private void LateUpdate()
    {
        if (mainCamera == null || worldCanvas == null || !uiPanel.activeSelf)
            return;

        // UI가 항상 카메라를 바라보도록만 처리
        worldCanvas.transform.LookAt(worldCanvas.transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }

    public void ShowResourceInfo()
    {
        if (parentNode == null)
        {
            Debug.LogError("WorldSpaceResourceUI: parentNode가 null입니다!");
            return;
        }

        if (uiPanel == null)
        {
            Debug.LogError("WorldSpaceResourceUI: uiPanel이 null입니다!");
            return;
        }

        uiPanel.SetActive(true);

        ResourceData data = parentNode.GetResourceData();


        string displayName = data.resourceName;
        if (data.resourceType == ResourceType.Wood)
        {
            displayName = "Tree";
        }

        if (resourceNameText != null)
        {
            resourceNameText.text = displayName;
        }

        int remaining = parentNode.GetRemainingHarvests();
        int max = parentNode.GetMaxHarvestCount();

        if (countText != null)
        {
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
    }

    public void HideUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
        HideLoadingBar();
    }

    public void UpdateUI()
    {
        ShowResourceInfo();
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
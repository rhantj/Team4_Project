using UnityEngine;
using UnityEngine.UI;

public class PanelToggleButton : MonoBehaviour
{
    [SerializeField] private GameObject targetPanel; // 각자의 I_LeftText
    [SerializeField] private Button toggleButton;    // 각자의 Button
    [SerializeField] private RectTransform arrowIcon; // 각자의 화살표 이미지
    [SerializeField] private bool isLeftArrow = true; // L이면 true, R이면 false
    [SerializeField] private bool startOpen = true;

    private bool isOpen;

    private void Start()
    {
        isOpen = startOpen;
        targetPanel.SetActive(isOpen);
        UpdateArrow();
        toggleButton.onClick.AddListener(OnToggleClicked);
    }

    private void OnToggleClicked()
    {
        isOpen = !isOpen;
        targetPanel.SetActive(isOpen);
        UpdateArrow();
    }

    private void UpdateArrow()
    {
       
        arrowIcon.localRotation = Quaternion.Euler(0f, 0f, isOpen ? 0f : 180f);
    }
}
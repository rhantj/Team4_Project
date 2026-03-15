using TMPro;
using UnityEngine;

public class PickupAreaControlView : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Text;
    PickupAreaControllerViewModel m_ViewModel;

    private void Awake()
    {
        m_ViewModel = GetComponentInParent<PickupAreaControllerViewModel>();
    }

    private void OnEnable()
    {
        m_ViewModel.UpgradeStatusText.Subscribe(SetText);
    }

    private void OnDisable()
    {
        m_ViewModel.UpgradeStatusText.Unsubscribe(SetText);
    }

    private void SetText(string str) => m_Text.text = str;
}
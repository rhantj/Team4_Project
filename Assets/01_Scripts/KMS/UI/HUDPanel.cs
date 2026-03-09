using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDPanel : UIPanel
{
    [Header("Inventory")]
    [SerializeField] InventoryExpended m_Inv;
    [SerializeField] private TMP_Text m_GoldText;
    [SerializeField] private TMP_Text m_InvCountText;

    [Header("Options")]
    [SerializeField] private Button m_OptionBtn;
    [SerializeField] private GameObject m_OptionPanel;

    private void OnEnable()
    {
        m_Inv.m_OnGoldChanged += UpdateGoldText;
        m_Inv.m_OnInventoryCountChanged += UpdateCountText;

        m_OptionBtn.onClick.AddListener(OptionButtonClicked);
    }

    private void OnDisable()
    {
        m_Inv.m_OnGoldChanged -= UpdateGoldText;
        m_Inv.m_OnInventoryCountChanged -= UpdateCountText;

        m_OptionBtn.onClick.RemoveAllListeners();
    }

    private void UpdateGoldText(int gold)
    {
        m_GoldText.text = $"{gold}";
    }

    private void UpdateCountText(int cnt)
    {
        m_InvCountText.text = $"{cnt} / 10";
    }

    private void OptionButtonClicked()
    {
        m_OptionPanel.SetActive(true);
    }
}
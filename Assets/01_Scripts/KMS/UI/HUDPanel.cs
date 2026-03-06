using TMPro;
using UnityEngine;

public class HUDPanel : UIPanel
{
    [SerializeField] InventoryExpended m_Inv;
    [SerializeField] private TMP_Text m_GoldText;
    [SerializeField] private TMP_Text m_InvCountText;

    private void OnEnable()
    {
        m_Inv.m_OnGoldChanged += UpdateGoldText;
        m_Inv.m_OnInventoryCountChanged += UpdateCountText;
    }

    private void OnDisable()
    {
        m_Inv.m_OnGoldChanged -= UpdateGoldText;
    }

    private void UpdateGoldText(int gold)
    {
        m_GoldText.text = $"{gold}";
    }

    private void UpdateCountText(int cnt)
    {
        m_InvCountText.text = $"{cnt} / 10";
    } 
}
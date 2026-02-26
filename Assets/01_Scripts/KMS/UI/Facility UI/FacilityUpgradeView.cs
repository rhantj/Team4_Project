using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FacilityUpgradeView : MonoBehaviour, IBindable<UpgradeVM>
{
    [SerializeField] private TMP_Text m_Text;
    [SerializeField] private float m_Fill;
    [SerializeField] private bool m_IsUpgraded;

    private UpgradeVM m_UpgradeVM;

    public void Bind(UpgradeVM vm)
    {
        Unbind();

        m_UpgradeVM = vm;
        m_UpgradeVM.OnTextChanged += SetText;
        m_UpgradeVM.OnFillChanged += SetFill;
        m_UpgradeVM.OnUpgradeChanged += SetUpgraded;
    }

    public void Unbind()
    {
        if (m_UpgradeVM == null) return;
        m_UpgradeVM.OnTextChanged -= SetText;
        m_UpgradeVM.OnFillChanged -= SetFill;
        m_UpgradeVM.OnUpgradeChanged -= SetUpgraded;
        m_UpgradeVM = null;
    }

    private void SetText(string str) => m_Text.text = str;
    private void SetFill(float fill) => m_Fill = fill;
    private void SetUpgraded(bool up) => m_IsUpgraded = up;
}
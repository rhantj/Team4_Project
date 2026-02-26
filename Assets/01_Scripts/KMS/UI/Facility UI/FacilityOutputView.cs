using TMPro;
using UnityEngine;

public class FacilityOutputView : MonoBehaviour, IBindable<OutputVM>
{
    [SerializeField] TMP_Text m_Text;
    private OutputVM m_OutputVM;

    public void Bind(OutputVM vm)
    {
        Unbind();

        m_OutputVM = vm;
        m_OutputVM.OnTextChanged += SetText;
    }

    public void Unbind()
    {
        if (m_OutputVM == null) return;
        m_OutputVM.OnTextChanged -= SetText;
        m_OutputVM = null;
    }

    private void OnDisable()
    {
        Unbind();
    }

    private void SetText(string str) => m_Text.text = str;
}
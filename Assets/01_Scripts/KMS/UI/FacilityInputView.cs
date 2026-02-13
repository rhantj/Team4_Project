using UnityEngine;
using TMPro;

public class FacilityInputView : MonoBehaviour, IBindable<InputVM>
{
    [SerializeField] TMP_Text m_Text;
    private InputVM m_InputVM;

    public void Bind(InputVM vm)
    {
        Unbind();

        m_InputVM = vm;
        m_InputVM.OnTextChanged += SetText;
    }

    public void Unbind()
    {
        if (m_InputVM == null) return;
        m_InputVM.OnTextChanged -= SetText;
        m_InputVM = null;
    }

    private void OnDisable()
    {
        Unbind();
    }

    private void SetText(string str) => m_Text.text = str;
}
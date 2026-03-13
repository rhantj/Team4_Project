using TMPro;
using UnityEngine;

public class ProgressDescriptionView : MonoBehaviour, IBindable<ProgressDescriptionVM>
{
    [SerializeField] TMP_Text m_Text;
    private ProgressDescriptionVM m_VM;

    public void Bind(ProgressDescriptionVM vm)
    {
        m_VM = vm;
        m_VM.OnTextChanged += SetText;

        m_VM.Refresh(false);
    }


    public void Unbind()
    {
        if (m_VM == null) return;
        m_VM.OnTextChanged -= SetText;
        m_VM = null;
    }

    private void SetText(string str)=> m_Text.text = str;
}
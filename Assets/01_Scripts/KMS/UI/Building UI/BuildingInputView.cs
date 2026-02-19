using UnityEngine;
using TMPro;

public class BuildingInputView : MonoBehaviour, IBindable<BuildingInputVM>
{
    [SerializeField] TMP_Text m_Text;
    private BuildingInputVM m_VM;

    public void Bind(BuildingInputVM vm)
    {
        Unbind();

        m_VM = vm;
        m_VM.OnTextChanged += SetText;
    }

    public void Unbind()
    {
        if (m_VM == null) return;
        m_VM.OnTextChanged -= SetText;
        m_VM = null;
    }

    private void OnDisable()
    {
        Unbind();
    }

    private void SetText(string str) => m_Text.text = str;
}

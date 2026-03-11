using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuildingInputView : MonoBehaviour, IBindable<BuildingInputVM>
{
    [SerializeField] TMP_Text m_Text;
    [SerializeField] Image m_Image;
    private BuildingInputVM m_VM;

    public void Bind(BuildingInputVM vm)
    {
        Unbind();

        m_VM = vm;
        m_VM.OnTextChanged += SetText;
        m_VM.OnSpriteChanged += SetSprite;

        m_VM.Refresh();
    }

    public void Unbind()
    {
        if (m_VM == null) return;
        m_VM.OnTextChanged -= SetText;
        m_VM.OnSpriteChanged -= SetSprite;
        m_VM = null;
    }

    private void OnDisable()
    {
        Unbind();
    }

    private void SetText(string str) => m_Text.text = str;
    private void SetSprite(Sprite sp)
    {
        if(m_Image != null)
            m_Image.sprite = sp;
    }
}

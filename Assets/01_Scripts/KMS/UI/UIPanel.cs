using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    protected bool m_IsOpen;
    public bool IsOpen
    {
        get { return m_IsOpen; }
        set
        {
            m_IsOpen = value;
            gameObject.SetActive(m_IsOpen);
        }
    }

    private CanvasGroup m_CG;

    protected virtual void Awake()
    {
        m_CG = GetComponent<CanvasGroup>();
    }

    public virtual void Open()
    {
        IsOpen = true;
        m_CG.alpha = 1f;
        m_CG.blocksRaycasts = true;
        m_CG.interactable = true;
        OnOpened();
    }

    public virtual void Close()
    {
        IsOpen = false;
        OnClosed();

        if (m_CG == null) return;
        m_CG.alpha = 0f;
        m_CG.blocksRaycasts = false;
        m_CG.interactable = false;
    }

    protected virtual void OnOpened() { }
    protected virtual void OnClosed() { }
}
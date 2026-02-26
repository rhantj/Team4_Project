using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image m_FillArea;
    [SerializeField, Range(0, 1)]
    private float m_Value = 0f;

    public float Value
    {
        get => m_Value;
        set
        {
            float val = Mathf.Clamp01(value);

            if (Mathf.Approximately(m_Value, val))
                return;

            m_Value = val;
            UpdateUI();
        }
    }

    private void Awake()
    {
        UpdateUI();
    }

    private void OnValidate()
    {
        m_Value = Mathf.Clamp01(m_Value);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (m_FillArea != null)
            m_FillArea.fillAmount = m_Value;
    }

}

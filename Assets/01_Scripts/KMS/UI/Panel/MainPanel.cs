using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class MainPanel : UIPanel
{
    [SerializeField] private List<Button> m_StageButtons = new();

    public event Action<int> m_OnStageSelected;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < m_StageButtons.Count; ++i)
        {
            var idx = i;
            var btn = m_StageButtons[i];
            btn.onClick.AddListener(() => StageSelected(idx));
        }
    }

    private void StageSelected(int idx)
    {
        m_OnStageSelected?.Invoke(idx);
    }
}
using UnityEngine;
using System;

public enum UIKey
{
    HUD,
    Menu,
    FadeOut
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private HUDPanel m_HUD;
    [SerializeField] private MainPanel m_StartUI;

    private StageManager m_StageManager;

    private void Awake()
    {
        m_StageManager = FindAnyObjectByType<StageManager>();

        m_HUD.Close();
        m_StartUI.Open();

    }

    private void OnEnable()
    {
        m_StageManager.m_OnStageFinished += SwapPanel;
        m_StartUI.m_OnStageSelected += StageSelected;
    }

    private void OnDisable()
    {
        m_StageManager.m_OnStageFinished -= SwapPanel;
        m_StartUI.m_OnStageSelected -= StageSelected;
    }

    private void StageSelected(int idx)
    {
        m_StageManager.BuildStage(idx);
        m_StartUI.Close();
        m_HUD.Open();
    }

    public void SwapPanel()
    {
        m_HUD.Close();
        m_StartUI.Open();
    }
}
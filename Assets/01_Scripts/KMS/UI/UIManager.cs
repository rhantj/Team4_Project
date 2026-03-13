using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HUDPanel m_HUD;
    [SerializeField] private MainPanel m_StartUI;
    [SerializeField] private ClearPanel m_ClearUI;

    private StageManager m_StageManager;

    private void Awake()
    {
        m_StageManager = FindAnyObjectByType<StageManager>();

        m_HUD.Close();
        m_StartUI.Open();

    }

    private void OnEnable()
    {
        m_StartUI.m_OnStageSelected += StageSelected;
    }

    private void OnDisable()
    {
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

    public void OpenClearPanel()
    {
        m_ClearUI.Open();
    }
}
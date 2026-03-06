using System;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private SOStageConfig m_StageConfig;
    [SerializeField] private int m_StageIdx = 0;

    private FacilitySpawner m_FacilitySpawner;

    private Building m_CurrentMainBuilding;
    public event Action m_OnStageFinished;
    public event Action<int> m_OnStageIdxChanged;

    public int StageIdx
    {
        get => m_StageIdx;
        set { m_StageIdx = value; m_OnStageIdxChanged?.Invoke(m_StageIdx); }
    }


    public async void BuildStage(int stageIdx)
    {
        StageIdx = stageIdx;
        await WaitForService();
        await RebuildStage(StageIdx);

        m_OnStageIdxChanged?.Invoke(m_StageIdx);
    }

    private async Awaitable WaitForService()
    {
        while (!m_FacilitySpawner)
        {
            m_FacilitySpawner = GameManager.Instance.GetService<FacilitySpawner>();
            await Awaitable.NextFrameAsync();
        }

        while (!m_FacilitySpawner.IsServiceReady)
            await Awaitable.NextFrameAsync();
    }

    private async Awaitable RebuildStage(int stageIdx)
    {
        ClearStage();

        var step = m_StageConfig.Steps[stageIdx];

        // Theme spawn
        var theme = await m_FacilitySpawner.SpawnFromRequestAsync(step.StageTheme);

        // Main Building spawn
        var buildingGO = await m_FacilitySpawner.SpawnFromRequestAsync(step.MainBuilding);

        if (buildingGO.TryGetComponent<Building>(out var building))
            m_CurrentMainBuilding = building;
        if (m_CurrentMainBuilding)
            m_CurrentMainBuilding.m_OnBuildCompleted += OnBuildCompleted;

        // Resources spawn
        foreach (var res in step.Resources)
        {
            var obj = await m_FacilitySpawner.SpawnFromRequestAsync(res);
        }
    }

    private void ClearStage()
    {
        m_FacilitySpawner.RemoveAllObject();

        if (m_CurrentMainBuilding)
        {
            m_CurrentMainBuilding.m_OnBuildCompleted -= OnBuildCompleted;
            m_CurrentMainBuilding = null;
        }
    }

    private void OnBuildCompleted()
    {
        m_OnStageFinished?.Invoke();
        ClearStage();
    }
}
 
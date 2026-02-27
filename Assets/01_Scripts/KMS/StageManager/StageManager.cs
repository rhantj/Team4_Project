using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageManagerConfig : ServiceConfig<StageManager>
{
    [field: SerializeField] public SOStageConfig StageConfig { get; private set; }
}

public class StageManager : MonoBehaviour, IService
{
    private SOStageConfig m_StageConfig;
    [SerializeField] private int m_StageIdx = 0;

    private FacilitySpawner m_FacilitySpawner;

    private List<GameObject> m_SpawnedObjects = new();
    private Building m_CurrentMainBuilding;

    public int StageIdx
    {
        get => m_StageIdx;
        set { m_StageIdx = value; }
    }

    private async void Start()
    {
        await RebuildStage(StageIdx);
    }

    public void Configure(IServiceConfig iConfig)
    {
        if (iConfig is StageManagerConfig cfg && cfg.StageConfig != null)
            m_StageConfig = cfg.StageConfig;

        m_FacilitySpawner ??= GameManager.Instance.GetService<FacilitySpawner>();
    }

    public async Awaitable RebuildStage(int stageIdx)
    {
        ClearStage();

        var step = m_StageConfig.Steps[stageIdx];

        // Theme spawn
        var theme = await m_FacilitySpawner.SpawnFromRequestAsync(step.StageTheme);
        m_SpawnedObjects.Add(theme);

        // Main Building spawn
        var buildingGO = await m_FacilitySpawner.SpawnFromRequestAsync(step.MainBuilding);
        m_SpawnedObjects.Add(buildingGO);

        m_CurrentMainBuilding = buildingGO.GetComponent<Building>();
        if (m_CurrentMainBuilding)
            m_CurrentMainBuilding.m_OnBuildCompleted += OnBuildCompleted;

        // Resources spawn
        foreach (var res in step.Resources)
        {
            var obj = await m_FacilitySpawner.SpawnFromRequestAsync(res);
            m_SpawnedObjects.Add(obj);
        }
    }

    private void ClearStage()
    {
        foreach (var obj in m_SpawnedObjects)
            m_FacilitySpawner. RemoveObject(obj);

        m_SpawnedObjects.Clear();

        if (m_CurrentMainBuilding)
        {
            m_CurrentMainBuilding.m_OnBuildCompleted -= OnBuildCompleted;
            m_CurrentMainBuilding = null;
        }
    }

    private async void OnBuildCompleted()
    {
        StageIdx++;
        await RebuildStage(StageIdx);
    }
}
 
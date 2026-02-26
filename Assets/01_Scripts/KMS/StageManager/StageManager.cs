using UnityEngine;

public class StageManager : MonoBehaviour, IService
{
    [SerializeField] private BuildingSpawnRequest[] m_BuildingConfig;
    [SerializeField] private BuildingSpawnRequest[] m_ResourceConfig;

    GameObjectPoolingService m_PoolingService;

    private int m_StageIdx = 0;

    public int StageIdx => m_StageIdx;

    private void OnEnable()
    {
        m_PoolingService ??= GameManager.Instance.GetService<GameObjectPoolingService>();
    }

    private void Start()
    {
        RebuildStage(StageIdx);
    }

    public void Configure(IServiceConfig iConfig)
    {
        if (iConfig is StageManagerConfig cfg)
        {
            if (cfg.BuildingSpawnRequests != null && cfg.BuildingSpawnRequests.Length > 0)
                m_BuildingConfig = cfg.BuildingSpawnRequests;

            if (cfg.ResourceSpawnRequests != null && cfg.ResourceSpawnRequests.Length > 0)
                m_ResourceConfig = cfg.ResourceSpawnRequests;
        }
    }

    public void RebuildStage(int stageIdx)
    {
        if (m_PoolingService == null) return;

        var buildingcfg = m_BuildingConfig[stageIdx];
        var resourcecfg = m_ResourceConfig[stageIdx];

        SpawnObjectFromRequest(buildingcfg);
        SpawnObjectFromRequest(resourcecfg);
    }

    private void SpawnObjectFromRequest(BuildingSpawnRequest req)
    {
        if (req.SpawnPoint == null)
            req.SpawnPoint = req.BuildingPF.transform;

        var bpos = req.SpawnPoint.TransformPoint(req.LocalOffset);
        var brot = req.UsePointRotation ?
                   req.SpawnPoint.rotation :
                   Quaternion.Euler(req.EulerRotationOverride);

        var binst = m_PoolingService.GetOrCreateGameObject(req.BuildingPF);

        binst.transform.SetPositionAndRotation(bpos, brot);
    }
}
 
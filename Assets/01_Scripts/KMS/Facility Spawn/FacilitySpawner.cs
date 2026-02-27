using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class FacilitySpawnerConfig : ServiceConfig<FacilitySpawner>
{
    [field: SerializeField] public Facility[] Facilities { get; private set; }
}

public class FacilitySpawner : MonoBehaviour, IService
{
    [SerializeField] private Facility[] m_Facilities;

    private readonly Dictionary<EFacilityType, AssetReferenceGameObject> m_AddressTable = new();
    private readonly Dictionary<EFacilityType, GameObject> m_InstanceCache = new();

    [ReadOnly, SerializeField] private GameObjectPoolingService m_PoolingService;
    [ReadOnly, SerializeField] private ResourceManager m_ResourceManager;

    private void Start()
    {
        m_PoolingService ??= GameManager.Instance.GetService<GameObjectPoolingService>();
        m_ResourceManager ??= GameManager.Instance.GetService<ResourceManager>();
    }

    public void Configure(IServiceConfig iConfig)
    {
        if (iConfig is FacilitySpawnerConfig cfg && cfg.Facilities != null && cfg.Facilities.Length > 0)
            m_Facilities = cfg.Facilities;

        RebuildPrefabCache();
    }

    private void RebuildPrefabCache()
    {
        m_AddressTable.Clear();
        if (m_Facilities == null) return;

        foreach(var facility in m_Facilities)
        {
            if (facility == null || facility.PrefabReference != null)
                continue;

            if (!m_AddressTable.ContainsKey(facility.FacilityType))
                m_AddressTable.Add(facility.FacilityType, facility.PrefabReference);
        }
    }

    //Spawn facility
    public async Awaitable<GameObject> SpawnFromRequestAsync(SpawnRequest req)
    {
        var prefab = await m_ResourceManager.LoadPrefabAsync(req.PrefabReference);
        if (!prefab) return null;

        var point = prefab.transform;

        var pos = point.TransformPoint(req.LocalOffset);
        var rot = req.UsePointRotation ?
                  point.rotation :
                  Quaternion.Euler(req.EulerRoatationOverride);

        var instance = m_PoolingService != null ?
                       m_PoolingService.GetOrCreateGameObject(prefab, pos, rot) :
                       Instantiate(prefab, pos, rot);
        return instance;
    }

    public void RemoveFacility(EFacilityType type)
    {
        if (!m_InstanceCache.TryGetValue(type, out var inst))
            return;

        if (m_PoolingService != null && inst)
            m_PoolingService.ReturnOrDestroyGameObject(inst);
        else
            Destroy(inst);

        m_InstanceCache.Remove(type);
    }

    public void RemoveObject(GameObject obj)
    {
        if (m_PoolingService != null && obj)
            m_PoolingService.ReturnOrDestroyGameObject(obj);
        else
            Destroy(obj);
    }

    public void Clear()
    {
        foreach(var pair in m_InstanceCache)
        {
            if (m_PoolingService != null && pair.Value)
                m_PoolingService.ReturnOrDestroyGameObject(pair.Value);
            else
                Destroy(pair.Value);
        }

        m_InstanceCache.Clear();
    }
}
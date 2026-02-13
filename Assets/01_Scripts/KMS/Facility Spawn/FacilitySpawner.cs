using System;
using System.Collections.Generic;
using UnityEngine;

public class FacilitySpawner : MonoBehaviour, IService // ISpawner
{
    [SerializeField] private Facility[] m_Facilities;

    private readonly Dictionary<EFacilityType, GameObject> m_PrefabCache = new();
    private readonly Dictionary<EFacilityType, GameObject> m_InstanceCache = new();

    GameObjectPoolingService m_PoolingService;

    //private void Awake()
    //{
    //    foreach (var facility in m_Facilities)
    //    {
    //        if (facility.FacilityPF == null) continue;

    //        if (!m_PrefabCache.ContainsKey(facility.FacilityType))
    //        {
    //            m_PrefabCache.Add(facility.FacilityType, facility.FacilityPF);
    //        }
    //    }
    //}

    private void OnEnable()
    {
        m_PoolingService ??= GameManager.Instance.GetService<GameObjectPoolingService>();
        //FacilitySpawnSystem.DI(this);
    }

    public void Configure(IServiceConfig iConfig)
    {
        var cfg = iConfig as FacilitySpawnerConfig;
        if (cfg != null && cfg.Facilities != null && cfg.Facilities.Length > 0)
            m_Facilities = cfg.Facilities;

        RebuildPrefabCache();
    }

    private void RebuildPrefabCache()
    {
        m_PrefabCache.Clear();
        if (m_PrefabCache == null) return;

        foreach(var facility in m_Facilities)
        {
            if (facility.FacilityPF == null) continue;

            if (!m_PrefabCache.ContainsKey(facility.FacilityType))
                m_PrefabCache.Add(facility.FacilityType, facility.FacilityPF);
        }
    }

    public GameObject GetOrCreateFacility(EFacilityType type, Vector3 pos, Quaternion rot)
    {
        if (m_InstanceCache.TryGetValue(type, out var existing) && existing)
        {
            existing.transform.SetPositionAndRotation(pos, rot);
            return existing;
        }

        if (!m_PrefabCache.TryGetValue(type, out var pf) || !pf) return null;

        var inst = (m_PoolingService != null) ?
                   m_PoolingService.GetOrCreateGameObject(pf) :
                   Instantiate(pf);

        inst.transform.SetPositionAndRotation(pos, rot);
        m_InstanceCache[type] = inst;

        return inst;
    }
}
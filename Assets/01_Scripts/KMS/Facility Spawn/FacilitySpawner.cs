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

    private List<GameObject> m_SpawnedObjects = new();

    bool m_IsServiceReady = false;
    public bool IsServiceReady => m_IsServiceReady;

    public void Configure(IServiceConfig iConfig)
    {
        if (iConfig is FacilitySpawnerConfig cfg && cfg.Facilities != null && cfg.Facilities.Length > 0)
            m_Facilities = cfg.Facilities;

        RebuildPrefabCache();
        m_IsServiceReady = true;
    }

    private void RebuildPrefabCache()
    {
        m_AddressTable.Clear();
        if (m_Facilities == null) return;

        foreach(var facility in m_Facilities)
        {
            if (facility == null || facility.PrefabReference == null)
                continue;

            m_AddressTable[facility.FacilityType] = facility.PrefabReference;
        }
    }

    private async Awaitable WaitForService()
    {
        while (!m_ResourceManager)
        {
            m_ResourceManager = GameManager.Instance.GetService<ResourceManager>();
            await Awaitable.NextFrameAsync();
        }

        while (!m_PoolingService)
        {
            m_PoolingService = GameManager.Instance.GetService<GameObjectPoolingService>();
            await Awaitable.NextFrameAsync();
        }
    }

    //Spawn facility
    public async Awaitable<GameObject> SpawnFromRequestAsync(SpawnRequest req)
    {
        await WaitForService();

        if (!m_AddressTable.TryGetValue(req.Type, out var assetRef))
            return null;

        var pf = await m_ResourceManager.LoadPrefabAsync(assetRef);
        if (!pf)
            return null;

        var instance = m_PoolingService.GetOrCreateGameObject(pf);
        var point = req.SpawnPoint ? req.SpawnPoint : instance.transform;
        var pos = point.TransformPoint(req.LocalOffset);
        var rot = req.UseOverrideRotation ?
                  Quaternion.Euler(req.EulerRoatationOverride) :
                  point.rotation;

        instance.transform.SetPositionAndRotation(pos, rot);
        m_SpawnedObjects.Add(instance);
        return instance;
    }

    public async Awaitable<GameObject> GetOrCreateFacility(SpawnRequest req)
    {
        if(m_InstanceCache.TryGetValue(req.Type, out var existing) && existing)
            return existing;

        var instance = await SpawnFromRequestAsync(req);
        if(instance) m_InstanceCache[req.Type] = instance;
        

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

    public void RemoveAllObject()
    {
        if (m_SpawnedObjects.Count > 0)
        {
            foreach(var inst in m_SpawnedObjects)
            {
                m_PoolingService.ReturnOrDestroyGameObject(inst);
            }
        }

        m_SpawnedObjects.Clear();
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
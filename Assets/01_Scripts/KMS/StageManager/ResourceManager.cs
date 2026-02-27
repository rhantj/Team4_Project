using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class ResourceManagerConfig : ServiceConfig<ResourceManager> { }

public class ResourceManager : MonoBehaviour, IService
{
    private Dictionary<AssetReferenceGameObject, GameObject> m_PrefabCache = new();
    private bool m_Initialized = false;

    public void Configure(IServiceConfig iConfig) { }

    public async Awaitable Initialize()
    {
        if (m_Initialized) return;
        var handle = Addressables.InitializeAsync();
        await handle.Task;
        m_Initialized = true;
    }

    public async Awaitable<GameObject> LoadPrefabAsync(AssetReferenceGameObject reference)
    {
        await Initialize();

        if (m_PrefabCache.TryGetValue(reference, out var cached))
            return cached;

        var handle = Addressables.LoadAssetAsync<GameObject>(reference);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
            return null;

        var pf = handle.Result;
        m_PrefabCache[reference] = pf;

        return pf;
    }

    public void ReleasePrefab(AssetReferenceGameObject address)
    {
        if (!m_PrefabCache.TryGetValue(address, out var prefab))
            return;

        Addressables.Release(prefab);
        m_PrefabCache.Remove(address);
    }

    public void Clear()
    {
        foreach(var kvp in m_PrefabCache)
            Addressables.Release(kvp.Value);
        m_PrefabCache.Clear();
    }
}
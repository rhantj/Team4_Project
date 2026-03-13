using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameObjectTaggedGroupCacheServiceConfig : ServiceConfig<GameObjectTaggedGroupCacheService>
{
    //
}

public class GameObjectTaggedGroupCacheService : MonoBehaviour, IService
{

    public void Configure(IServiceConfig iConfig)
    {
        //
    }

    private Dictionary<string, HashSet<TaggedGroupCacheHelper>> m_TaggedGroupCache;

    private void OnEnable()
    {
        m_TaggedGroupCache = new Dictionary<string, HashSet<TaggedGroupCacheHelper>>();
    }

    private void OnDisable()
    {
        m_TaggedGroupCache = null;
    }

    public void RegisterTaggedGroupCache(TaggedGroupCacheHelper tagWrapper)
    {
        string tag = tagWrapper.tag;

        if (!m_TaggedGroupCache.TryGetValue(tag, out HashSet<TaggedGroupCacheHelper> cache))
        {
            cache = new HashSet<TaggedGroupCacheHelper>();
            m_TaggedGroupCache.Add(tag, cache);
        }

        cache.Add(tagWrapper);
    }

    public void UnregisterTaggedGroupCache(TaggedGroupCacheHelper tagWrapper)
    {
        string tag = tagWrapper.tag;

        if (!m_TaggedGroupCache.TryGetValue(tag, out HashSet<TaggedGroupCacheHelper> cache)) throw new InvalidOperationException($"Tagged Group Cache for \"{tag}\" is not found.");
        cache.Remove(tagWrapper);
        if (0 == cache.Count) m_TaggedGroupCache.Remove(tag);
    }

    public IEnumerable<GameObject> GetTaggedGroupCache(string tag)
    {
        if (!m_TaggedGroupCache.TryGetValue(tag, out HashSet<TaggedGroupCacheHelper> cache)) return Enumerable.Empty<GameObject>();
        return cache.Select(helper => helper.gameObject);
    }
}

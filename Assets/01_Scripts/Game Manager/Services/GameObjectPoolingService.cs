using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class GameObjectPoolingServiceConfig : ServiceConfig<GameObjectPoolingService>
{
    [field: SerializeField] public SerializableNullable<int> DefaultInitialCount { get; private set; }
    [field: SerializeField] public SerializableNullable<int> DefaultCapacity { get; private set; }
}

/// <summary>
/// Service to manage game object pooling.
/// Replace Instantiate&ltGameObject&gt with GetOrCreateGameObject and/or GetOrCreateInactivatedGameObject methods to achieve pooling.
/// </summary>
public class GameObjectPoolingService : MonoBehaviour, IService
{
    [field: SerializeField] public int DefaultInitialCount { get; private set; }
    [field: SerializeField] public int DefaultCapacity { get; private set; }

    public void Configure(IServiceConfig iConfig)
    {
        GameObjectPoolingServiceConfig config = iConfig as GameObjectPoolingServiceConfig;

        DefaultInitialCount = config.DefaultInitialCount.GetValueOrDefault(DefaultInitialCount);
        DefaultCapacity     = config.DefaultCapacity    .GetValueOrDefault(DefaultCapacity);
    }

    /// <summary>
    /// Parent GameObject to store pooled objects as siblings.
    /// </summary>
    private GameObject m_Pooler;

    /// <summary>
    /// Dictionary of prefabs to their capacities.
    /// </summary>
    private Dictionary<GameObject, int> m_Capacities;

    /// <summary>
    /// Dictionary mapping from tracked GameObject instances to their prefabs.
    /// </summary>
    private Dictionary<GameObject, GameObject> m_TrackingMap;

    /// <summary>
    /// Dictionary mapping from prefabs to stacks for their pooled instances available to use.
    /// </summary>
    private Dictionary<GameObject, Stack<GameObject>> m_PoolMap;

    private void OnEnable()
    {
        m_Pooler = new GameObject("Pooler");
        m_Pooler.SetActive(false);
        DontDestroyOnLoad(m_Pooler);

        m_Capacities = new Dictionary<GameObject, int>();
        m_TrackingMap = new Dictionary<GameObject, GameObject>();
        m_PoolMap = new Dictionary<GameObject, Stack<GameObject>>();
    }

    private void OnDisable()
    {
        m_Capacities = null;
        m_PoolMap = null;
        m_TrackingMap = null;

        Destroy(m_Pooler);
    }

    public void RetrievePooledGameObjects()
    {
        foreach (GameObject obj in m_TrackingMap.Keys)
        {
            if (null != obj.transform.parent && obj.transform.parent.gameObject == m_Pooler) continue;
            ReturnOrDestroyGameObject(obj);
        }
    }

    public GameObject GetOrCreateInactivatedGameObject(GameObject prefab)
    {
        Stack<GameObject> pool = GetOrCreatePool(prefab);
        if (null == pool) return null;
        if (!pool.TryPop(out GameObject result) || null == result)
        {
            result = Instantiate(prefab, m_Pooler.transform);
            if (null == result) return null;
            m_TrackingMap[result] = prefab;
        }
        result.SetActive(false);
        result.transform.SetParent(null);
        Scene scene = SceneManager.GetActiveScene();
        if (null == scene) throw new InvalidOperationException("Active Scene is null.");
        else if (result.scene != scene) SceneManager.MoveGameObjectToScene(result, scene);
        return result;
    }

    public GameObject GetOrCreateInactivatedGameObject(GameObject prefab, Transform parent)
    {
        GameObject result = GetOrCreateInactivatedGameObject(prefab);
        result.transform.SetParent(parent);
        return result;
    }

    public GameObject GetOrCreateInactivatedGameObject(GameObject prefab, Transform parent, bool worldPositionStays)
    {
        GameObject result = GetOrCreateInactivatedGameObject(prefab);
        result.transform.SetParent(parent, worldPositionStays);
        return result;
    }

    public GameObject GetOrCreateInactivatedGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject result = GetOrCreateInactivatedGameObject(prefab);
        result.transform.SetPositionAndRotation(position, rotation);
        return result;
    }

    public GameObject GetOrCreateInactivatedGameObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject result = GetOrCreateInactivatedGameObject(prefab);
        result.transform.SetPositionAndRotation(position, rotation);
        result.transform.SetParent(parent);
        return result;
    }

    public GameObject GetOrCreateGameObject(GameObject prefab)
    {
        GameObject result = GetOrCreateInactivatedGameObject(prefab);
        result.SetActive(true);
        return result;
    }

    public GameObject GetOrCreateGameObject(GameObject prefab, Transform parent)
    {
        GameObject result = GetOrCreateInactivatedGameObject(prefab, parent);
        result.SetActive(true);
        return result;
    }

    public GameObject GetOrCreateGameObject(GameObject prefab, Transform parent, bool worldPositionStays)
    {
        GameObject result = GetOrCreateInactivatedGameObject(prefab, parent, worldPositionStays);
        result.SetActive(true);
        return result;
    }

    public GameObject GetOrCreateGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject result = GetOrCreateInactivatedGameObject(prefab, position, rotation);
        result.SetActive(true);
        return result;
    }

    public GameObject GetOrCreateGameObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject result = GetOrCreateInactivatedGameObject(prefab, position, rotation, parent);
        result.SetActive(true);
        return result;
    }

    public void ReturnOrDestroyGameObject(GameObject obj)
    {
        if (null == obj) return;

        if (!m_TrackingMap.TryGetValue(obj, out GameObject prefab)
            || !m_PoolMap.TryGetValue(prefab, out Stack<GameObject> pool)
            || m_Capacities.GetValueOrDefault(prefab, DefaultCapacity) <= pool.Count)
        {
            m_TrackingMap.Remove(obj);
            Destroy(obj);
            return;
        }

        if (!pool.Contains(obj))
        {
            pool.Push(obj);
            obj.transform.SetParent(m_Pooler.transform);
        }
    }

    public void Shrink()
    {
        foreach (GameObject prefab in m_PoolMap.Keys) ShrinkPool(prefab);
    }

    public void ShrinkPool(GameObject prefab)
    {
        if (!m_PoolMap.TryGetValue(prefab, out Stack<GameObject> pool)) return;
        while (pool.Count > 0)
        {
            GameObject obj = pool.Pop();
            m_TrackingMap.Remove(obj);
            Destroy(obj);
        }
    }

    private Stack<GameObject> GetOrCreatePool(GameObject prefab)
    {
        if (null == prefab) return null;
        if (!m_PoolMap.TryGetValue(prefab, out Stack<GameObject> pool))
        {
            pool = new Stack<GameObject>();
            GameObjectPoolingPolicyModifier helper = prefab.GetComponent<GameObjectPoolingPolicyModifier>();
            int initialCount = (null != helper) ? helper.ModifiedInitialCount.GetValueOrDefault(DefaultInitialCount) : DefaultInitialCount;
            int capacity =     (null != helper) ? helper.ModifiedCapacity.GetValueOrDefault(DefaultCapacity)         : DefaultCapacity;

            if (null == pool
                || !m_PoolMap.TryAdd(prefab, pool)
                || !m_Capacities.TryAdd(prefab, capacity))
            {
                m_PoolMap.Remove(prefab);
                return null;
            }

            for (int i = 0; i < initialCount; i++)
            {
                GameObject obj = Instantiate(prefab, m_Pooler.transform);
                m_TrackingMap[obj] = prefab;
                pool.Push(obj);
            }
        }
        return pool;
    }
}

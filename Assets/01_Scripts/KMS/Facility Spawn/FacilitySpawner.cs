using System;
using System.Collections.Generic;
using UnityEngine;

public class FacilitySpawner : MonoBehaviour, ISpawner
{
    [SerializeField] private Facility[] m_Facilities;

    private readonly Dictionary<EFacilityType, GameObject> m_PrefabCache = new();
    private readonly Dictionary<EFacilityType, GameObject> m_InstanceCache = new();

    private void Awake()
    {
        foreach (var facility in m_Facilities)
        {
            if (facility.FacilityPF == null) continue;

            if (!m_PrefabCache.ContainsKey(facility.FacilityType))
            {
                m_PrefabCache.Add(facility.FacilityType, facility.FacilityPF);
            }
        }

        FacilitySpawnSystem.DI(this);
    }

    public GameObject GetOrCreateFacility(EFacilityType type, Vector3 pos, Quaternion rot)
    {
        if (m_InstanceCache.TryGetValue(type, out var existing) && existing)
        {
            existing.transform.SetPositionAndRotation(pos, rot);
            return existing;
        }

        if (!m_PrefabCache.TryGetValue(type, out var pf) || !pf) return null;

        var inst = Instantiate(pf, pos, rot);
        m_InstanceCache[type] = inst;
        return inst;
    }
}
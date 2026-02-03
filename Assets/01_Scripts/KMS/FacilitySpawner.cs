using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Facility
{
    public GameObject FacilityPF;
    public EFacilityType FacilityType;
}

public static class FacilitySpawnSystem
{
    public static ISpawner Spawner { get; private set; }
    public static void DI(ISpawner spawner)
    {
        Spawner = spawner;
    }
}

public class FacilitySpawner : MonoBehaviour, ISpawner
{
    [SerializeField] private Facility[] m_Facilities;
    private Dictionary<EFacilityType, GameObject> m_FacilityCache = new();

    private void Awake()
    {
        foreach (var facility in m_Facilities)
        {
            if (facility.FacilityPF == null) continue;

            if (!m_FacilityCache.ContainsKey(facility.FacilityType))
            {
                m_FacilityCache.Add(facility.FacilityType, facility.FacilityPF);
            }
        }

        FacilitySpawnSystem.DI(this);
    }

    public void InstantiateFaility(EFacilityType type, Vector3 pos, Quaternion rot)
    {
        if (m_FacilityCache.TryGetValue(type, out var pf))
        {
            Instantiate(pf, pos, rot);
        }
    }
}
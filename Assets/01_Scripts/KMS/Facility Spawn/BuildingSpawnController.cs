using UnityEngine;
using System.Collections.Generic;

public class BuildingSpawnController : MonoBehaviour
{
    [SerializeField] private Building m_Building;
    [SerializeField] private List<StepSpawnGroup> m_SpawnGroups;

    private FacilitySpawner m_FacilitySpawner;

    private void Awake()
    {
        m_Building ??= GetComponent<Building>();
    }

    private void Start()
    {
        m_FacilitySpawner = GameManager.Instance.GetService<FacilitySpawner>();
        m_Building.m_OnStepCompleted += OnStepCompleted;
    }

    private void OnDisable()
    {
        m_Building.m_OnStepCompleted -= OnStepCompleted;
    }

    private async void OnStepCompleted(int stepIdx)
    {
        var gp = m_SpawnGroups.Find(g => g.StepIndexToTrigger == stepIdx);
        if (gp == null) return;

        foreach(var req in gp.Requests)
        {
            var go = await m_FacilitySpawner.GetOrCreateFacility(req);
        }
    }
}
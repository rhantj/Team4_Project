using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SOBuilding m_BuildingData;

    [Header("Upgrade Spawns")] [Tooltip("Per Step Index")]
    [SerializeField] private List<StepSpawnGroup> m_SpawnGroups = new();

    [Header("Status")]
    [SerializeField] private int m_CurrentStepIdx = 0;
    [SerializeField] private int m_CurrentStepItems = 0;
    [Range(0, 100)]
    [SerializeField] private float m_Progress = 0;

    [Header("Area")]
    [SerializeField] private ItemIOArea m_InputArea;

    private Coroutine m_InputCoroutine;
    private readonly WaitForSeconds m_InputDuration = new(0.1f);

    private readonly HashSet<int> m_ExcutedStepSpawns = new();

    private void OnEnable()
    {
        m_InputArea.m_OnEnterArea += InputItems;
        m_InputArea.m_OnExitArea += ExitArea;
    }

    private void OnDisable()
    {
        m_InputArea.m_OnEnterArea -= InputItems;
        m_InputArea.m_OnExitArea -= ExitArea;
    }

    public void InputItems()
    {
        m_InputCoroutine = StartCoroutine(Co_InputItems());
    }

    void ExitArea()
    {
        if (m_InputCoroutine != null)
        {
            StopCoroutine(m_InputCoroutine);
            m_InputCoroutine = null;
        }
    }

    private IEnumerator Co_InputItems()
    {
        float elapsedTime = 0f;
        while (elapsedTime < .5f)
        {
            if (!m_InputArea.IsPlayerEnter) yield break;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        var inv = m_InputArea.Player.GetComponent<InventoryExpended>();
        var currentStep = m_BuildingData.Steps[m_CurrentStepIdx];

        while (m_InputArea.IsPlayerEnter)
        {
            if (m_CurrentStepIdx >= m_BuildingData.Steps.Count)
            {
                yield return null;
                continue;
            }

            if (inv.IsEmpty)
            {
                yield return null;
                continue;
            }

            if (inv.TryRemoveItemByName(currentStep.StepName))
            {
                m_CurrentStepItems++;
            }

            UpdateProgress();

            if (m_CurrentStepItems >= currentStep.RequierAmount)
            {
                m_CurrentStepIdx++;
                m_CurrentStepItems = 0;

                // spawn facility
                ExcuteSpawnGroupForStep(m_CurrentStepIdx);

                yield break;
            }

            yield return m_InputDuration;
        }

        m_InputCoroutine = null;
    }

    private void UpdateProgress()
    {
        if (m_BuildingData.Steps.Count == 0) return;

        var totalStep = m_BuildingData.Steps.Count;
        var currentStepProgress = (float)m_CurrentStepItems / m_BuildingData.Steps[m_CurrentStepIdx].RequierAmount;

        m_Progress = ((m_CurrentStepIdx + currentStepProgress) / totalStep) * 100f;
    }

    private void ExcuteSpawnGroupForStep(int stepIdx)
    {
        if (m_ExcutedStepSpawns.Contains(stepIdx)) return;

        var group = m_SpawnGroups.Find(g => g.StepIndexToTrigger == stepIdx);
        if (group == null || group.Requests == null || group.Requests.Count == 0)
            return;

        foreach(var req in group.Requests)
        {
            if(req.SpawnPoint == null) continue;
            var pos = req.SpawnPoint.TransformPoint(req.LocalOffset);
            Quaternion rot = req.UsePointRotation ?
                req.SpawnPoint.rotation :
                Quaternion.Euler(req.EulerRotationOverride);

            FacilitySpawnSystem.Spawner.GetOrCreateFacility(req.Type, pos, rot);
        }

        m_ExcutedStepSpawns.Add(stepIdx);
    }
}

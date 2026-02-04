using System.Collections;
using UnityEngine;

public class BuildArea : ItemIOArea
{
    [Header("Facility Type")]
    [SerializeField] private EFacilityType type;

    [Header("Config")]
    [SerializeField] private int m_Cost;

    private float m_ElapsedTime = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_OnEnterArea += EnterBuildFacilityArea;
        m_OnExitArea += ExitBuildFacilityArea;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        m_OnEnterArea -= EnterBuildFacilityArea;
        m_OnExitArea -= ExitBuildFacilityArea;
    }

    private void EnterBuildFacilityArea()
    {
        StartCoroutine(Co_EnterBuildFacilityArea());
    }

    private void ExitBuildFacilityArea()
    {
        m_ElapsedTime = 0;
    }

    private IEnumerator Co_EnterBuildFacilityArea()
    {
        while(m_ElapsedTime < 0.5f)
        {
            m_ElapsedTime += Time.deltaTime;
            yield return null;
        }

        // if

        FacilitySpawnSystem.Spawner.InstantiateFaility(type, transform.position, transform.rotation);
        canDetect = false;
    }
}

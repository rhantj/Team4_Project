using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SOBuilding m_BuildingData;

    [Header("Status")]
    [SerializeField] private EFacilityType m_FacilityType;
    [SerializeField] private int m_CurrentStepIdx = 0;
    [SerializeField] private int m_CurrentStepItems = 0;
    [SerializeField] private GameObject m_FacilityCache = null;
    [Range(0, 100)]
    [SerializeField] private float m_Progress = 0;

    [Header("Area")]
    [SerializeField] private ItemIOArea m_InputArea;

    private Coroutine m_InputCoroutine;

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

        var inv = m_InputArea.Player.GetComponent<Inventory>();
        var currentStep = m_BuildingData.Steps[m_CurrentStepIdx];
        var wait = new WaitForSeconds(0.1f);


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

            var itemdata = ScriptableObject.CreateInstance<ResourceItemData>();
            itemdata.m_SoItemName = currentStep.StepName;
            itemdata.m_ItemPrefab = currentStep.RequierItem;

            inv.RemoveItem(itemdata);
            m_CurrentStepItems++;

            UpdateProgress();

            if (m_CurrentStepItems >= currentStep.RequierAmount)
            {
                m_CurrentStepIdx++;
                m_CurrentStepItems = 0;

                // spawn facility
                if(m_FacilityCache == null)
                {
                    FacilitySpawnSystem.Spawner.InstantiateFaility(
                    m_FacilityType,
                    Vector3.left * 5f,
                    Quaternion.identity,
                    out var treeFacility);

                    m_FacilityCache = treeFacility;
                }

                yield return null;
                continue;
            }

            yield return wait;
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
}

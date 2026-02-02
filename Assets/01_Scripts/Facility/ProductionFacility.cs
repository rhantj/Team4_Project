using System.Collections;
using UnityEngine;

public class ProdictionFacility : MonoBehaviour
{
    [SerializeField] private SOProductionFacility m_FacilitySO;
    [SerializeField] private ItemIOArea m_OutputArea;
    [SerializeField] private ItemIOArea m_InputArea;

    private int m_Input;
    private int m_Output;
    private Coroutine m_ProductionCoroutine;

    public SOProductionFacility FacilitySO { get { return m_FacilitySO; } }

    private void Awake()
    {
        InitializeIOProduct(m_FacilitySO);
    }

    private void OnEnable()
    {
        if (m_OutputArea)
        {
            m_OutputArea.m_OnEnterArea += PlayerEnter;
            m_OutputArea.m_OnExitArea += PlayerExit;
        }
    }

    private void OnDisable()
    {
        if (m_OutputArea)
        {
            m_OutputArea.m_OnEnterArea -= PlayerEnter;
            m_OutputArea.m_OnExitArea -= PlayerExit;
        }
    }

    private void PlayerEnter()
    {
        m_ProductionCoroutine = StartCoroutine(WaitForOutput());
    }

    private void PlayerExit()
    {
        if (m_ProductionCoroutine != null)
        {
            StopCoroutine(m_ProductionCoroutine);
            m_ProductionCoroutine = null;
        }
    }

    private void InitializeIOProduct(SOProductionFacility data)
    {
        m_Input = data.inputItem;
        m_Output = data.outputItem;
    }

    private IEnumerator WaitForOutput()
    {
        Debug.Log("item product routine start");
        float elapsedTime = 0f;
        while (elapsedTime < .5f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GiveItemToPlayer();

        m_ProductionCoroutine = null;
    }

    private IEnumerator WaitForInput()
    {
        float elapsedTime = 0f;
        while (elapsedTime < .5f) 
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // return input
    }

    private void GiveItemToPlayer()
    {
        Debug.Log($"Return : {m_Output}");
    }
}
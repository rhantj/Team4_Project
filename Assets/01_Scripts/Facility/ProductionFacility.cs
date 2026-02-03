using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ProdictionFacility : MonoBehaviour
{
    [SerializeField] private SOProductionFacility m_FacilitySO;
    [SerializeField] private ItemIOArea m_OutputArea;
    [SerializeField] private ItemIOArea m_InputArea;
    [SerializeField] private int m_InputLimit = 5;
    [SerializeField] private int m_OutputLimit = 5;
    [SerializeField] private float m_ProductionTime = 2f;

    private int m_Input;
    private int m_Output;
    private Coroutine m_OutputCoroutine;
    private Coroutine m_InputCoroutine;
    private Coroutine m_ProductionCoroutine;

    private Stack<int> m_Inputs;
    private Stack<int> m_Outputs;

    public SOProductionFacility FacilitySO { get { return m_FacilitySO; } }

    private void Awake()
    {
        InitializeIOProduct(m_FacilitySO);
        m_Inputs = new Stack<int>(m_InputLimit);
        m_Outputs = new Stack<int>(m_OutputLimit);
    }

    private void OnEnable()
    {
        if (m_OutputArea)
        {
            m_OutputArea.m_OnEnterArea += PlayerEnterOutputArea;
            m_OutputArea.m_OnExitArea += PlayerExitOutputArea;
        }

        if (m_InputArea)
        {
            m_InputArea.m_OnEnterArea += PlayerEnterInputArea;
            m_InputArea.m_OnExitArea += PlayerExitInputArea;
        }
    }

    private void OnDisable()
    {
        if (m_OutputArea)
        {
            m_OutputArea.m_OnEnterArea -= PlayerEnterOutputArea;
            m_OutputArea.m_OnExitArea -= PlayerExitOutputArea;
        }

        if (m_InputArea)
        {
            m_InputArea.m_OnEnterArea -= PlayerEnterInputArea;
            m_InputArea.m_OnExitArea -= PlayerExitInputArea;
        }
    }
    private void InitializeIOProduct(SOProductionFacility data)
    {
        m_Input = data.inputItem;   
        m_Output = data.outputItem;
    }

    private void PlayerEnterOutputArea() => 
        m_OutputCoroutine = StartCoroutine(WaitForOutput());

    private void PlayerExitOutputArea()
    {
        if (m_OutputCoroutine != null)
        {
            StopCoroutine(m_OutputCoroutine);
            m_OutputCoroutine = null;
        }

        if (m_InputCoroutine != null)
        {
            StopCoroutine(m_InputCoroutine);
            m_InputCoroutine = null;
        }
    }

    private void PlayerEnterInputArea() =>
        m_InputCoroutine = StartCoroutine(WaitForInput());

    private void PlayerExitInputArea()
    {
        if (m_InputCoroutine != null)
        {
            StopCoroutine(m_InputCoroutine);
            m_InputCoroutine = null;
        }
    }

    private IEnumerator WaitForOutput()
    {
        if (m_Outputs.Count == 0)
        {
            yield break;
        }

        Debug.Log("item output start");
        float elapsedTime = 0f;
        while (elapsedTime < .5f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GiveItemToPlayer();
        m_OutputCoroutine = null;
    }

    private IEnumerator WaitForInput()
    {
        if(m_Inputs.Count >= m_InputLimit)
        {
            yield break;
        }

        Debug.Log("item input start");
        float elapsedTime = 0f;
        while (elapsedTime < .5f) 
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (m_Inputs.Count < m_InputLimit)
        {
            GetItemFromPlayer();
            yield return new WaitForSeconds(.5f);
        }
    }

    private void GiveItemToPlayer()
    {
        if (m_Outputs.Count > 0)
        {
            var item = m_Outputs.Pop();
            Debug.Log($"Return : {item}");
        }
    }

    private void GetItemFromPlayer()
    {
        var player = m_InputArea.m_player.GetComponent<ALTPlayer>();

        // if player's item is same as m_Input, return m_Input;
        if (m_Inputs.Count < m_InputLimit)
        {
            if (!player.items.TryPop(out var item)) return;
            m_Inputs.Push(item);
            Debug.Log($"Get : {m_Input}, Current Stack : {m_Inputs.Count}");
        }

        m_ProductionCoroutine ??= StartCoroutine(ProductItems(m_ProductionTime));
    }

    private IEnumerator ProductItems(float delay)
    {
        var wait = new WaitForSeconds(delay);

        while (m_Inputs.Count > 0)
        {
            if (m_Outputs.Count >= m_OutputLimit)
            {
                yield return null;
                continue;
            }

            yield return wait;

            var item = m_Inputs.Pop();
            m_Outputs.Push(item);
        }

        m_ProductionCoroutine = null;
    }
}
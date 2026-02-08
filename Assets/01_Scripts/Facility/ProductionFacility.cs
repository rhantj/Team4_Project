using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Net;

public class ProdictionFacility : MonoBehaviour
{
    [SerializeField] private SOProductionFacility m_FacilitySO;
    [SerializeField] private ItemIOArea m_OutputArea;
    [SerializeField] private ItemIOArea m_InputArea;
    [SerializeField] private int m_InputLimit = 5;
    [SerializeField] private int m_OutputLimit = 5;
    [SerializeField] private float m_ProductionTime = 5f;

    private int m_Input;
    private int m_Output;
    private Coroutine m_OutputCoroutine;
    private Coroutine m_InputCoroutine;
    private Coroutine m_ProductionCoroutine;

    [SerializeField] private List<int> m_Inputs;
    [SerializeField] private List<int> m_Outputs;

    public SOProductionFacility FacilitySO { get { return m_FacilitySO; } }

    private void Awake()
    {
        InitializeIOProduct(m_FacilitySO);
        m_Inputs = new List<int>(m_InputLimit);
        m_Outputs = new List<int>(m_OutputLimit);
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
    private void PlayerEnterInputArea() =>
        m_InputCoroutine = StartCoroutine(WaitForInput());

    private void PlayerExitOutputArea()
    {
        if (m_OutputCoroutine != null)
        {
            StopCoroutine(m_OutputCoroutine);
            m_OutputCoroutine = null;
        }
    }

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

        var player = m_OutputArea.m_player.GetComponent<ALTPlayer>();
        while (m_OutputArea.m_isPlayerEnter)
        {
            if (m_Outputs.Count == 0)
            {
                yield return null;
                continue;
            }

            if (m_Outputs.Count > 0)
            {
                var item = m_Outputs[0];
                m_Outputs.RemoveAt(0);
                player.items.Enqueue(item);
                Debug.Log($"Return : {item}");
            }
            yield return new WaitForSeconds(.5f);
        }

        m_OutputCoroutine = null;
    }

    private IEnumerator WaitForInput()
    {
        Debug.Log("item input start");
        float elapsedTime = 0f;
        while (elapsedTime < .5f) 
        {
            if (!m_InputArea.m_isPlayerEnter) yield break;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        var player = m_InputArea.m_player.GetComponent<ALTPlayer>();
        while (m_InputArea.m_isPlayerEnter)
        {
            if (m_Inputs.Count >= m_InputLimit)
            {
                yield return null;
                continue;
            }

            if(player.items.Count == 0)
            {
                yield return null;
                continue;
            }

            if (m_Inputs.Count < m_InputLimit)
            {
                var item = player.items.Dequeue();
                if (item != m_Input)
                {
                    yield return null;
                    player.items.Enqueue(item);
                    continue;
                }

                m_Inputs.Add(item);
                Debug.Log($"Get : {item}, Current Stack : {m_Inputs.Count}");
            }

            m_ProductionCoroutine ??= StartCoroutine(ProductItems(m_ProductionTime));
            yield return new WaitForSeconds(.5f);
        }

        m_InputCoroutine = null;
    }

    private void GiveItemToPlayer(ALTPlayer player)
    {
        if (m_Outputs.Count > 0)
        {
            var item = m_Outputs[0];
            if (item == m_Output) return;
            m_Outputs.RemoveAt(0);

            player.items.Enqueue(item);
            Debug.Log($"Return : {item}");
        }
    }

    private void GetItemFromPlayer(ALTPlayer player)
    {
        if (m_Inputs.Count < m_InputLimit)
        {
            var item = player.items.Dequeue();
            if (item != m_Input) return;
            m_Inputs.Add(item);
            Debug.Log($"Get : {item}, Current Stack : {m_Inputs.Count}");
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

            m_Inputs.RemoveAt(0);
            m_Outputs.Add(m_Output);
        }

        m_ProductionCoroutine = null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProdictionFacility : MonoBehaviour
{
    [SerializeField] private SOProductionFacility m_FacilitySO;
    [SerializeField] private ItemIOArea m_OutputArea;
    [SerializeField] private ItemIOArea m_InputArea;
    [SerializeField] private int m_InputLimit = 5;
    [SerializeField] private int m_OutputLimit = 5;
    [SerializeField] private float m_ProductionTime = 5f;

    private GameObject m_Input;
    private GameObject m_Output;
    private Coroutine m_OutputCoroutine;
    private Coroutine m_InputCoroutine;
    private Coroutine m_ProductionCoroutine;

    [SerializeField] private List<GameObject> m_Inputs;
    [SerializeField] private List<GameObject> m_Outputs;

    public SOProductionFacility FacilitySO { get { return m_FacilitySO; } }

    private void Awake()
    {
        InitializeIOProduct(m_FacilitySO);
        m_Inputs = new List<GameObject>(m_InputLimit);
        m_Outputs = new List<GameObject>(m_OutputLimit);
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

        var inv = m_OutputArea.Player.GetComponent<Inventory>();
        while (m_OutputArea.IsPlayerEnter)
        {
            if (m_Outputs.Count == 0)
            {
                yield return null;
                continue;
            }

            if (m_Outputs.Count > 0)
            {
                if (inv.IsFull)
                {
                    yield return null;
                    continue;
                }

                var itemdata = ScriptableObject.CreateInstance<ResourceItemData>();
                itemdata.m_ItemName = "Log2";
                itemdata.m_ItemPrefab = m_Outputs[0];

                inv.AddItem(itemdata);
                m_Outputs.RemoveAt(0);
                Debug.Log($"Output : {itemdata.m_ItemPrefab}, Current Stack : {m_Inputs.Count}");
            }
            yield return new WaitForSeconds(.5f);
        }

        m_OutputCoroutine = null;
    }

    private IEnumerator WaitForInput()
    {
        float elapsedTime = 0f;
        while (elapsedTime < .5f) 
        {
            if (!m_InputArea.IsPlayerEnter) yield break;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        var inv = m_InputArea.Player.GetComponent<Inventory>();
        while (m_InputArea.IsPlayerEnter)
        {
            if (m_Inputs.Count >= m_InputLimit)
            {
                yield return null;
                continue;
            }

            if (inv.IsEmpty)
            {
                yield return null;
                continue;
            }

            if(m_Inputs.Count < m_InputLimit)
            {
                var itemdata = ScriptableObject.CreateInstance<ResourceItemData>();
                itemdata.m_ItemName = "Log";
                itemdata.m_ItemPrefab = m_Input;

                inv.RemoveItem(itemdata);
                m_Inputs.Add(itemdata.m_ItemPrefab);

                Debug.Log($"Input : {itemdata.m_ItemPrefab}, Current Stack : {m_Inputs.Count}");
            }

            m_ProductionCoroutine ??= StartCoroutine(ProductItems(m_ProductionTime));
            yield return new WaitForSeconds(.5f);
        }

        m_InputCoroutine = null;
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
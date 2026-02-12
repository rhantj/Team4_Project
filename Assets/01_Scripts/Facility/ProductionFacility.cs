using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ProdictionFacility : MonoBehaviour
{
    [Header("Production")]
    [SerializeField] private SOProductionFacility m_FacilitySO;
    [SerializeField] private ItemIOArea m_OutputArea;
    [SerializeField] private ItemIOArea m_InputArea;
    [SerializeField] private ItemIOArea m_UpgradeArea;
    [SerializeField] private int m_InputLimit = 5;
    [SerializeField] private int m_OutputLimit = 5;
    [SerializeField] private float m_ProductionTime = 5f;

    [Header("Upgrades")]
    [ReadOnly][SerializeField] private float m_CurrentCostProgress = 0f;
    [ReadOnly][SerializeField] private float m_UpgradeCost = 1000f;
    [ReadOnly][SerializeField] private bool m_IsUpgraded = false;

    private GameObject m_Input;
    private GameObject m_Output;

    private Coroutine m_OutputCoroutine;
    private Coroutine m_InputCoroutine;
    private Coroutine m_ProductionCoroutine;
    private Coroutine m_UpgradeCoroutine;
    private readonly WaitForSeconds m_IODuration = new(0.5f);

    [SerializeField] private List<GameObject> m_Inputs = new();
    [SerializeField] private List<GameObject> m_Outputs = new();

    private InventoryExpended m_Inv;

    public SOProductionFacility FacilitySO { get { return m_FacilitySO; } }

    private void Awake()
    {
        InitializeIOProduct(m_FacilitySO);
        m_Inv = m_InputArea.Player.GetComponent<InventoryExpended>();
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

        if (m_UpgradeArea)
        {
            m_UpgradeArea.m_OnEnterArea += PlayerEnterUpgradeArea;
            m_UpgradeArea.m_OnExitArea += PlayerExitUpgradeArea;
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

        if (m_UpgradeArea)
        {
            m_UpgradeArea.m_OnEnterArea -= PlayerEnterUpgradeArea;
            m_UpgradeArea.m_OnExitArea -= PlayerExitUpgradeArea;
        }
    }

    private void InitializeIOProduct(SOProductionFacility data)
    {
        m_Input = data.inputItem;   
        m_Output = data.outputItem;
    }

    private void PlayerEnterOutputArea() => 
        m_OutputCoroutine = StartCoroutine(Co_WaitForOutput());
    private void PlayerEnterInputArea() =>
        m_InputCoroutine = StartCoroutine(Co_WaitForInput());

    private void PlayerEnterUpgradeArea() =>
        m_UpgradeCoroutine = StartCoroutine(Co_WaitForUpgrade());

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

    private void PlayerExitUpgradeArea()
    {
        if (m_UpgradeCoroutine != null)
        {
            StopCoroutine(m_UpgradeCoroutine);
            m_UpgradeCoroutine = null;
        }
    }

    private IEnumerator Co_WaitForOutput()
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

        while (m_OutputArea.IsPlayerEnter)
        {
            if (m_Outputs.Count == 0)
            {
                yield return null;
                continue;
            }

            if (m_Outputs.Count > 0)
            {
                if (m_Inv.IsFull)
                {
                    yield return null;
                    continue;
                }

                m_Inv.AddItem(m_Outputs[0]);
                m_Outputs.RemoveAt(0);
            }
            yield return m_IODuration;
        }

        m_OutputCoroutine = null;
    }

    private IEnumerator Co_WaitForInput()
    {
        float elapsedTime = 0f;
        while (elapsedTime < .5f) 
        {
            if (!m_InputArea.IsPlayerEnter) yield break;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (m_InputArea.IsPlayerEnter)
        {
            if (m_Inputs.Count >= m_InputLimit)
            {
                yield return null;
                continue;
            }

            if (m_Inv.IsEmpty)
            {
                yield return null;
                continue;
            }

            if (m_Inv.TryRemoveItemByName(m_Input.name))
            {
                m_Inputs.Add(m_Input);
                m_ProductionCoroutine ??= StartCoroutine(Co_ProductItems(m_ProductionTime));
            }

            yield return m_IODuration;
        }

        m_InputCoroutine = null;
    }

    private IEnumerator Co_ProductItems(float delay)
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

    private IEnumerator Co_WaitForUpgrade()
    {
        float elapse = 0;
        while (elapse < 0.5f)
        {
            elapse += Time.deltaTime;
            yield return null;
        }

        while (m_UpgradeArea.IsPlayerEnter && m_CurrentCostProgress < m_UpgradeCost && !m_IsUpgraded)
        {
            if (m_Inv.Gold > 0)
            {
                m_Inv.Gold -= 100f;
                m_CurrentCostProgress += 100f;
            }

            if(m_CurrentCostProgress >= m_UpgradeCost)
            {
                UpgradeList();
                yield break;
            }

            yield return m_IODuration;
        }

        m_UpgradeCoroutine = null;
    }

    private void UpgradeList()
    {
        m_IsUpgraded = true;

        m_OutputLimit += 5;
        m_InputLimit += 5;
        m_ProductionTime *= 0.5f;
    }
}
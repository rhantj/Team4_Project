using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ProductionSales : MonoBehaviour
{
    [SerializeField] private ItemIOArea m_InputArea;
    [SerializeField] private SOAllMaterials m_Materials;

    private Coroutine m_InputCoroutine;

    private void OnEnable()
    {
        if (m_InputArea)
        {
            m_InputArea.m_OnEnterArea += PlayerEnterInputArea;
            m_InputArea.m_OnExitArea += PlayerExitInputArea;
        }
    }

    private void OnDisable()
    {
        if (m_InputArea)
        {
            m_InputArea.m_OnEnterArea -= PlayerEnterInputArea;
            m_InputArea.m_OnExitArea -= PlayerExitInputArea;
        }
    }

    private void PlayerEnterInputArea() =>
        m_InputCoroutine ??= StartCoroutine(Co_WaitForInput());

    private void PlayerExitInputArea()
    {
        if (m_InputCoroutine != null)
        {
            StopCoroutine(m_InputCoroutine);
            m_InputCoroutine = null;
        }
    }

    private IEnumerator Co_WaitForInput()
    {
        float elapsedTime = 0f;
        var wait = new WaitForSeconds(.1f);
        while (elapsedTime < 0.5f)
        {
            if(!m_InputArea.IsPlayerEnter) yield break;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        var inv = m_InputArea.Player.GetComponent<InventoryExpended>();
        while (m_InputArea.IsPlayerEnter)
        {
            if (inv.IsEmpty)
            {
                yield return null;
                continue;
            }

            // sale input items
            foreach (var item in m_Materials.AllMaterials)
            {
                var itemName = item.name;
                if (inv.TryRemoveItemByName(itemName))
                {
                    // return cash
                    Debug.Log("Get Cash");
                    inv.Gold += 100f;
                }
            }

            yield return wait;
        }
    }
}
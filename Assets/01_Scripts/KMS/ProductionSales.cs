using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ProductionSales : MonoBehaviour
{
    [SerializeField] private ItemIOArea m_InputArea;
    [SerializeField] private SOAllMaterials m_Materials;

    private IEnumerator Co_WaitForInput()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            if(!m_InputArea.IsPlayerEnter) yield break;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        var inv = m_InputArea.Player.GetComponent<Inventory>();
        while (m_InputArea.IsPlayerEnter)
        {
            if (inv.IsEmpty)
            {
                yield return null;
                continue;
            }

            // sale input items

            // return cash
        }
    }
}
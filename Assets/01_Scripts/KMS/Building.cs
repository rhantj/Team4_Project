using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private ItemIOArea m_InputArea;
    [Range(0, 100)]
    [SerializeField] private float m_Progress = 0;
    private int m_Steps;
    private List<int> m_InputItems = new();

    private Coroutine m_InputCoroutine;

    public void SetStep(int step) => m_Steps = step;

    private void OnEnable()
    {
        m_InputArea.m_OnEnterArea += InputItems;
    }

    private void OnDisable()
    {
        m_InputArea.m_OnEnterArea -= InputItems;

        if (m_InputCoroutine != null)
        {
            StopCoroutine(m_InputCoroutine);
            m_InputCoroutine = null;
        }
    }

    public void SetItemAndStep(SOBuilding data)
    {
        m_Steps = data.Step;
        m_InputItems = data.InputItems;
    }

    private void InputItems()
    {
        m_InputCoroutine = StartCoroutine(Co_InputItems());
    }

    IEnumerator Co_InputItems()
    {
        var player = m_InputArea.m_player.GetComponent<ALTPlayer>();
        var wait = new WaitForSeconds(0.1f);

        while (m_InputArea.m_isPlayerEnter)
        {
            var item = player.items.Dequeue();
            Debug.Log($"Get Item : {item}");

            

            yield return wait;
        }
    }
}

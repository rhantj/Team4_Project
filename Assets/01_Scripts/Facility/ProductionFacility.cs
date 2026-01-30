using System.Collections;
using UnityEngine;

public class ProdictionFacility : MonoBehaviour
{
    [SerializeField] private SOProductionFacility m_FacilitySO;
    [SerializeField] private ItemIOArea m_InputArea;

    private int m_Input;
    private int m_Output;

    public SOProductionFacility FacilitySO { get { return m_FacilitySO; } }

    private void Awake()
    {
        InitializeIOProduct(m_FacilitySO);
    }

    private void InitializeIOProduct(SOProductionFacility data)
    {
        m_Input = data.inputItem;
        m_Output = data.outputItem;
    }

    private IEnumerator WaitForOutput()
    {
        float elapsedTime = 0f;
        while (elapsedTime < .5f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // return output
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
}
using UnityEngine;
using UnityEngine.Rendering;

public class BillboardUI : MonoBehaviour
{
    private Camera m_Camera;

    private void Awake()
    {
        m_Camera = Camera.main;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + m_Camera.transform.forward);
    }
}
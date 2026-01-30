using System;
using System.Collections;
using UnityEngine;

public class ItemIOArea : MonoBehaviour
{
    public event Action m_OnEnterArea;
    public event Action m_OnExitArea;

    [Header("Setting")]
    [SerializeField] Transform m_player;
    [SerializeField] private float m_CheckAreaInterval = 0.1f;
    [SerializeField] private float m_Width;
    [SerializeField] private float m_Height;
    [SerializeField] public bool m_isPlayerEnter = false;
    RectZone m_IOArea = new()
    {
        minX = 0,
        minZ = 0,
        maxX = 0,
        maxZ = 0
    };

    private Coroutine m_CheckCoroutine;

    public bool IsPlayerEnter => m_isPlayerEnter;

    private void Awake()
    {
        UpdateBox();
    }

    private void Start()
    {
        m_CheckCoroutine ??= StartCoroutine(Co_CheckArea());
    }

    private void OnDisable()
    {
        if(m_CheckCoroutine != null)
        {
            StopCoroutine(m_CheckCoroutine);
            m_CheckCoroutine = null;
        }
    }

    void UpdateBox()
    {
        m_IOArea.minX = transform.position.x - m_Width / 2;
        m_IOArea.maxX = transform.position.x + m_Width / 2;

        m_IOArea.minZ = transform.position.z - m_Height / 2;
        m_IOArea.maxZ = transform.position.z + m_Height / 2;
    }

    IEnumerator Co_CheckArea()
    {
        var wait = new WaitForSeconds(m_CheckAreaInterval);

        while (true)
        {
            UpdateBox();

            bool isInsideNow = m_IOArea.IsInside(m_player.position);

            if(isInsideNow && !m_isPlayerEnter)
            {
                m_isPlayerEnter = true;
                m_OnEnterArea?.Invoke();
                Debug.Log("Player Enter Zone");
            }
            else if(!isInsideNow && m_isPlayerEnter)
            {
                m_isPlayerEnter = false;
                m_OnExitArea?.Invoke();
                Debug.Log("Player Exit Zone");
            }

            yield return wait;
        }
    }

    private void OnDrawGizmos()
    {
        UpdateBox();
        Gizmos.color = Color.blue;

        float width = m_IOArea.maxX - m_IOArea.minX;
        float depth = m_IOArea.maxZ - m_IOArea.minZ;
        float centerX = m_IOArea.minX + width / 2;
        float centerZ = m_IOArea.minZ + depth / 2;

        Gizmos.DrawCube(new Vector3(centerX, 0, centerZ), new Vector3(width, 1, depth));
    }
}
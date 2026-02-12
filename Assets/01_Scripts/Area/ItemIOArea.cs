using System;
using System.Collections;
using UnityEngine;


public class ItemIOArea : MonoBehaviour
{
    public event Action m_OnEnterArea;
    public event Action m_OnExitArea;

    [Header("Setting")]
    [SerializeField] private SOPlayerReference m_Player;
    [SerializeField] protected float m_CheckAreaInterval = 0.1f;
    [SerializeField] protected float m_Width;
    [SerializeField] protected float m_Height;
    [SerializeField] private bool m_isPlayerEnter = false;
    protected bool canDetect = true;
    RectZone m_IOArea = new()
    {
        minX = 0,
        minZ = 0,
        maxX = 0,
        maxZ = 0
    };

    private Coroutine m_CheckCoroutine;

    public Transform Player => m_Player.player.transform;
    public bool IsPlayerEnter => m_isPlayerEnter;

    protected virtual void Awake()
    {
        UpdateBox();
    }


    protected virtual void Start()
    {
        m_CheckCoroutine ??= StartCoroutine(Co_CheckArea());
    }

    protected virtual void OnEnable() { }

    protected virtual void OnDisable()
    {
        if(m_CheckCoroutine != null)
        {
            StopCoroutine(m_CheckCoroutine);
            m_CheckCoroutine = null;
        }
    }

    protected void UpdateBox()
    {
        m_IOArea.minX = transform.position.x - m_Width / 2;
        m_IOArea.maxX = transform.position.x + m_Width / 2;

        m_IOArea.minZ = transform.position.z - m_Height / 2;
        m_IOArea.maxZ = transform.position.z + m_Height / 2;

        transform.localScale = new Vector3(m_Width, .5f, m_Height);
    }

    protected IEnumerator Co_CheckArea()
    {
        var wait = new WaitForSeconds(m_CheckAreaInterval);

        while (canDetect)
        {
            UpdateBox();

            bool isInsideNow = m_IOArea.IsInside(m_Player.player.transform.position);

            if(isInsideNow && !m_isPlayerEnter)
            {
                m_isPlayerEnter = true;
                m_OnEnterArea?.Invoke();
            }
            else if(!isInsideNow && m_isPlayerEnter)
            {
                m_isPlayerEnter = false;
                m_OnExitArea?.Invoke();
            }

            yield return wait;
        }
    }

    protected void OnDrawGizmos()
    {
        UpdateBox();
        Gizmos.color = Color.green;

        float width = m_IOArea.maxX - m_IOArea.minX;
        float depth = m_IOArea.maxZ - m_IOArea.minZ;
        float centerX = m_IOArea.minX + width / 2;
        float centerZ = m_IOArea.minZ + depth / 2;

        Gizmos.DrawWireCube(new Vector3(centerX, 0, centerZ), new Vector3(width, 1, depth));
    }
}
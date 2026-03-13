using System;
using UnityEngine;


public class ItemIOArea : MonoBehaviour
{
    public event Action m_OnEnterArea;
    public event Action m_OnExitArea;

    [Header("Setting")]
    [SerializeField] protected float m_CheckAreaInterval = 0.1f;
    [SerializeField] protected float m_Width;
    [SerializeField] protected float m_Height;
    [SerializeField] private bool m_isPlayerEnter = false;
    protected bool canDetect = true;

    private Vector3 m_WorldCenter;
    private Vector3 m_AxisX;
    private Vector3 m_AxisZ;

    private Coroutine m_CheckCoroutine;

    public bool IsPlayerEnter => m_isPlayerEnter;

    protected virtual void Awake()
    {
        RecalculateOBB();
    }

    protected virtual void Start()
    {
    }

    protected virtual void OnEnable()
    {
        SpatialHashManager.Instance.SetArea(this);
    }

    protected virtual void OnDisable()
    {
        if(m_CheckCoroutine != null)
        {
            StopCoroutine(m_CheckCoroutine);
            m_CheckCoroutine = null;
        }

        SpatialHashManager.Instance.RemoveArea(this);
    }

    protected void RecalculateOBB()
    {
        if (!gameObject.activeSelf) return;
        transform.GetPositionAndRotation(out var pos, out var rot);
        m_WorldCenter = pos;

        m_AxisX = Rotate(rot, Vector3.right).normalized;
        m_AxisZ = Rotate(rot, Vector3.forward).normalized;

        transform.localScale = new Vector3(m_Width, .5f, m_Height);
    }

    private bool IsInsideOBB(Vector3 worldPos)
    {
        var d = worldPos - m_WorldCenter;

        float px = Vector3.Dot(d, m_AxisX);
        float pz = Vector3.Dot(d, m_AxisZ);

        float halfW = m_Width * 0.5f;
        float halfH = m_Height * 0.5f;

        return Mathf.Abs(px) <= halfW && Mathf.Abs(pz) <= halfH;
    }

    private static Vector3 Rotate(Quaternion q, Vector3 v)
    {
        // v' = v +2(s * (u x v) + u x (u x v)))
        var u = new Vector3(q.x, q.y, q.z);
        var s = q.w;

        Vector3 crossUV = Vector3.Cross(u, v);              // u x v
        Vector3 crossU_crossUV = Vector3.Cross(u, crossUV); // u x (u x v)

        return v + 2f * (s * crossUV + crossU_crossUV);
    }

    public bool CheckPlayer(Vector3 pos)
    {
        RecalculateOBB();
        return IsInsideOBB(pos);
    }

    public void Enter()
    {
        m_isPlayerEnter = true;
        m_OnEnterArea?.Invoke();
    }

    public void Exit()
    {
        m_isPlayerEnter = false;
        m_OnExitArea?.Invoke();
    }

    protected void OnDrawGizmos()
    {
        RecalculateOBB();
        Gizmos.color = Color.green;

        float halfW = m_Width * 0.5f;
        float halfD = m_Height * 0.5f;

        Vector3 p0 = m_WorldCenter + m_AxisX * (-halfW) + m_AxisZ * (-halfD);
        Vector3 p1 = m_WorldCenter + m_AxisX * (halfW) + m_AxisZ * (-halfD);
        Vector3 p2 = m_WorldCenter + m_AxisX * (halfW) + m_AxisZ * (halfD);
        Vector3 p3 = m_WorldCenter + m_AxisX * (-halfW) + m_AxisZ * (halfD);

        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p0);

        Gizmos.DrawSphere(m_WorldCenter, 0.05f);
    }
}
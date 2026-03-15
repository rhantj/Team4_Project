using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAreaDetector : MonoBehaviour
{
    [SerializeField] private float m_CheckInterval = .1f;

    private Coroutine m_CheckCo;
    private List<ItemIOArea> m_Areas = new();

    private void Start()
    {
        m_CheckCo = StartCoroutine(Co_CheckArea());
    }

    IEnumerator Co_CheckArea()
    {
        var wait = new WaitForSeconds(m_CheckInterval);

        while (true)
        {
            Vector3 playerPos = transform.position;
            m_Areas = SpatialHashManager.Instance.Query(playerPos);

            foreach(var area in m_Areas)
            {
                if (!area) continue;
                bool isInside = area.CheckPlayer(playerPos);

                if (isInside && !area.IsPlayerEnter)
                    area.Enter();
                else if (!isInside && area.IsPlayerEnter)
                    area.Exit();
            }

            yield return wait;
        }
    }
}
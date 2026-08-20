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

    private void OnDrawGizmos()
    {
        if (!SpatialHashManager.Instance) return;

        float cellSize = SpatialHashManager.Instance.CellSize;
        Vector3 pos = transform.position;

        int baseX = Mathf.FloorToInt(pos.x / cellSize);
        int baseZ = Mathf.FloorToInt(pos.z / cellSize);

        Gizmos.color = Color.yellow;

        for (int x = -1; x <= 1; ++x)
        {
            for (int z = -1; z <= 1; ++z)
            {
                int cx = baseX + x;
                int cz = baseZ + z;

                Vector3 center = new Vector3(
                    (cx + 0.5f) * cellSize,
                    0,
                    (cz + 0.5f) * cellSize
                );

                Vector3 size = new Vector3(cellSize, 0.1f, cellSize);

                Gizmos.DrawWireCube(center, size);
            }
        }

        if (m_Areas != null)
        {
            Gizmos.color = Color.red;

            foreach (var area in m_Areas)
            {
                if (!area) continue;
                Gizmos.DrawLine(transform.position, area.transform.position);
            }
        }
    }
}
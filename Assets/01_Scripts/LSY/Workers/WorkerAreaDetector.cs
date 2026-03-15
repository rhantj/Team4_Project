using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerAreaDetector : MonoBehaviour
{
    [SerializeField] private float m_CheckInterval = 0.1f;

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
            Vector3 position = transform.position;
            m_Areas = SpatialHashManager.Instance.Query(position);

            foreach (var area in m_Areas)
            {
                if (null == area) continue;
                bool isInside = area.CheckPlayer(position); // the method name is not appropriate, but preserve this.

                if (isInside && !area.IsWorkerEnter(gameObject)) area.WorkerEnter(gameObject);
                else if (!isInside && area.IsWorkerEnter(gameObject)) area.WorkerExit(gameObject);
            }

            yield return wait;
        }
    }
}

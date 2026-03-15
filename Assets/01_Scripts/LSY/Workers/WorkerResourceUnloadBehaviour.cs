using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Reworked
{
    [RequireComponent(typeof(Inventory))]
    public class WorkerResourceUnloadBehaviour : MonoBehaviour
    {
        [SerializeField] private float m_UnloadDelay;

        private Inventory m_Inventory;
        private CancellationTokenSource m_OnDisableCancellationTokenSource;
        private CancellationTokenSource m_UnloadTokenSource;
        private Awaitable m_UnloadDelayAwaitable;

        private void Awake()
        {
            if (!TryGetComponent(out m_Inventory) && Application.isEditor) Debug.LogError("Failed To get Inventory.");
        }

        private void OnEnable()
        {
            m_OnDisableCancellationTokenSource = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            m_OnDisableCancellationTokenSource.Cancel();
            m_OnDisableCancellationTokenSource = null;
        }

        public async Awaitable<ResourceItemData> UnloadAsync(List<ResourceItemData> requestedResourceItemData)
        {
            if (null != m_UnloadDelayAwaitable)
            {
                await m_UnloadDelayAwaitable;
                m_UnloadDelayAwaitable = null;
            }

            foreach (ResourceItemData item in requestedResourceItemData)
            {
                if (0 == m_Inventory.GetItemCount(item)) continue;
                m_Inventory.RemoveItem(item);
                m_UnloadDelayAwaitable = Awaitable.WaitForSecondsAsync(m_UnloadDelay, m_OnDisableCancellationTokenSource.Token);
                return item;
            }
            return null;
        }
    }
}

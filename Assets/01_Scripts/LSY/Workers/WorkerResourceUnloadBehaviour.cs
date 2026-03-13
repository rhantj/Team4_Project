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

        public async Awaitable<ResourceItemData> UnloadAsync(ResourceItemData requestedResourceItemData)
        {
            if (null != m_UnloadDelayAwaitable)
            {
                await m_UnloadDelayAwaitable;
                m_UnloadDelayAwaitable = null;
            }

            if (0 == m_Inventory.GetItemCount(requestedResourceItemData)) return null;
            m_Inventory.RemoveItem(requestedResourceItemData);
            m_UnloadDelayAwaitable = Awaitable.WaitForSecondsAsync(m_UnloadDelay, m_UnloadTokenSource.Token);
            return requestedResourceItemData;
        }
    }
}

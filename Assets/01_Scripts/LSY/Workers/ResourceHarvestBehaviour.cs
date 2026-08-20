using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Reworked
{
    [RequireComponent(typeof(Inventory))]
    public class ResourceHarvestBehaviour : MonoBehaviour, IHarvester
    {
        // avoid inefficient PlayerResourceCollector structure

        [field: Header("Resource Config")]
        [field: SerializeField] public List<ResourceType> TargetResourceTypes { get; private set; }
        [field: SerializeField] public float HarvestSpeedMultiplier { get; private set; }
        [field: Range(1, 10)][field: SerializeField] public int HarvestAmount { get; private set; }

        [field: Header("Harvest Status")]
        [field: ReadOnly][field: SerializeField] public bool IsHarvesting { get; private set; }

        [Header("UI References")]
        [ReadOnly(true)][SerializeField] private bool b_IsHarvestingUIActive = true;

        private Inventory m_Inventory;

        private CancellationTokenSource m_OnDisableCancellationTokenSource;

        // TODO: may possible to use a stack-like, list based structure to delete an element in the middle by just nullifying it.
        private LinkedList<IHarvestable> m_ContactedHarvestableQueueAsLinkedList;
        private IHarvestable m_InteractingHarvestable;
        private Awaitable<List<ResourceItemData>> m_HarvestAwaitable;
        private CancellationTokenSource m_HarvestAwaitableTokenSource;

        private ResourceNodeUI m_HarvestingUI;

        private void Awake()
        {
            if (!TryGetComponent(out m_Inventory) && Application.isEditor) Debug.LogError("Failed To get Inventory.");
            m_ContactedHarvestableQueueAsLinkedList = new LinkedList<IHarvestable>();
        }

        private void OnEnable()
        {
            if (b_IsHarvestingUIActive && !TryGetComponent(out m_HarvestingUI) && Application.isEditor) Debug.LogWarning("Failed To get PlayerUI.");
            m_OnDisableCancellationTokenSource = new CancellationTokenSource();

            IsHarvesting = false;

            LateFixedUpdateAsync();
        }

        private void OnDisable()
        {
            m_OnDisableCancellationTokenSource.Cancel();
            m_OnDisableCancellationTokenSource = null;

            m_ContactedHarvestableQueueAsLinkedList.Clear();
        }

        private void FixedUpdate()
        {
            Vector2 positionOnXZ = new Vector2(transform.position.x, transform.position.z);

            IEnumerable<GameObject> group = GameManager.Instance.GetService<GameObjectTaggedGroupCacheService>().GetTaggedGroupCache("Resource Node");
            if (null == group) return;

            foreach (GameObject resource in group)
            {
                if (null == resource) continue;
                if (!resource.activeInHierarchy) continue;
                if (!resource.TryGetComponent(out IHarvestable harvestable)) continue;
                if (!harvestable.IsContacted(positionOnXZ)) continue;
                if (m_ContactedHarvestableQueueAsLinkedList.Contains(harvestable)) continue;
                m_ContactedHarvestableQueueAsLinkedList.AddLast(harvestable);
            }

            List<IHarvestable> removalList = new List<IHarvestable>();
            foreach (IHarvestable harvestable in m_ContactedHarvestableQueueAsLinkedList)
            {
                if (null == harvestable) continue;
                if (null != harvestable && harvestable.IsContacted(positionOnXZ)) continue;
                removalList.Add(harvestable);
            }
            foreach (IHarvestable harvestable in removalList) m_ContactedHarvestableQueueAsLinkedList.Remove(harvestable);
        }

        private async void LateFixedUpdateAsync()
        {
            try
            {
                while (!m_OnDisableCancellationTokenSource.IsCancellationRequested)
                {
                    await Awaitable.FixedUpdateAsync(m_OnDisableCancellationTokenSource.Token);

                    // if the awaitable is valid and completed, process the result of the awaitable
                    if (null != m_HarvestAwaitable)
                    {
                        var awaiter = m_HarvestAwaitable.GetAwaiter();
                        if (awaiter.IsCompleted)
                        {
                            IsHarvesting = false;
                            foreach (ResourceItemData itemData in awaiter.GetResult()) if (!m_Inventory.IsFull) m_Inventory.AddItem(itemData);
                            m_HarvestAwaitable = null;
                        }
                    }

                    // if the interaction is exited, clear the interacting harvestable and cancel the token
                    if (null != m_InteractingHarvestable && !m_ContactedHarvestableQueueAsLinkedList.Contains(m_InteractingHarvestable))
                    {
                        m_HarvestAwaitableTokenSource?.Cancel();
                        IsHarvesting = false;
                        m_HarvestAwaitableTokenSource = null;
                        m_InteractingHarvestable = null;
                        m_HarvestAwaitable = null;
                    }

                    // if an interacting harvestable is null and inventory is still not full, find the next queued contacted harvestable
                    if (null == m_InteractingHarvestable && !m_Inventory.IsFull)
                    {
                        foreach (IHarvestable harvestable in m_ContactedHarvestableQueueAsLinkedList)
                        {
                            if (!harvestable.IsHarvestable) continue;
                            m_InteractingHarvestable = harvestable;
                            break;
                        }
                    }

                    // if awaitable is null, try to initiate awaitable
                    if (null != m_InteractingHarvestable && null == m_HarvestAwaitable)
                    {
                        if (null == m_HarvestAwaitableTokenSource) m_HarvestAwaitableTokenSource = CancellationTokenSource.CreateLinkedTokenSource(m_OnDisableCancellationTokenSource.Token);

                        // TODO: need to get remaining capacity from m_Inventory
                        int requestedHarvestAmount = /*Mathf.Min(HarvestAmount, m_Inventory);*/ HarvestAmount;

                        IsHarvesting = true;
                        m_HarvestAwaitable = m_InteractingHarvestable.HarvestAsync(this, HarvestSpeedMultiplier, requestedHarvestAmount, m_HarvestAwaitableTokenSource.Token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                //
                Debug.Log("OperationCanceledException");
            }
        }

        //private void OnTriggerEnter(Collider other) // or manually trigger it in fixedupdate with Physics.Overlap*** etc.
        //{
        //    if (other is not IHarvestable harvestable) return;
        //    if (!TargetResourceTypes.Contains(harvestable.TargetResourceType)) return;
        //    m_ContactedHarvestableQueueAsLinkedList.AddLast(harvestable);
        //}

        //private void OnTriggerExit(Collider other) // or manually trigger it in fixedupdate with Physics.Overlap*** etc.
        //{
        //    if (other is not IHarvestable harvestable) return;
        //    m_ContactedHarvestableQueueAsLinkedList.Remove(harvestable);
        //}

        public bool TryEnableUI(float progressNormalized)
        {
            if (!b_IsHarvestingUIActive) return false;
            if (null == m_HarvestingUI) return false;
            //m_HarvestingUI.ShowResourceInfo()
            //m_HarvestingUI.ShowLoadingBar(progressNormalized);
            return true;
        }

        public bool TryDisableUI()
        {
            if (!b_IsHarvestingUIActive) return false;
            if (null == m_HarvestingUI) return false;
            m_HarvestingUI.HideUI();
            return true;
        }

        public void AddTargetResource(ResourceType resourceType)
        {
            TargetResourceTypes.Add(resourceType);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Reworked
{
    public class ResourceNodeReworked : MonoBehaviour, IHarvestable
    {
        [Header("Resource")]
        [ReadOnly(true)][SerializeField] private ResourceType m_ResourceType = ResourceType.Wood;
        [ReadOnly(true)][SerializeField] private ResourceItemData m_ItemData;
        [ReadOnly(true)][Min(0)][SerializeField] private int m_DefaultResourceItemCount = 1;

        [Header("Harvest")]
        [ReadOnly(true)][Min(0)][SerializeField] private int m_MaxHarvestCount = 10;
        [ReadOnly(true)][Min(0f)][SerializeField] private float m_HarvestTime = 1f;
        [ReadOnly(true)][Min(0f)][SerializeField] private float m_HarvestContactDistance = 3f;

        [Header("Regeneration")]
        [ReadOnly(true)][Min(0)][SerializeField] private int m_RegeneratedResourceItemCountPerTick = 5;
        [ReadOnly(true)][Min(0f)][SerializeField] private float m_RegenerateTimePerTick = 5f;

        [Header("UI References")]
        [ReadOnly(true)][SerializeField] private bool b_IsWorldSpaceUIActive = true;

        public bool IsHarvestable { get => !b_IsBeingHarvested && 0 < m_CurrentResourceItemCount; }
        public ResourceType TargetResourceType { get; private set; }

        private bool b_IsBeingHarvested;
        private int m_CurrentResourceItemCount;
        private CancellationTokenSource m_OnDisableCancellationTokenSource;
        private WorldSpaceResourceUI m_WorldSpaceUI;

        private void Awake()
        {
            if (Application.isEditor && b_IsWorldSpaceUIActive && !TryGetComponent(out m_WorldSpaceUI)) Debug.LogWarning("Failed To get WorldSpaceUI.");
        }

        private void OnEnable()
        {
            b_IsBeingHarvested = false;
            m_CurrentResourceItemCount = m_DefaultResourceItemCount;
            m_OnDisableCancellationTokenSource = new CancellationTokenSource();

            RegenerateResourceItemAsync(m_OnDisableCancellationTokenSource.Token);
        }

        private void OnDisable()
        {
            m_OnDisableCancellationTokenSource.Cancel();
            m_OnDisableCancellationTokenSource = null;
        }

        public bool IsContacted(Vector2 positionOnXZ)
        {
            Vector2 resourcePositionOnXZ = new Vector2(transform.position.x, transform.position.z);
            return Vector2.Distance(resourcePositionOnXZ, positionOnXZ) <= m_HarvestContactDistance;
        }

        public async Awaitable<List<ResourceItemData>> HarvestAsync(IHarvester harvester, float harvestSpeedMultiplier, int requestedResourceItemAmount, CancellationToken externalToken)
        {
            using CancellationTokenSource linkedToken = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, externalToken);

            List<ResourceItemData> result = null;

            try
            {
                float elapsed = 0f;
                b_IsBeingHarvested = true;
                harvester.TryEnableUI(0f);

                do
                {
                    await Awaitable.FixedUpdateAsync(linkedToken.Token);

                    elapsed += harvester.HarvestSpeedMultiplier * Time.fixedDeltaTime;
                    harvester.TryEnableUI(Mathf.InverseLerp(0f, m_HarvestTime, elapsed));
                    if (b_IsWorldSpaceUIActive) m_WorldSpaceUI.ShowLoadingBar(Mathf.InverseLerp(0f, m_HarvestTime, elapsed));
                }
                while (elapsed < m_HarvestTime);

                int harvestedResourceItemCount = Mathf.Min(requestedResourceItemAmount, m_CurrentResourceItemCount);
                result = new List<ResourceItemData>(harvestedResourceItemCount);
                for (int i = 0; i < harvestedResourceItemCount; i++) result.Add(m_ItemData);
                m_CurrentResourceItemCount -= harvestedResourceItemCount;
            }
            catch (OperationCanceledException e)
            {
                if (Application.isEditor) Debug.LogError(e.Message);
            }
            finally
            {
                b_IsBeingHarvested = false;
                harvester.TryDisableUI();
                m_WorldSpaceUI?.HideUI();
            }
            return result;
        }

        private async void RegenerateResourceItemAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Awaitable.WaitForSecondsAsync(m_RegenerateTimePerTick, token);

                    m_CurrentResourceItemCount = Mathf.Min(m_DefaultResourceItemCount,
                                                           m_CurrentResourceItemCount + m_RegeneratedResourceItemCountPerTick);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}

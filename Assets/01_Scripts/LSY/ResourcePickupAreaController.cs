using Reworked;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(ItemIOArea))]
public class ResourcePickupAreaController : MonoBehaviour
{
    [SerializeField] private float m_PlayerLoadDuration;
    [ReadOnly(true)][SerializeField] private ResourceItemData m_ResourceItemData;
    [ReadOnly(true)][SerializeField] private float m_MaxResourceCount;

    // TODO: need to change to fetch player inventory dynamically
    [SerializeField] private InventoryExpended m_PlayerInventory;

    private ItemIOArea m_ItemIOArea;

    private Stack<ResourceItemData> m_StoredItems; // need serialization

    private Dictionary<WorkerResourceUnloadBehaviour, CancellationTokenSource> m_WorkerInteractionCancellationTokenSourceMapping;
    CancellationTokenSource m_PlayerInteractionCancellationTokenSource;

    private void Awake()
    {
        if (!TryGetComponent(out m_ItemIOArea) && Application.isEditor) Debug.LogError("Cannot find ItemIOArea.");
    }

    private void OnEnable()
    {
        m_StoredItems = new Stack<ResourceItemData>();

        m_WorkerInteractionCancellationTokenSourceMapping = new Dictionary<WorkerResourceUnloadBehaviour, CancellationTokenSource>();

        m_ItemIOArea.m_OnEnterAreaByPlayer += OnEnterAreaByPlayer;
        m_ItemIOArea.m_OnExitAreaByPlayer += OnExitAreaByPlayer;

        m_ItemIOArea.m_OnEnterAreaByWorker += OnEnterAreaByWorkerAction;
        m_ItemIOArea.m_OnExitAreaByWorker += OnExitAreaByWorkerAction;

    }

    private void OnDisable()
    {
        m_ItemIOArea.m_OnExitAreaByWorker -= OnExitAreaByWorkerAction;
        m_ItemIOArea.m_OnEnterAreaByWorker -= OnEnterAreaByWorkerAction;

        m_ItemIOArea.m_OnExitAreaByPlayer -= OnExitAreaByPlayer;
        m_ItemIOArea.m_OnEnterAreaByPlayer -= OnEnterAreaByPlayer;

        foreach (CancellationTokenSource tokenSource in m_WorkerInteractionCancellationTokenSourceMapping.Values) tokenSource.Cancel();
        m_WorkerInteractionCancellationTokenSourceMapping.Clear();
        m_WorkerInteractionCancellationTokenSourceMapping = null;

        m_PlayerInteractionCancellationTokenSource?.Cancel();
        m_PlayerInteractionCancellationTokenSource = null;

        m_StoredItems.Clear();
        m_StoredItems = null;
    }

    Action<WorkerResourceUnloadBehaviour> OnEnterAreaByWorkerAction => unloader => OnEnterAreaByWorker(unloader);
    Action<WorkerResourceUnloadBehaviour> OnExitAreaByWorkerAction => unloader => OnExitAreaByWorker(unloader);

    private void OnEnterAreaByWorker(WorkerResourceUnloadBehaviour unloader)
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        m_WorkerInteractionCancellationTokenSourceMapping.Add(unloader, tokenSource);
        WorkerUnloadAsync(unloader, tokenSource.Token);
    }

    private void OnExitAreaByWorker(WorkerResourceUnloadBehaviour unloader)
    {
        m_WorkerInteractionCancellationTokenSourceMapping[unloader].Cancel();
        m_WorkerInteractionCancellationTokenSourceMapping.Remove(unloader);
    }

    private async void WorkerUnloadAsync(WorkerResourceUnloadBehaviour unloader, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            ResourceItemData resourceItemData = await unloader.UnloadAsync(m_ResourceItemData);
            if (null != resourceItemData) m_StoredItems.Push(resourceItemData);
        }
    }

    private void OnEnterAreaByPlayer()
    {
        m_PlayerInteractionCancellationTokenSource = new CancellationTokenSource();
        PlayerLoadAsync(m_PlayerInteractionCancellationTokenSource.Token);
    }

    private void OnExitAreaByPlayer()
    {
        m_PlayerInteractionCancellationTokenSource.Cancel();
        m_PlayerInteractionCancellationTokenSource = null;
    }

    private async void PlayerLoadAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (m_StoredItems.TryPop(out ResourceItemData resourceItemData)) m_PlayerInventory.AddItem(resourceItemData);
            await Awaitable.WaitForSecondsAsync(m_PlayerLoadDuration);
        }
    }
}

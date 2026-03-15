using System;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(ResourcePickupAreaController))]
public class ResourcePickupAreaUpgrader : MonoBehaviour
{
    [field: Header("Input Rate")]
    [field: SerializeField] public float InputTickPeriod { get; private set; }
    [field: SerializeField] public int  InputPerTick { get; private set; }

    [field: Header("Upgrade Config")]
    [field: SerializeField] public int UpgradePrice { get; private set; }
    public GameObject WorkerPrefab;
    public GameObject UpgradeUI;
    public GameObject ControlUI;


    public int AccumulatedGold { get; private set; }

    private GameObjectPoolingService m_PoolingService;
    private SoundManager m_SoundManager;
    private const string m_CoinInputSound = "ITEM_Coin Buy";

    private ItemIOArea m_ItemIOArea;

    private CancellationTokenSource m_PlayerInteractionCancellationTokenSource;

    ResourcePickupAreaController controller;

    private InventoryExpended m_InventoryExpended;

    private void Awake()
    {
        m_PoolingService = GameManager.Instance.GetService<GameObjectPoolingService>();
        if (!TryGetComponent(out m_ItemIOArea) && Application.isEditor) Debug.LogError("Cannot find ItemIOArea.");
        if (!TryGetComponent(out controller) && Application.isEditor) Debug.LogError("Cannot find ResourcePickupAreaController.");
    }

    private void OnEnable()
    {
        m_SoundManager ??= GameManager.Instance.GetService<SoundManager>();

        m_ItemIOArea.m_OnEnterAreaByPlayer += OnEnterAreaByPlayer;
        m_ItemIOArea.m_OnExitAreaByPlayer += OnExitAreaByPlayer;

        m_InventoryExpended = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryExpended>();
    }

    private void OnDisable()
    {
        //m_PoolingService.ReturnOrDestroyGameObject(m_Worker);

        m_ItemIOArea.m_OnExitAreaByPlayer -= OnExitAreaByPlayer;
        m_ItemIOArea.m_OnEnterAreaByPlayer -= OnEnterAreaByPlayer;

        m_PlayerInteractionCancellationTokenSource?.Cancel();
        m_PlayerInteractionCancellationTokenSource = null;

    }

    private void OnEnterAreaByPlayer()
    {
        m_PlayerInteractionCancellationTokenSource = new CancellationTokenSource();
        PlayerUnloadAsync(m_PlayerInteractionCancellationTokenSource.Token);
    }

    private void OnExitAreaByPlayer()
    {
        m_PlayerInteractionCancellationTokenSource.Cancel();
        m_PlayerInteractionCancellationTokenSource = null;
    }

    private async void PlayerUnloadAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (m_InventoryExpended.Gold >= InputPerTick)
            {
                m_InventoryExpended.Gold -= InputPerTick;
                AccumulatedGold += InputPerTick;
                m_SoundManager.PlaySound(m_CoinInputSound, transform.position, Quaternion.identity);

            }

            if (AccumulatedGold >= UpgradePrice)
            {
                OnUpgrade();
                return;
            }

            await Awaitable.WaitForSecondsAsync(InputTickPeriod);
        }
    }

    private void OnUpgrade()
    {
        enabled = false;
        UpgradeUI.SetActive(false);

        controller.enabled = true;
        ControlUI.SetActive(true);

        GameObject worker = m_PoolingService.GetOrCreateGameObject(WorkerPrefab, transform.position, transform.rotation);
        controller.HandOverWorker(worker);
    }
}

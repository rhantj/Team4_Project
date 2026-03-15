using System;
using System.Threading;
using UnityEngine;

public class PickupAreaUpgraderViewModel : MonoBehaviour
{
    private ResourcePickupAreaUpgrader areaUpgraderModel;

    public SimpleBindableProperty<string> UpgradeStatusText { get; private set; }

    CancellationTokenSource m_OnDisableCancellationTokenSource;

    private void OnEnable()
    {
        m_OnDisableCancellationTokenSource = new CancellationTokenSource();

        areaUpgraderModel = GetComponent<ResourcePickupAreaUpgrader>();
        UpgradeStatusText = new SimpleBindableProperty<string>($"{areaUpgraderModel.AccumulatedGold} / {areaUpgraderModel.UpgradePrice}");

        OnAfterUpdate(m_OnDisableCancellationTokenSource.Token);
    }

    private void OnDisable()
    {
        m_OnDisableCancellationTokenSource.Cancel();
        m_OnDisableCancellationTokenSource = null;
    }

    private async void OnAfterUpdate(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await Awaitable.NextFrameAsync(token);

                UpgradeStatusText.Value = $"{areaUpgraderModel.AccumulatedGold} / {areaUpgraderModel.UpgradePrice}";
                // use UniRX/R3 Bindable Property to really use this... simple bindable property implementation is just a stopgap.
            }
            catch (OperationCanceledException)
            {
                //
            }
        }
    }
}

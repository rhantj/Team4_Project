using System;
using System.Threading;
using UnityEngine;

public class PickupAreaControllerViewModel : MonoBehaviour
{
    private ResourcePickupAreaController areaControllerModel;

    public SimpleBindableProperty<string> UpgradeStatusText { get; private set; }

    CancellationTokenSource m_OnDisableCancellationTokenSource;

    private void OnEnable()
    {
        m_OnDisableCancellationTokenSource = new CancellationTokenSource();

        areaControllerModel = GetComponent<ResourcePickupAreaController>();
        UpgradeStatusText = new SimpleBindableProperty<string>();

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

                UpgradeStatusText.Value = $"{areaControllerModel.StoredItems.Count} / {areaControllerModel.MaxResourceCount}";
                // use UniRX/R3 Bindable Property to really use this... simple bindable property implementation is just a stopgap.
            }
            catch (OperationCanceledException)
            {
                //
            }
        }
    }
}

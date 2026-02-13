using System;

public class FacilityPanelVM : IDisposable
{
    private readonly ProductionFacility _facility;

    public InputVM Input { get; private set; }
    public OutputVM Output { get; private set; }
    public UpgradeVM Upgrade { get; private set; }

    public FacilityPanelVM(ProductionFacility facility)
    {
        _facility = facility;

        Input = new InputVM(_facility);
        Output = new OutputVM(_facility);
        Upgrade = new UpgradeVM(_facility);

        _facility.m_OnInputChanged += Input.Refresh;
        _facility.m_OnOutputChanged += Output.Refresh;
        _facility.m_OnUpgradeChanged += OnUpgradeChanged;

        Upgrade.Refresh();
        Input.Refresh();
        Output.Refresh();
    }

    public void Dispose()
    {
        _facility.m_OnInputChanged -= Input.Refresh;
        _facility.m_OnOutputChanged -= Output.Refresh;
        _facility.m_OnUpgradeChanged -= OnUpgradeChanged;
    }

    private void OnUpgradeChanged()
    {
        Upgrade.Refresh();
        Input.Refresh();
        Output.Refresh();
    }
}
using System;

public class FacilityPanelVM : IDisposable
{
    private readonly ProductionFacility m_Facility;

    public InputVM Input { get; private set; }
    public OutputVM Output { get; private set; }
    public UpgradeVM Upgrade { get; private set; }

    public FacilityPanelVM(ProductionFacility model)
    {
        m_Facility = model;

        Input = new InputVM(m_Facility);
        Output = new OutputVM(m_Facility);
        Upgrade = new UpgradeVM(m_Facility);

        m_Facility.m_OnInputChanged += Input.Refresh;
        m_Facility.m_OnOutputChanged += Output.Refresh;
        m_Facility.m_OnUpgradeChanged += OnUpgradeChanged;

        Upgrade.Refresh(0);
        Input.Refresh();
        Output.Refresh();
    }

    public void Dispose()
    {
        m_Facility.m_OnInputChanged -= Input.Refresh;
        m_Facility.m_OnOutputChanged -= Output.Refresh;
        m_Facility.m_OnUpgradeChanged -= OnUpgradeChanged;
    }

    private void OnUpgradeChanged(int cost)
    {
        Upgrade.Refresh(cost);
        Input.Refresh();
        Output.Refresh();
    }
}
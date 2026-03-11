using System;

public class BuildingPanelVM : IDisposable
{
    private readonly Building m_Building;

    public BuildingInputVM Input { get; private set; }
    public ProgressDescriptionVM ProgressDescription { get; private set; }
    public BuildingInputVM NextProgress { get; private set; }
    public event Action<float> OnProgressChanged;

    public BuildingPanelVM(Building building)
    {
        this.m_Building = building;

        Input = new BuildingInputVM(m_Building);
        ProgressDescription = new ProgressDescriptionVM(m_Building);
        NextProgress = new BuildingInputVM(m_Building);

        m_Building.m_OnInputChanged += Input.Refresh;
        m_Building.m_OnInputChanged += ProgressDescription.Refresh;
        m_Building.m_OnInputChanged += NextProgress.Refresh;
    }

    public void Dispose()
    {
        m_Building.m_OnInputChanged -= Input.Refresh;
        m_Building.m_OnInputChanged -= ProgressDescription.Refresh;
        m_Building.m_OnInputChanged -= NextProgress.Refresh;
    }
}
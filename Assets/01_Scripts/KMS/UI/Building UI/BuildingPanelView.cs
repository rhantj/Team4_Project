using UnityEngine;

public class BuildingPanelView : MonoBehaviour, IBindable<Building>
{
    [SerializeField] private ProgressBar m_ProgressBar;
    [SerializeField] private BuildingInputView m_InputView;

    private BuildingPanelVM m_PanelVM;

    private void OnDisable()
    {
        Unbind();
    }

    public void Bind(Building vm)
    {
        Unbind();

        m_PanelVM = new BuildingPanelVM(vm);

        vm.m_OnProgressChanged += UpdateProgressBar;
        m_InputView.Bind(m_PanelVM.Input);

        m_PanelVM.Input.Refresh();
    }

    public void Unbind()
    {
        m_InputView.Unbind();

        m_PanelVM?.Dispose();
        m_PanelVM = null;
    }

    private void UpdateProgressBar(float f) =>
        m_ProgressBar.Value = f;
}
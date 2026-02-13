using UnityEngine;

public class FacilityPanelView : MonoBehaviour, IBindable<ProductionFacility>
{
    [SerializeField] private FacilityInputView m_InputView;
    [SerializeField] private FacilityOutputView m_OutputView;
    [SerializeField] private FacilityUpgradeView m_UpgradeView;

    private FacilityPanelVM m_PanelVM;

    public void Bind(ProductionFacility facility)
    {
        Unbind();

        m_PanelVM = new FacilityPanelVM(facility);

        m_InputView.Bind(m_PanelVM.Input);
        m_OutputView.Bind(m_PanelVM.Output);
        m_UpgradeView.Bind(m_PanelVM.Upgrade);

        m_PanelVM.Input.Refresh();
        m_PanelVM.Output.Refresh();
        m_PanelVM.Upgrade.Refresh();
    }

    public void Unbind()
    {
        m_InputView.Unbind();
        m_OutputView.Unbind();
        m_UpgradeView.Unbind();

        m_PanelVM?.Dispose();
        m_PanelVM = null;
    }

    private void OnDisable()
    {
        Unbind();
    }
}
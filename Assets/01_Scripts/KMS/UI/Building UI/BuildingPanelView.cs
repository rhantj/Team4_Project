using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanelView : MonoBehaviour, IBindable<Building>
{
    [SerializeField] private Slider m_ProgressBar;
    [SerializeField] private TMP_Text m_StageText;
    [SerializeField] private BuildingInputView m_InputView;
    [SerializeField] private ProgressDescriptionView m_ProgressDescriptionView;
    [SerializeField] private BuildingInputView m_NextProgressView;

    private BuildingPanelVM m_PanelVM;
    private StageManager m_StageManager;
    private Building m_Model;

    private void OnEnable()
    {
        m_StageManager = FindAnyObjectByType<StageManager>();
    }

    private void OnDisable()
    {
        Unbind();
    }

    public void Bind(Building model)
    {
        Unbind();

        m_Model = model;
        m_PanelVM = new BuildingPanelVM(model);

        model.m_OnProgressChanged += UpdateProgressBar;
        m_InputView.Bind(m_PanelVM.Input);
        m_ProgressDescriptionView.Bind(m_PanelVM.ProgressDescription);
        m_NextProgressView.Bind(m_PanelVM.NextProgress);
        m_StageManager.m_OnStageIdxChanged += UpdateStageText;

        UpdateProgressBar(0);
        m_PanelVM.Input.Refresh();
    }

    public void Unbind()
    {
        m_InputView.Unbind();
        m_ProgressDescriptionView.Unbind();
        m_NextProgressView.Unbind();

        if(m_Model != null)
            m_Model.m_OnProgressChanged -= UpdateProgressBar;

        if(m_StageManager != null)
            m_StageManager.m_OnStageIdxChanged -= UpdateStageText;

        m_PanelVM?.Dispose();
        m_PanelVM = null;
    }

    private void UpdateProgressBar(float f) =>
        m_ProgressBar.value = f;

    private void UpdateStageText(int stage) =>
        m_StageText.text = $"Stage {++stage}";
}
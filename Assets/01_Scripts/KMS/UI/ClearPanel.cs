using UnityEngine;
using UnityEngine.UI;

public class ClearPanel : UIPanel
{
    [SerializeField] private Image m_BuildingImage;
    [SerializeField] private Button m_NextStageBtn;
    [SerializeField] private Button m_MainLobbyBtn;
    [SerializeField] private Button m_GameQuitBtn;

    private StageManager m_StageManager;
    private UIManager m_UImanager;

    private void Start()
    {
        Close();

        m_StageManager = GameManager.Instance.gameObject.GetComponent<StageManager>();
        m_UImanager = GameManager.Instance.gameObject.GetComponent<UIManager>();
    }

    private void OnEnable()
    {
        m_NextStageBtn.onClick.AddListener(OnNextStageBtnClicked);
        m_MainLobbyBtn.onClick.AddListener(OnMainLobbyButtonClicked);
        m_GameQuitBtn.onClick.AddListener(OnGameQuitButtonClicked);
    }

    private void OnDisable()
    {
        m_NextStageBtn.onClick.RemoveAllListeners();
        m_MainLobbyBtn.onClick.RemoveAllListeners();
        m_GameQuitBtn.onClick.RemoveAllListeners();
    }

    private void OnMainLobbyButtonClicked()
    {
        m_StageManager.ClearStage();
        m_UImanager.SwapPanel();
        Close();
    }

    private void OnGameQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Applicayion.Quit();
#endif
    }

    private void OnNextStageBtnClicked()
    {
        int stage = m_StageManager.StageIdx;
        m_StageManager.BuildStage(stage + 1);
        Close();
    }
}
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Scrollbar m_Volume;

    [Header("Buttons")]
    [SerializeField] private Button m_MainLobbyBtn;
    [SerializeField] private Button m_GitLinkBtn;
    [SerializeField] private Button m_GameQuitBtn;
    [SerializeField] private Button m_ClosePanelBtn;

    private SoundManager m_SoundManager;
    private StageManager m_StageManager;
    private UIManager m_UImanager;

    private void Start()
    {
        gameObject.SetActive(false);

        m_SoundManager = GameManager.Instance.GetService<SoundManager>();
        m_StageManager = GameManager.Instance.gameObject.GetComponent<StageManager>();
        m_UImanager    = GameManager.Instance.gameObject.GetComponent<UIManager>();
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;

        m_Volume.onValueChanged.AddListener(UpdateVolume);

        if (m_MainLobbyBtn != null)
            m_MainLobbyBtn.onClick.AddListener(OnMainLobbyButtonClicked);

        m_GitLinkBtn.onClick.AddListener(OnGitLinkButtonClicked);
        m_GameQuitBtn.onClick.AddListener(OnGameQuitButtonClicked);
        m_ClosePanelBtn.onClick.AddListener(OnClosedButtonClicked);
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;

        m_Volume.onValueChanged.RemoveAllListeners();

        if(m_MainLobbyBtn != null)
            m_MainLobbyBtn.onClick.RemoveAllListeners();

        m_GitLinkBtn.onClick.RemoveAllListeners();
        m_GameQuitBtn.onClick.RemoveAllListeners();
        m_ClosePanelBtn.onClick.RemoveAllListeners();
    }

    private void UpdateVolume(float value)
    {
        m_SoundManager.MasterVolume = value;
    }

    private void OnMainLobbyButtonClicked()
    {
        m_StageManager.ClearStage();
        m_UImanager.SwapPanel();
        ClosePanel();
    }

    private void OnGameQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Applicayion.Quit();
#endif
    }

    private void OnClosedButtonClicked()
    {
        ClosePanel();
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    private void OnGitLinkButtonClicked()
    {
        Application.OpenURL("https://github.com/rhantj/Team4_Project");
    }
}
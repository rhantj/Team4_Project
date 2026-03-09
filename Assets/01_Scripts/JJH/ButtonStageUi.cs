using UnityEngine;
using UnityEngine.UI;

public class ButtonStageUi : MonoBehaviour
{
    [Header("옵션창")]
    public GameObject m_OptionPannel;
    public Button m_OpenOption;
    public Button m_CloseOption;
    public Button m_GitLink;
    public Button m_GameQuit;

    [Header("게임클리어창")]
    public GameObject m_ClearPannel;
    public Button m_NextStage;
    public Button m_MainLobby;
    public Button m_GameQuit2;

    private void Start()
    {
        if (m_OpenOption != null)
        {
            m_OpenOption.onClick.AddListener(OpenOption);
        }
        if(m_CloseOption != null)
        {
            m_CloseOption.onClick.AddListener(CloseOption);
        }
        if (m_OptionPannel != null)
        {
            m_OptionPannel.SetActive(false);
        }

        if (m_GitLink!=null)
        {
            m_GitLink.onClick.AddListener(GitLink);
        }

        if(m_GameQuit != null)
        {
            m_GameQuit.onClick.AddListener(OnApplicationQuit);
        }




        
    }

    private void OpenOption()
    {
        if (m_OptionPannel != null)
        {
            m_OptionPannel.SetActive(true);
            Time.timeScale = 0f; // 게임 일시정지
        }
    }
    private void CloseOption()
    {
        if (m_OptionPannel != null)
        {
            m_OptionPannel.SetActive(false);
            Time.timeScale = 1f; // 게임 재개
        }
    }
    private void GitLink()
    {
        Application.OpenURL("https://github.com/rhantj/Team4_Project");

    }
    private void OnApplicationQuit()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }
    private void OpenClearPannel()
    {
        if(m_ClearPannel != null)
        {
            m_ClearPannel.SetActive(true);
            Time.timeScale = 0f; // 게임 일시정지
        }
    }
}




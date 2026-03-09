using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMainLobby : MonoBehaviour
{
    //버튼 연결어떻게할지
    //1.노가다로 [SerializeField] 만들어서 넣기
    //2.버튼 이름? 으로 자동으로 연결하기
    //3.이벤트 트리거
    //4.정의된 인터페이스 활용하기

    [Header("옵션창 버튼")]
    public GameObject m_OptionPannel;
    public Button m_OpenOption;
    public Button m_CloseOption;

    [Header("건물정보")]
    public GameObject m_BuildingInfoPannel;
    public Button m_OpenBuildingInfo;
    public Button m_CloseBuildingInfo;
    public GameObject m_InfoContent;
    public Image m_InfoPanel;
    public Image m_InfoImage;
    public Button m_InfoCloseButton;


    [Header("게임종료")]
    public Button m_QuitGame;
    public Button m_Option_QuitGame;

    [Header("GitLink")]
    public Button m_GitLink;

    [Header("팀 소개")]
    public Image m_TeamIcon;
    public Image m_TeamName;


    private void Start()
    {
        //옵션창
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
        //


        //건물정보창
        if(m_OpenBuildingInfo != null)
        {
            m_OpenBuildingInfo.onClick.AddListener(OpenBuildingOption);
        }
        if(m_CloseBuildingInfo != null)
        {
            m_CloseBuildingInfo.onClick.AddListener(CloseBuildingOption);
        }
        if(m_BuildingInfoPannel != null)
        {
            m_BuildingInfoPannel.SetActive(false);
        }
        Button[]infoBtn= m_InfoContent.GetComponentsInChildren<Button>();
        foreach(Button btn in infoBtn)
        {
            Image image= btn.GetComponent<Image>();

            if (image != null)
            {
                btn.onClick.AddListener(() => DetailImage(image.sprite));
            }
        }
        if(m_InfoCloseButton != null)m_InfoCloseButton.onClick.AddListener(CloseDetailImage);

        //

        //게임종료
        if (m_QuitGame != null)
        {
            m_QuitGame.onClick.AddListener(GameQuit);
        }
        if(m_Option_QuitGame !=null)m_Option_QuitGame.onClick.AddListener(GameQuit);

        //GitLink
        if(m_GitLink!=null)m_GitLink.onClick.AddListener(GitLink);



    }

    //옵션창
    private void OpenOption()
    {
        m_OptionPannel.SetActive(true);
    }
    private void CloseOption()=>m_OptionPannel.SetActive(false);
    //

    //건물정보창
    private void OpenBuildingOption()
    {
        m_BuildingInfoPannel.SetActive(true);
    }
    private void CloseBuildingOption() => m_BuildingInfoPannel.SetActive(false);

    public void DetailImage(Sprite sprite)
    {

        m_InfoImage.sprite = sprite;

        m_InfoPanel.gameObject.SetActive(true);

    }
    private void CloseDetailImage()
    {
        m_InfoPanel.gameObject.SetActive(false);
    }
    //

    //게임종료
    private void GameQuit()
    {
        //Debug.Log("게임종료");
        m_QuitGame.onClick.AddListener(Application.Quit);
        m_Option_QuitGame.onClick.AddListener(Application.Quit);
    }

    //GitLink
    private void GitLink()
    {
        //Debug.Log("GitLink");
        Application.OpenURL("https://github.com/rhantj/Team4_Project");
    }

    //팀 소개아이콘
    public void ShowImage()
    {
        if(m_TeamName!=null)m_TeamName.gameObject.SetActive(true);
    }
    public void HideImaeg()
    {
        if(m_TeamName!=null)m_TeamName.gameObject.SetActive(false);
    }




}

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
        //
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
    //


}

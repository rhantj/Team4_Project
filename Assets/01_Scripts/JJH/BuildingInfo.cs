using UnityEngine;
using UnityEngine.UI;

public class BuildingInfo : MonoBehaviour
{
    [Header("건물 정보")]
    public Image m_BuildingInfo;
    public Button m_CloseBTN;

    private void Start()
    {
        if(m_CloseBTN!=null)m_CloseBTN.onClick.AddListener(CloseBuildingInfo);
    }

    private void CloseBuildingInfo()
    {
        if(m_BuildingInfo!=null)m_BuildingInfo.gameObject.SetActive(false);
    }



}

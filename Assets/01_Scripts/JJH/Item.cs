using UnityEngine;

public class Item : MonoBehaviour
{
    /*프리팹에 붙일 스크립트
    so를 받고,so말고 아이템스크립트들을 만들어야된다
    프리팹에 스크립트를 넣어야되고
    스크립트를 넣은 프리팹도 so에 들어가야된다
    so를 만드는게아니라 스크립트가붙어있는 프리팹을만든다
    +여기서 아이템의 날아가는 기능 및ㅇ ㅣ팩트들 추가

    인벤
    아이템스크립트를 등에쌓기

    생성
    아이템스크립트를 생성
    제거
    아이템스크립트를 제거

    아이템스크립트
    날아가는 기능
    아이템의 기능
    */
    public ResourceItemData m_ItemData;

    public string m_ItemName;

    private bool m_IsFlying=false;

    Vector3 m_targetPos;

    private void Awake()
    {
        if(m_ItemData!=null)
        {
            m_ItemName = m_ItemData.m_SoItemName;
        }
    }
    public void FlyTo(Vector3 targetPos, System.Action value)
    {
        m_targetPos = targetPos;
        m_IsFlying = true;
    }


}

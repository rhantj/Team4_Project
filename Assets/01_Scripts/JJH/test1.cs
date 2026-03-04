using UnityEngine;
using UnityEngine.UI;

public class test1 : MonoBehaviour
{
    //버튼에 직접 스크립트 연결안하고 이 스크립트를 추가한곳에 인스펙터에 버튼을 넣기만하면됨
    public Button test;
    private void Start()
    {
        test.onClick.AddListener(() => Test1());
    }
    public void Test1()
    {
        Debug.Log("test1");
    }



    //이 스크립트가적용된 오브젝트를 버튼의 온클릭으로 연결해서 지정해야됨(인스펙터상에서 연결을 안해도됨)
    public void Test2()
    {
        Debug.Log("test2");
    }
}

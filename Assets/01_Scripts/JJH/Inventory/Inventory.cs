using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //용량
    [SerializeField] private int capacity = 10;
    //아이템 위치
    [SerializeField] private Transform itemSpawnPoint;

    public bool IsFull => stackItem.Count >= capacity;

    private List<GameObject>stackItem=new List<GameObject>();

    public void AddItem(ResourceItemData data)
    {
        //가득참 체크
        if (IsFull)
        {
            Debug.Log("인벤토리 가득참");
            return;
        }
        //아이템 생성
        GameObject resourceItem = Instantiate(data.itemPrefab);
        resourceItem.transform.SetParent(itemSpawnPoint,false);

        //아이템 높이계산
        float height = 0f;
        foreach(GameObject obj in stackItem)
        {
            height+=obj.GetComponentInChildren<Renderer>().bounds.size.y;
        }

        resourceItem.transform.localPosition = new Vector3(0, height, 0);


        stackItem.Add(resourceItem);
    }
    public void RemoveItem()
    {
        if (stackItem.Count > 0)
        {
            int lastIndex = stackItem.Count - 1;
            GameObject removeItem=stackItem[lastIndex];

            stackItem.RemoveAt(lastIndex);

            //제거는 나중에 수정할예정
            Destroy(removeItem);


        }
    }

}

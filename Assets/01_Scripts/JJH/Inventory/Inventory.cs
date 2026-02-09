using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    
    [Header("가방 용량")]
    [SerializeField] private int m_Capacity = 10;
    [Header("아이템 생성 위치")]
    [SerializeField] private Transform m_ItemSpawnPoint;

    public bool IsFull => m_StackItem.Count >= m_Capacity;

    public bool IsEmpty => m_StackItem.Count == 0;

    //private List<GameObject>m_StackItem=new List<GameObject>();

    private List<Item> m_StackItem = new List<Item>();

    public void AddItem(ResourceItemData data)
    {
        if (IsFull) return;

        GameObject resourceItem = Instantiate(data.m_ItemPrefab);

        Item item=resourceItem.GetComponent<Item>();

        resourceItem.transform.SetParent(m_ItemSpawnPoint, false);

        float height = 0f;

        foreach (Item stackItem in m_StackItem)
        {
            height += stackItem.GetComponentInChildren<Renderer>().bounds.size.y;
        }

        resourceItem.transform.localPosition = new Vector3(0, height, 0);

        m_StackItem.Add(item);

    }   
    //public void AddItem(ResourceItemData data)
    //{
    //    //가득참 체크
    //    if (IsFull)
    //    {
    //        Debug.Log("인벤토리 가득참");
    //        return;
    //    }
    //    //아이템 생성
    //    GameObject resourceItem = Instantiate(data.m_ItemPrefab);
    //    resourceItem.transform.SetParent(m_ItemSpawnPoint,false);

    //    resourceItem.name = data.m_ItemName;

    //    //아이템 높이계산
    //    float height = 0f;
    //    foreach(GameObject obj in m_StackItem)
    //    {
    //        height+=obj.GetComponentInChildren<Renderer>().bounds.size.y;
    //    }

    //    resourceItem.transform.localPosition = new Vector3(0, height, 0);


    //    m_StackItem.Add(resourceItem);
    //}
    public void RemoveItem(ResourceItemData data)
    {
        for(int i=m_StackItem.Count-1;i>=0;i--)
        {
            if (m_StackItem[i].m_ItemData == data)
            {
                Item itemRemove = m_StackItem[i];
                m_StackItem.RemoveAt(i);

                Destroy(itemRemove.gameObject);

                SortItem();
                return;
            }
        }
    }
    //public void RemoveItem(ResourceItemData data)
    //{
    //    for(int i=m_StackItem.Count-1;i>=0;i--)
    //    {
    //        if(m_StackItem[i].name==data.m_ItemName)
    //        {
    //            GameObject removeItem=m_StackItem[i];
    //            m_StackItem.RemoveAt(i);
    //            Destroy(removeItem);
    //            SortItem();
    //            return;
    //        }
    //    }
    //}
    public void SortItem()
    {
        float height = 0f;

        for(int i=0;i<m_StackItem.Count;i++)
        {
            m_StackItem[i].transform.localPosition = new Vector3(0, height, 0);
            height += m_StackItem[i].GetComponentInChildren<Renderer>().bounds.size.y;
        }
    }
    //public void RemoveItem(ResourceItemData data)
    //{
    //    if (m_StackItem.Count > 0)
    //    {
    //        int lastIndex = m_StackItem.Count - 1;

    //        if (m_StackItem[lastIndex].name == data.m_ItemName)
    //        {
    //            GameObject removeItem=m_StackItem[lastIndex];
    //            m_StackItem.RemoveAt(lastIndex);
    //            Destroy(removeItem);
    //        }

            
    //    }
    //}
    //public void RemoveItem()
    //{
    //    if (m_StackItem.Count > 0)
    //    {
    //        int lastIndex = m_StackItem.Count - 1;
    //        GameObject removeItem=m_StackItem[lastIndex];

    //        m_StackItem.RemoveAt(lastIndex);

    //        //제거는 나중에 수정할예정
    //        Destroy(removeItem);


    //    }
    //}
    public Vector3 GetNextItemPos()
    {
        float yOffset = 0f;
        foreach(var item in m_StackItem)
        {
            yOffset+=item.GetComponentInChildren<Renderer>().bounds.size.y;
        }
        return m_ItemSpawnPoint.position + (m_ItemSpawnPoint.up * yOffset);
    }
    public void AdditemList(Item item)
    {
        item.transform.SetParent(m_ItemSpawnPoint);
        item.transform.localRotation=Quaternion.identity;
        m_StackItem.Add(item);
    }
}

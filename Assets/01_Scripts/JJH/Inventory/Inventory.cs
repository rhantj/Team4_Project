using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Inventory : MonoBehaviour
{
    
    [Header("가방 용량")]
    [SerializeField] private int m_Capacity = 10;
    [Header("아이템 생성 위치")]
    [SerializeField] protected Transform m_ItemSpawnPoint;

    public bool IsFull => m_StackItem.Count >= m_Capacity;

    public bool IsEmpty => m_StackItem.Count == 0;

    protected List<Item> m_StackItem = new List<Item>();

    //public void AddItem(ResourceItemData data)
    //{
    //    if (IsFull) return;

    //    GameObject resourceItem = Instantiate(data.m_ItemPrefab);
    //    Item item = resourceItem.GetComponentInChildren<Item>();

    //    item.transform.SetParent(m_ItemSpawnPoint, false);
    //    item.transform.localRotation = Quaternion.identity;

    //    var renderer = item.GetComponentInChildren<Renderer>();

    //    float currentY = m_ItemSpawnPoint.position.y;
    //    if (m_StackItem.Count > 0)
    //    {
    //        var lastRenderer = m_StackItem[m_StackItem.Count - 1].GetComponentInChildren<Renderer>();
    //        currentY = lastRenderer.bounds.max.y;
    //    }

    //    Vector3 itemCenter = item.transform.position - renderer.bounds.center;

    //    float itemX = m_ItemSpawnPoint.position.x + itemCenter.x;
    //    float itemZ = m_ItemSpawnPoint.position.z + itemCenter.z;

    //    float itemBtm = item.transform.position.y - renderer.bounds.min.y;
    //    float itemY = currentY + itemBtm;

    //    item.transform.position = new Vector3(itemX, itemY, itemZ);

    //    m_StackItem.Add(item);



    //}
    public void AddItem(ResourceItemData data)
    {
        if (IsFull) return;

        GameObject resourceItem = Instantiate(data.m_ItemPrefab);

        Item item = resourceItem.GetComponent<Item>();

        resourceItem.transform.SetParent(m_ItemSpawnPoint, false);

        float height = 0f;

        foreach (Item stackItem in m_StackItem)
        {
            height += stackItem.GetComponentInChildren<Renderer>().bounds.size.y;
        }

        resourceItem.transform.localPosition = new Vector3(0, height, 0);

        m_StackItem.Add(item);

    }

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
    
    public void SortItem()
    {
        float height = 0f;

        for(int i=0;i<m_StackItem.Count;i++)
        {
            m_StackItem[i].transform.localPosition = new Vector3(0, height, 0);
            height += m_StackItem[i].GetComponentInChildren<Renderer>().bounds.size.y;
        }
    }
    
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

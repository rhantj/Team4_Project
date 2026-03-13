using System;
using UnityEngine;

public class InventoryExpended : Inventory
{
    private int m_Gold;
    public int Gold
    {
        get { return m_Gold; }
        set
        {
            m_Gold = value;
            m_OnGoldChanged?.Invoke(m_Gold);
        }
    }

    public event Action<int> m_OnGoldChanged;

    public bool TryRemoveItemByName(string itemName)
    {
        for (int i = m_StackItem.Count - 1; i >= 0; --i)
        {
            if (m_StackItem[i].m_ItemData.m_SoItemName == itemName)
            {
                Item removeItem = m_StackItem[i];
                m_StackItem.RemoveAt(i);

                Destroy(removeItem.gameObject);
                SortItem();

                NotifyCountChanged();
                return true;
            }
        }
        return false;
    }

    public void AddItem(GameObject item)
    {
        if (IsFull) return;
        if (!item) return;

        GameObject resItem = Instantiate(item);

        Item i = resItem.GetComponent<Item>();
        if (!i)
        {
            Destroy(resItem);
            return;
        }

        resItem.transform.SetParent(m_ItemSpawnPoint, false);

        float height = 0f;
        foreach (Item stackItem in m_StackItem)
        {
            height += stackItem.GetComponentInChildren<Renderer>().bounds.size.y;
        }

        resItem.transform.localPosition = new Vector3(0, height, 0);

        m_StackItem.Add(i);
        NotifyCountChanged();
    }
}
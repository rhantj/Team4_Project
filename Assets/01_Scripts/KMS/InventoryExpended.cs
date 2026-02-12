using UnityEngine;

public class InventoryExpended : Inventory
{
    public bool HasItem(Item item)
    {
        if (m_StackItem.Contains(item))
        {
            return true;
        }
        return false;
    }

    public float Gold { get; set; }

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
    }
}
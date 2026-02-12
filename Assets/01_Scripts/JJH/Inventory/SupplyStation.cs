using UnityEngine;

public class SupplyStation : MonoBehaviour
{
    //리소스 아이템SO
    [SerializeField]private ResourceItemData m_SoItemData;
    //공급 간격(낮을수록빨라짐)
    [SerializeField]private float m_SupplyInterval = 1f;
    private float m_SupplyTimer = 0f;
    

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inven=other.GetComponent<Inventory>();

            if (inven != null&&!inven.IsFull)
            {
                m_SupplyTimer += Time.deltaTime;
                if(m_SupplyTimer >= m_SupplyInterval)
                {
                    //GameObject obj = Instantiate(m_SoItemData.m_ItemPrefab, transform.position, Quaternion.identity);
                    //Item item=obj.GetComponent<Item>();

                    //if (item != null)
                    //{
                    //    Vector3 targetPos=inven.GetNextItemPos();

                    //    item.FlyTo(targetPos,() =>
                    //    {
                    //        inven.AddItem(m_SoItemData);
                    //    });

                    //    m_SupplyTimer= 0f;
                    //}

                    inven.AddItem(m_SoItemData);
                    m_SupplyTimer = 0f;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_SupplyTimer = 0f;
        }
    }
        
}

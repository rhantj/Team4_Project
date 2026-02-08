using UnityEngine;

public class RemoveStation : MonoBehaviour
{
    [SerializeField]private ResourceItemData m_SoItemData;
    //薯剪 除問(雪擊熱煙說塭颶)
    [SerializeField]private float m_RemoveInterval = 1f;
    private float m_RemoveTimer = 0f;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Inventory inven=other.GetComponent<Inventory>();
            if (inven != null)
            {
                m_RemoveTimer += Time.deltaTime;
                if(m_RemoveTimer >= m_RemoveInterval)
                {
                    inven.RemoveItem(m_SoItemData);
                    m_RemoveTimer = 0f;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_RemoveTimer = 0f;
        }
    }
    
}

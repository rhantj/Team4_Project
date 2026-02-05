using UnityEngine;

public class SupplyStation : MonoBehaviour
{
    //리소스 아이템SO
    [SerializeField]private ResourceItemData SOItemData;
    //공급 간격(낮을수록빨라짐)
    [SerializeField]private float supplyInterval = 1f;
    private float supplyTimer = 0f;
    

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inven=other.GetComponent<Inventory>();

            if (inven != null&&!inven.IsFull)
            {
                supplyTimer += Time.deltaTime;
                if(supplyTimer >= supplyInterval)
                {
                    inven.AddItem(SOItemData);
                    supplyTimer = 0f;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            supplyTimer = 0f;
        }
    }
        
}

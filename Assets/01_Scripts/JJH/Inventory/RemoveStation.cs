using UnityEngine;

public class RemoveStation : MonoBehaviour
{
    //薯剪 除問(雪擊熱煙說塭颶)
    [SerializeField]private float removeInterval = 1f;
    private float removeTimer = 0f;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Inventory inven=other.GetComponent<Inventory>();
            if (inven != null)
            {
                removeTimer += Time.deltaTime;
                if(removeTimer >= removeInterval)
                {
                    inven.RemoveItem();
                    removeTimer = 0f;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            removeTimer = 0f;
        }
    }
    
}

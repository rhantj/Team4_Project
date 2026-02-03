using UnityEngine;

public class RemoveStation : MonoBehaviour
{
    [SerializeField]private float removeInterval = 1f;
    private float removeTimer = 0f;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(other.CompareTag("Player"))
            {
                removeTimer+=Time.deltaTime;
                if(removeTimer>=removeInterval)
                {
                    Inventory playerInventory = other.GetComponent<Inventory>();
                    if(playerInventory != null)
                    {
                        playerInventory.RemoveItem();
                        
                    }
                    removeTimer=0f;
                }

            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            removeTimer=0f;
        }
    }
}

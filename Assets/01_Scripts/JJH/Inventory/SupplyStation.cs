using UnityEngine;

public class SupplyStation : MonoBehaviour
{

    [SerializeField]private float supplyInterval = 1f;
    private float supplyTimer = 0f;

    

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            supplyTimer+=Time.deltaTime;
            if(supplyTimer>=supplyInterval)
            {
                Inventory playerInventory = other.GetComponent<Inventory>();

                if(playerInventory != null)
                {
                    playerInventory.AddItem2();
                    
                }
                supplyTimer=0f;
            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            supplyTimer=0f;
        }
    }
}

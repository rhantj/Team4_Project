using UnityEngine;

public class PlayerStat : MonoBehaviour
{

    [Header("Player Stats")]
    [SerializeField] private float interactionRange = 2f;



    public float InteractionRange => interactionRange;

    private void Awake()
    {
        
    }

   

}

using UnityEngine;

public class PlayerAutoCollector : MonoBehaviour
{


    [Header("Collection Settings")]
    [SerializeField] private float collectionRange = 2f;
    [SerializeField] private LayerMask collectableLayer;
    [SerializeField] private float checkInterval = 0.2f;

    private PlayerAnimator playerAnimator;
    private Collider[] hitBuffer = new Collider[10];
    private float checkTimer = 0f;
    private ICollectable currentTarget = null;

    void Start()
    {
        
    }

 
    void Update()
    {
        
    }
}

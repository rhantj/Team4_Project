using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{


    [Header("PlayerInteraction Settings")]
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.E;


    private PlayerStat playerStat;
    private Collider[] hitBuffer = new Collider[10];




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;

public class PlayerResourceCollector : MonoBehaviour
{
    [Header("Collection Settings")]
    [SerializeField] private float collectionRange = 2f;
    [SerializeField] private LayerMask resourceLayer;

    private ResourceStack resourceStack; // 추가!
    private Collider[] hitBuffer = new Collider[10];
    private float checkTimer = 0f;
    private ResourceNode currentTarget = null;

    private void Awake()
    {
        resourceStack = GetComponent<ResourceStack>(); // 추가!
    }

    private void Update()
    {
        checkTimer += Time.deltaTime;

        if (checkTimer >= 0.2f)
        {
            checkTimer = 0f;
            CheckForResources();
        }
    }

    private void CheckForResources()
    {
        if (currentTarget != null && !currentTarget.CanHarvest)
            currentTarget = null;

        if (currentTarget != null)
            return;

        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            collectionRange,
            hitBuffer,
            resourceLayer
        );

        if (hitCount > 0)
        {
            ResourceNode closestNode = FindClosestResource(hitCount);

            if (closestNode != null && closestNode.CanHarvest)
            {
                StartHarvest(closestNode);
            }
        }
    }

    private ResourceNode FindClosestResource(int hitCount)
    {
        float closestDistance = float.MaxValue;
        ResourceNode closest = null;

        for (int i = 0; i < hitCount; i++)
        {
            ResourceNode node = hitBuffer[i].GetComponent<ResourceNode>();

            if (node != null && node.CanHarvest)
            {
                float distance = Vector3.Distance(
                    transform.position,
                    hitBuffer[i].transform.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = node;
                }
            }
        }

        return closest;
    }

    private void StartHarvest(ResourceNode node)
    {
        currentTarget = node;
        node.StartHarvest(resourceStack); // resourceStack 전달!

    
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collectionRange);
    }
}
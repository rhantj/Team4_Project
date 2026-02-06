using UnityEngine;

public class PlayerResourceCollector : MonoBehaviour
{
    [Header("Collection Settings")]
    [SerializeField] private float collectionRange = 2f;
    [SerializeField] private LayerMask resourceLayer;

    private Inventory inventory;
    private Collider[] hitBuffer = new Collider[10];
    private float checkTimer = 0f;
    private ICollectable currentTarget = null; // ICollectable

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
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
        // ICollectable 사용
        if (currentTarget != null && !currentTarget.CanCollect())
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
            ICollectable closest = FindClosestCollectable(hitCount);

            if (closest != null && closest.CanCollect())
            {
                StartHarvest(closest);
            }
        }
    }

    private ICollectable FindClosestCollectable(int hitCount)
    {
        float closestDistance = float.MaxValue;
        ICollectable closest = null;

        for (int i = 0; i < hitCount; i++)
        {
            // ICollectable로 검색
            ICollectable collectable = hitBuffer[i].GetComponent<ICollectable>();

            if (collectable != null && collectable.CanCollect())
            {
                float distance = Vector3.Distance(
                    transform.position,
                    hitBuffer[i].transform.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = collectable;
                }
            }
        }

        return closest;
    }

    private void StartHarvest(ICollectable collectable)
    {
        currentTarget = collectable;

        // ResourceNode에 인벤토리 전달
        if (collectable is ResourceNode node)
        {
            node.SetInventory(inventory);
        }

        collectable.Collect(); // ICollectable.Collect() 호출

        Debug.Log("자원 수집 시작!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collectionRange);
    }
}
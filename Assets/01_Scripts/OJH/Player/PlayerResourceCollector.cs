using UnityEngine;
using System.Collections;

public class PlayerResourceCollector : MonoBehaviour
{
    [Header("Collection Settings")]
    [SerializeField] private float collectionRange = 3f;
    [SerializeField] private LayerMask resourceLayer;

    public float CollectionRange => collectionRange;

    private Inventory inventory;
    private Collider[] hitBuffer = new Collider[10];
    private float checkTimer = 0f;
    private ICollectable currentTarget = null;
    private bool isCancelCooldown = false;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            ResourceNode node = currentTarget as ResourceNode;
            if (node != null)
            {
                // 수집 완료 감지
                if (!node.IsBeingHarvested())
                {
                    currentTarget = null;
                    return;
                }

                // 범위 이탈 감지
                float distance = Vector3.Distance(transform.position, node.transform.position);
                if (distance > collectionRange)
                {
                    node.CancelHarvest();
                    currentTarget = null;
                    StartCoroutine(CancelCooldown());
                    return;
                }
            }
        }

        if (isCancelCooldown) return;

        checkTimer += Time.deltaTime;
        if (checkTimer >= 0.2f)
        {
            checkTimer = 0f;
            CheckForResources();
        }
    }

    private IEnumerator CancelCooldown()
    {
        isCancelCooldown = true;
        yield return new WaitForSeconds(0.3f);
        isCancelCooldown = false;
    }

    private void CheckForResources()
    {
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

        if (collectable is ResourceNode node)
        {
            node.SetInventory(inventory);
        }

        collectable.Collect();
        //Debug.Log("자원 수집 시작!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collectionRange);
    }
}
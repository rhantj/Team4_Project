using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask landmarkLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("UI References")]
    [SerializeField] private GameObject interactionPromptUI;

    private Inventory inventory;
    private Landmark nearbyLandmark = null;
    private Collider[] hitBuffer = new Collider[5];
    private float checkTimer = 0f;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        CheckNearbyLandmark();
        HandleInput();
    }

    private void CheckNearbyLandmark()
    {
        checkTimer += Time.deltaTime;

        if (checkTimer < 0.2f)
            return;

        checkTimer = 0f;

        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            interactionRange,
            hitBuffer,
            landmarkLayer
        );

        Landmark previousLandmark = nearbyLandmark;

        if (hitCount > 0)
        {
            nearbyLandmark = FindClosestLandmark(hitCount);
        }
        else
        {
            nearbyLandmark = null;
        }

        if (previousLandmark != nearbyLandmark)
        {
            UpdateInteractionUI();
        }
    }

    private Landmark FindClosestLandmark(int hitCount)
    {
        float closestDistance = float.MaxValue;
        Landmark closest = null;

        for (int i = 0; i < hitCount; i++)
        {
            Landmark landmark = hitBuffer[i].GetComponent<Landmark>();

            if (landmark != null && landmark.CanInteract())
            {
                float distance = Vector3.Distance(
                    transform.position,
                    hitBuffer[i].transform.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = landmark;
                }
            }
        }

        return closest;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(interactKey) && nearbyLandmark != null)
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (nearbyLandmark == null || inventory == null)
            return;

        //Inventory
        bool success = nearbyLandmark.TryUpgrade(inventory);

        if (success)
        {
            Debug.Log("건설 성공!");
        }
        else
        {
            Debug.Log("건설 실패!");
        }
    }

    private void UpdateInteractionUI()
    {
        if (interactionPromptUI != null)
        {
            bool shouldShow = nearbyLandmark != null;
            interactionPromptUI.SetActive(shouldShow);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
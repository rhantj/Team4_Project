using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Animator Settings")]
    [SerializeField] private Animator animator;

    private PlayerMovement playerMovement;

    private readonly int hashIsMoving = Animator.StringToHash("IsMoving");
    private readonly int hashCollect = Animator.StringToHash("Collect");

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (animator != null)
        {
            animator.SetBool(hashIsMoving, playerMovement.IsMoving);
        }
    }

    public void PlayCollectAnimation()
    {
        animator?.SetTrigger(hashCollect);
    }
}
using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour, ICollectable
{
    [Header("Resource Settings")]
    [SerializeField] private ResourceType resourceType = ResourceType.Wood;
    [SerializeField] private string resourceName = "나무";
    [SerializeField] private int woodAmount = 3;

    [Header("Collection Settings")]
    [SerializeField] private float collectionTime = 1f; // 채집 시간
    [SerializeField] private ParticleSystem collectEffect; // 채집 이펙트

    private bool isBeingCollected = false;
    private bool isCollected = false;

    public bool CanCollect()
    {
        return !isBeingCollected && !isCollected;
    }

    public void Collect()
    {
        if (!CanCollect()) return;

        StartCoroutine(CollectCoroutine());
    }

    private IEnumerator CollectCoroutine()
    {
        isBeingCollected = true;

        // 채집 이펙트 재생
        if (collectEffect != null)
            collectEffect.Play();

        Debug.Log($"{resourceName} 채집 중...");

        yield return new WaitForSeconds(collectionTime);

        Debug.Log($"{resourceName} x{woodAmount} 획득!");

        isCollected = true;

        // 나무 사라지는 애니메이션 (선택)
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        Destroy(gameObject);
    }

    public ResourceData GetResourceData()
    {
        return new ResourceData
        {
            resourceType = resourceType,
            resourceName = resourceName,
            amount = woodAmount
        };
    }

    // 시각적 피드백
    private void OnDrawGizmos()
    {
        Gizmos.color = isCollected ? Color.gray : Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
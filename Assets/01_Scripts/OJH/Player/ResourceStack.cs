using UnityEngine;
using System.Collections.Generic;

public class ResourceStack : MonoBehaviour
{
    [Header("Stack Settings")]
    [SerializeField] private Transform stackStartPoint;
    [SerializeField] private float stackSpacing = 0.35f; // 간격
    [SerializeField] private float stackHeight = 0.25f;  // 층 높이

    [Header("Visual")]
    [SerializeField] private Vector3 logScale = new Vector3(0.15f, 0.35f, 0.15f); // 나무 크기

    [Header("Prefabs")]
    [SerializeField] private GameObject woodLogPrefab;

    private List<GameObject> stackedWoods = new List<GameObject>();
    private int currentStackCount = 0;

    private void Awake()
    {
        if (stackStartPoint == null)
        {
            GameObject stackPoint = new GameObject("StackPoint");
            stackPoint.transform.SetParent(transform);
            stackPoint.transform.localPosition = new Vector3(0f, 1.2f, -0.6f); // 등 뒤 위
            stackPoint.transform.localRotation = Quaternion.identity;
            stackStartPoint = stackPoint.transform;
        }
    }

    public void AddWood(int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            StartCoroutine(SpawnWoodLog());
        }
    }

    private System.Collections.IEnumerator SpawnWoodLog()
    {
        if (woodLogPrefab == null)
        {
        
            yield break;
        }

        // 쌓을 위치 계산
        Vector3 stackPosition = CalculateStackPosition();

        // 완벽한 가로 회전 (Z축 90도)
        Quaternion stackRotation = Quaternion.Euler(0f, 0f, 90f);

        // 나무 생성
        GameObject wood = Instantiate(woodLogPrefab, stackPosition, stackRotation);
        wood.transform.SetParent(stackStartPoint);

        // 크기 고정
        wood.transform.localScale = logScale;

        // 떨어지는 애니메이션
        Vector3 startPos = stackPosition + Vector3.up * 2f;
        wood.transform.position = startPos;
        wood.transform.rotation = stackRotation; // 회전 고정

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = 1f - Mathf.Pow(1f - t, 3f); // EaseOut

            wood.transform.position = Vector3.Lerp(startPos, stackPosition, t);
            wood.transform.rotation = stackRotation; // 계속 회전 유지

            yield return null;
        }

        // 최종 위치/회전 확정
        wood.transform.position = stackPosition;
        wood.transform.rotation = stackRotation;
        wood.transform.localScale = logScale;

        stackedWoods.Add(wood);
        currentStackCount++;

    }

    private Vector3 CalculateStackPosition()
    {
        // 피라미드 계산
        int layer = 0;
        int totalItems = 0;

        // 현재 층 찾기
        while (totalItems + (layer + 1) <= currentStackCount)
        {
            totalItems += (layer + 1);
            layer++;
        }

        // 현재 층에서의 위치
        int itemsInCurrentLayer = currentStackCount - totalItems;
        int itemsPerLayer = layer + 1;

        // X 위치 (중앙 정렬)
        float x = (itemsInCurrentLayer - (itemsPerLayer - 1) * 0.5f) * stackSpacing;

        // Y 위치 (높이)
        float y = layer * stackHeight;

        // 로컬 좌표로 반환 (부모 기준)
        return stackStartPoint.TransformPoint(new Vector3(x, y, 0f));
    }

    public void RemoveWood(int amount = 1)
    {
        for (int i = 0; i < amount && stackedWoods.Count > 0; i++)
        {
            GameObject wood = stackedWoods[stackedWoods.Count - 1];
            stackedWoods.RemoveAt(stackedWoods.Count - 1);
            Destroy(wood);
            currentStackCount--;
        }
    }

    public int GetWoodCount()
    {
        return currentStackCount;
    }
}
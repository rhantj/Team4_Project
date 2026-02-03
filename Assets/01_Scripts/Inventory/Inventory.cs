using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    //용량
    [SerializeField] private int capacity = 10;
    //짊어질 아이템
    [SerializeField] private GameObject itemPrefab;
    //아이템 위치
    [SerializeField] private Transform itemSpawnPoint;
    //아이템 간격
    [SerializeField] private float stackInterval = 1f;

    private List<GameObject>stackItem=new List<GameObject>();



    private float movespeed = 5f;
    
    public void AddItem()
    {
        if(stackItem.Count>=capacity)
        {
            Debug.Log("인벤토리 꽉참");
            return;
        }

        //아이템생성위치관련
        Vector3 spawnPos = itemSpawnPoint.position + (itemSpawnPoint.up * stackInterval * stackItem.Count);
        GameObject newItem = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
        
        
        newItem.transform.SetParent(itemSpawnPoint);

        stackItem.Add(newItem);
    }
    public void RemoveItem()
    {
        if(stackItem.Count==0)
        {
            Debug.Log("인벤토리 비어있음");
            return;
        }
        GameObject removedItem = stackItem[stackItem.Count - 1];
        stackItem.RemoveAt(stackItem.Count - 1);
        Destroy(removedItem);
    }
    public void AddItem2()
    {
        if(stackItem.Count >= capacity)
        {
            Debug.Log("인벤토리 꽉참");
            return;
        }

        GameObject newItem = Instantiate(itemPrefab);
        newItem.transform.SetParent(itemSpawnPoint, false);

        float totalOffset = 0f;
        for(int i = 0; i < stackItem.Count; i++)
        {
            totalOffset+= ItemHeight(stackItem[i]);
        }

        newItem.transform.localPosition = new Vector3(0, totalOffset, 0);
        stackItem.Add(newItem);
    }
    private float ItemHeight(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if(renderer != null)
        {
            return renderer.bounds.size.y;
        }
        return stackInterval;

    }



    //테스트용 쌓기
    private void Update()
    {
        
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AddItem2();
            Debug.Log(stackItem.Count);
            Debug.Log(stackItem.Capacity);
            Debug.Log("스페이스누름");
        }

        move();

    }

    private void move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v).normalized;
        transform.Translate(dir * movespeed * Time.deltaTime);
    }
    





}

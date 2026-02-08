using System.Collections.Generic;
using UnityEngine;

public class ALTPlayer : MonoBehaviour
{
    public Queue<int> items = new();

    private void Awake()
    {
        items.Clear();
        for (int i = 0; i < 10; ++i)
        {
            items.Enqueue(i);
        }
    }
}
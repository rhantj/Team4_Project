using System.Collections.Generic;
using UnityEngine;

public class ALTPlayer : MonoBehaviour
{
    public Stack<int> items = new();

    private void Awake()
    {
        items.Clear();
        for (int i = 0; i < 10; ++i)
        {
            items.Push(i);
        }
    }
}
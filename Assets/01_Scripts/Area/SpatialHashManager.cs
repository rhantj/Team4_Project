using System.Collections.Generic;
using UnityEngine;

public class SpatialHashManager : MonoBehaviour
{
    public static SpatialHashManager Instance;

    [SerializeField] private float m_CellSize = 5f;

    private Dictionary<Vector2Int, List<ItemIOArea>> m_Grid = new();

    public float CellSize => m_CellSize;

    private void Awake()
    {
        Instance = this;
    }

    Vector2Int GetCell(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / m_CellSize);
        int z = Mathf.FloorToInt(pos.z / m_CellSize);

        return new Vector2Int(x, z);
    }

    public void SetArea(ItemIOArea area)
    {
        var cell = GetCell(area.transform.position);
        if (!m_Grid.TryGetValue(cell, out var list))
        {
            list = new List<ItemIOArea>();
            m_Grid[cell] = list;
        }

        list.Add(area);
    }

    public void RemoveArea(ItemIOArea area)
    {
        var cell = GetCell(area.transform.position);
        if (m_Grid.TryGetValue(cell, out var list))
            list.Remove(area);
    }

    public List<ItemIOArea> Query(Vector3 pos)
    {
        List<ItemIOArea> res = new();

        var cell = GetCell(pos);

        for (int x = -1; x <= 1; ++x)
        {
            for (int z = -1; z <= 1; ++z)
            {
                var checkCell = new Vector2Int(cell.x + x, cell.y + z);
                if (m_Grid.TryGetValue(checkCell, out var list))
                    res.AddRange(list);
            }
        }

        return res;
    }
}
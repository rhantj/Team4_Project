using System;
using UnityEngine;

[Serializable]
public struct RectZone
{
    public float minX, minZ, maxX, maxZ;
    public readonly bool IsInside(Vector3 pos)
    {
        return pos.x >= minX && pos.x <= maxX &&
               pos.z >= minZ && pos.z <= maxZ;
    }
}
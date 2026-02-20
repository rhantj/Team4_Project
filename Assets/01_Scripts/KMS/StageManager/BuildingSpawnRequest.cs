using System;
using UnityEngine;

[Serializable]
public class BuildingSpawnRequest
{
    public GameObject BuildingPF;
    public Transform SpawnPoint;
    public Vector3 LocalOffset;
    public bool UsePointRotation = true;
    public Vector3 EulerRotationOverride;
}
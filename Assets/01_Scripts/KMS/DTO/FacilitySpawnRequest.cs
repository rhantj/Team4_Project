using System;
using UnityEngine;

[Serializable]
public class FacilitySpawnRequest
{
    public EFacilityType Type;
    public Transform SpawnPoint;
    public Vector3 LocalOffset;
    public bool UsePointRotation = true;
    public Vector3 EulerRotationOverride;
}
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnRequest
{
    public EFacilityType Type;
    public Transform SpawnPoint;
    public Vector3 LocalOffset;
    public bool UseOverrideRotation;
    public Vector3 EulerRoatationOverride;
}

[System.Serializable]
public struct StageStep
{
    public SpawnRequest StageTheme;
    public SpawnRequest MainBuilding;
    public SpawnRequest[] Resources;
}

[CreateAssetMenu(fileName = "SOStageConfig", menuName = "Scriptable Objects/SOStageConfig")]
public class SOStageConfig : ScriptableObject
{
    public List<StageStep> Steps = new();
}
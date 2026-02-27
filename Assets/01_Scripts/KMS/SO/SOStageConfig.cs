using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
public struct SpawnRequest
{
    public AssetReferenceGameObject PrefabReference;
    public Transform SpawnPoint;
    public Vector3 LocalOffset;
    public bool UsePointRotation;
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
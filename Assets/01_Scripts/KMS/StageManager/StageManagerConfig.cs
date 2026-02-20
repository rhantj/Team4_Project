using System;
using UnityEngine;

[Serializable]
public class StageManagerConfig : ServiceConfig<StageManager>
{
    [field : SerializeField] public BuildingSpawnRequest[] BuildingSpawnRequests { get; private set; }
    [field : SerializeField] public BuildingSpawnRequest[] ResourceSpawnRequests { get; private set; }
}
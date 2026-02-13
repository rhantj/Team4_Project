using System;
using UnityEngine;

[Serializable]
public class FacilitySpawnerConfig : ServiceConfig<FacilitySpawner>
{
    [field : SerializeField] public Facility[] Facilities { get; private set; }
}
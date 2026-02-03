using UnityEngine;
public interface ISpawner
{
    void InstantiateFaility(EFacilityType type, Vector3 pos, Quaternion rot);
}
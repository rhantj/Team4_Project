using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BuildingStep
{
    public string StepName;
    public GameObject RequierItem;
    public int RequierAmount;
}

[CreateAssetMenu(fileName = "SOBuilding", menuName = "Scriptable Objects/SOBuilding")]
public class SOBuilding : ScriptableObject
{
    public List<BuildingStep> Steps = new();
}
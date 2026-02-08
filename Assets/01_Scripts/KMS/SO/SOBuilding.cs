using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOBuilding", menuName = "Scriptable Objects/SOBuilding")]
public class SOBuilding : ScriptableObject
{
    public int Step;
    public List<int> InputItems = new();
}
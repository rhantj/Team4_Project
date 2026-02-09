using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOAllMaterials", menuName = "Scriptable Objects/SOAllMaterials")]
public class SOAllMaterials : ScriptableObject
{
    public List<GameObject> AllMaterials = new();
}
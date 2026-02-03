using UnityEngine;

[CreateAssetMenu(fileName = "SOProductionFacility", menuName = "Scriptable Objects/SOProductionFacility")]
public class SOProductionFacility : ScriptableObject
{
    public int inputItem;
    public int outputItem;
    public EFacilityType facilityType;
}

using UnityEngine;


[CreateAssetMenu(fileName = "Resource Item", menuName = "Inventory/Resource Item")]
public class ResourceItemData : ScriptableObject
{
    public string itemName;
    public GameObject itemPrefab;

}

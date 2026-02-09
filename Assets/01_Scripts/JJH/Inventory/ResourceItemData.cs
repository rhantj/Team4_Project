using UnityEngine;


[CreateAssetMenu(fileName = "Resource Item", menuName = "Inventory/Resource Item")]
public class ResourceItemData : ScriptableObject
{
    public string m_SoItemName;
    public GameObject m_ItemPrefab;

}

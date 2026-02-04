public interface ICollectable
{
    bool CanCollect();
    void Collect();
    ResourceData GetResourceData();
}

public struct ResourceData
{
    public ResourceType resourceType;
    public string resourceName;
    public int amount;
}

public enum ResourceType
{
    Wood, Stone, Ore, Berry
}
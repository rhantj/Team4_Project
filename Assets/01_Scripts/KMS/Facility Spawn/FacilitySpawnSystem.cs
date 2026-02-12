public static class FacilitySpawnSystem
{
    public static ISpawner Spawner { get; private set; }
    public static void DI(ISpawner spawner)
    {
        Spawner = spawner;
    }
}
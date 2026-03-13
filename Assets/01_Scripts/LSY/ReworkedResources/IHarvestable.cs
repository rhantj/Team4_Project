using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Reworked
{
    public interface IHarvestable
    {
        ResourceType TargetResourceType { get; }

        bool IsHarvestable { get; }

        bool IsContacted(Vector2 positionOnXZ);
        Awaitable<List<ResourceItemData>> HarvestAsync(IHarvester harvester, float harvestSpeedMultiplier, int requestedAmount, CancellationToken token);
    }
}

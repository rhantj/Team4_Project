using System.Collections.Generic;
using UnityEngine;

namespace Reworked
{
    public interface IHarvester
    {
        List<ResourceType> TargetResourceTypes { get; }
        float HarvestSpeedMultiplier { get; }
        public bool IsHarvesting { get; }

        bool TryEnableUI(float progressNormalized);
        bool TryDisableUI();
    }
}

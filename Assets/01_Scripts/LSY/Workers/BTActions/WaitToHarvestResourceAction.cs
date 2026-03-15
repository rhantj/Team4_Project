using BehaviourTree;
using Reworked;
using UnityEngine;

[CreateAssetMenu(fileName = "WaitToHarvestResourceAction", menuName = "Scriptable Objects/Behaviour Tree/Action/Wait to Harvest Resource")]
public class WaitToHarvestResourceAction : BehaviourTreeActionStrategy
{
    private const string DestinationGameObjectKey = "DestinationGameObject";

    public override void Initialize(Blackboard blackboard)
    {
        //
    }

    public override INode.EState Tick(Blackboard blackboard)
    {
        if (!blackboard.TryGetValue("Inventory", out Inventory inventory)) return INode.EState.Failure;
        if (!blackboard.TryGetValue("Harvester", out IHarvester harvester)) return INode.EState.Failure;

        if (inventory.IsFull && harvester.IsHarvesting) return INode.EState.Success;
        return INode.EState.Failure;
    }
}

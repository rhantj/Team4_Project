using BehaviourTree;
using Reworked;
using UnityEngine;

[CreateAssetMenu(fileName = "WaitToUnloadResourceAction", menuName = "Scriptable Objects/Behaviour Tree/Action/Wait to Unload Resource")]
public class WaitToUnloadResourceAction : BehaviourTreeActionStrategy
{
    private const string DestinationGameObjectKey = "DestinationGameObject";

    public override void Initialize(Blackboard blackboard)
    {
        //
    }

    public override INode.EState Tick(Blackboard blackboard)
    {
        if (!blackboard.TryGetValue("Inventory", out Inventory inventory)) return INode.EState.Failure;
        //if (!blackboard.TryGetValue("Rigidbody", out Rigidbody rb)) return INode.EState.Failure;
        //if (!blackboard.TryGetValue("WorkerResourceUnloadBehaviour", out WorkerResourceUnloadBehaviour unloader)) return INode.EState.Failure;

        if (inventory.IsEmpty) return INode.EState.Success;
        return INode.EState.Failure;
    }
}

using UnityEngine;
using BehaviourTree;

[CreateAssetMenu(fileName = "CheckInventoryAction", menuName = "Scriptable Objects/Behaviour Tree/Action/Check Inventory")]
public class CheckInventoryAction : BehaviourTreeActionStrategy
{
    public override void Initialize(Blackboard blackboard)
    {
        //
    }

    public override INode.EState Tick(Blackboard blackboard)
    {
        if (!blackboard.TryGetValue("Inventory", out Inventory inventory)) return INode.EState.Failure;
        blackboard.Set("Inventory.IsFull", inventory.IsFull);
        blackboard.Set("Inventory.IsEmpty", inventory.IsEmpty);
        return INode.EState.Success;
    }
}

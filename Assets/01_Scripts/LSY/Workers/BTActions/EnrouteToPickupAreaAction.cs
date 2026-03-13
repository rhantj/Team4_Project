using UnityEngine;
using BehaviourTree;

[CreateAssetMenu(fileName = "EnrouteToPickupAreaAction", menuName = "Scriptable Objects/Behaviour Tree/Action/Enroute To Pickup Area")]
public class EnrouteToPickupAreaAction : BehaviourTreeActionStrategy
{
    private const string TargetPickupAreaKey = "TargetPickupArea";
    private const string DestinationGameObjectKey = "DestinationGameObject";

    public override void Initialize(Blackboard blackboard)
    {
        //
    }

    public override INode.EState Tick(Blackboard blackboard)
    {
        if (!blackboard.TryGetValue("Rigidbody", out Rigidbody rb)) return INode.EState.Failure;
        if (!blackboard.TryGetValue(TargetPickupAreaKey, out ItemIOArea targetItemIOArea)) return INode.EState.Failure;
        if (!blackboard.TryGetValue(DestinationGameObjectKey, out GameObject destinationGameObject)) return INode.EState.Failure;

        if (null != targetItemIOArea && null != destinationGameObject && targetItemIOArea.gameObject == destinationGameObject) return INode.EState.Success;
        else if (TryEnroute(blackboard, rb.position, targetItemIOArea)) return INode.EState.Success;
        else return INode.EState.Failure;
    }

    private bool TryEnroute(Blackboard blackboard, Vector3 position, ItemIOArea pickupArea)
    {
        GameObject destinationGameObject = null != pickupArea.gameObject ? pickupArea.gameObject : null;
        blackboard.Set(DestinationGameObjectKey, destinationGameObject);
        return null != destinationGameObject;
    }
}

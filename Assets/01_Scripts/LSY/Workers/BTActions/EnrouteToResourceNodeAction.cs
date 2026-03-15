using BehaviourTree;
using Reworked;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EnrouteToResourceNodeAction", menuName = "Scriptable Objects/Behaviour Tree/Action/Enroute To Resource Node")]
public class EnrouteToResourceNodeAction : BehaviourTreeActionStrategy
{
    private const string DestinationGameObjectKey = "DestinationGameObject";
    private const string ResourceTypeListKey = "ResourceTypes";
    private const string ResourceTagKey = "Resource Node";

    public override void Initialize(Blackboard blackboard)
    {
        //
    }

    public override INode.EState Tick(Blackboard blackboard)
    {
        if (!blackboard.TryGetValue("Rigidbody", out Rigidbody rb)) return INode.EState.Failure;
        blackboard.TryGetValue(DestinationGameObjectKey, out GameObject destinationGameObject);

        if (null != destinationGameObject && IsHarvestable(destinationGameObject)) return INode.EState.Success;
        else if (TryEnroute(blackboard, rb.position)) return INode.EState.Success;
        else return INode.EState.Failure;
    }

    private bool IsHarvestable(GameObject gameObject)
    {
        return gameObject.TryGetComponent(out IHarvestable harvestable) && harvestable.IsHarvestable;
    }

    public bool TryEnroute(Blackboard blackboard, Vector3 position)
    {
        if (!blackboard.TryGetValue(ResourceTypeListKey, out List<ResourceType> resourceTypes)) return false;

        Vector2 workerPositionOnXZ = new Vector2(position.x, position.z);

        GameObject destinationGameObject = FindNearestAvailableResourceObject(workerPositionOnXZ, resourceTypes);
        blackboard.Set(DestinationGameObjectKey, destinationGameObject);

        return null == destinationGameObject;
    }

    private GameObject FindNearestAvailableResourceObject(Vector2 position, List<ResourceType> resourceTypes)
    {
        IEnumerable<GameObject> resources = GameManager.Instance.GetService<GameObjectTaggedGroupCacheService>().GetTaggedGroupCache(ResourceTagKey);
        if (null == resources) return null;
        else return resources
            .Where(r => r.activeInHierarchy)
            .Where(r => r.TryGetComponent(out IHarvestable h) && h.IsHarvestable && resourceTypes.Contains(h.TargetResourceType))
            .Aggregate<GameObject, ValueTuple<GameObject, float>, GameObject>(
            (null, float.MaxValue),
            (nearstPair, comparand) =>
            {
                Vector2 positionOnXZ = new Vector2(comparand.transform.position.x, comparand.transform.position.z);
                float comparandDistance = Vector2.Distance(position, positionOnXZ);
                return (comparandDistance < nearstPair.Item2) ? (comparand, comparandDistance) : nearstPair;
            },
            pair => pair.Item1);
    }
}

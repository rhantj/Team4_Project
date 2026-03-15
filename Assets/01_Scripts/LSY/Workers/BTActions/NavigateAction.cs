using BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NavigateAction", menuName = "Scriptable Objects/Behaviour Tree/Action/Navigate")]
public class NavigateAction : BehaviourTreeActionStrategy
{
    private const string RigidbodyKey = "Rigidbody";
    private const string NavMeshAgentKey = "NavMeshAgent";
    private const string NavMeshObstacleKey = "NavMeshObstacle";
    private const string DestinationGameObjectKey = "DestinationGameObject";
    private const string PrevDestinationGameObjectKey = "PreviousDestinationGameObject";

    public override void Initialize(Blackboard blackboard)
    {
        //
    }

    public override INode.EState Tick(Blackboard blackboard)
    {
        if (!blackboard.TryGetValue(RigidbodyKey, out Rigidbody rb)) return INode.EState.Failure;
        if (!blackboard.TryGetValue(NavMeshAgentKey, out NavMeshAgent agent)) return INode.EState.Failure;
        //if (!blackboard.TryGetValue(NavMeshObstacleKey, out NavMeshObstacle obstacle)) return INode.EState.Failure;
        if (!blackboard.TryGetValue(DestinationGameObjectKey, out GameObject destinationGameObject) || null == destinationGameObject) return INode.EState.Failure;
        if (!blackboard.TryGetValue(PrevDestinationGameObjectKey, out GameObject previousDestinationGameObject)) previousDestinationGameObject = null;

        if (destinationGameObject == previousDestinationGameObject) return INode.EState.Success;
        if (Application.isEditor) Debug.Log($"Worker Navigate to \"{destinationGameObject.name}\".");
        Navigate(agent, destinationGameObject);
        blackboard.Set(PrevDestinationGameObjectKey, destinationGameObject);
        return INode.EState.Success;
    }

    private void Navigate(NavMeshAgent agent, GameObject destinationGameObject)
    {
        Vector3 targetPositionOnXZ = new Vector3(destinationGameObject.transform.position.x, 0f, destinationGameObject.transform.position.z);

        float obstacleRadius =
            (destinationGameObject.TryGetComponent(out NavMeshObstacle obstacle) && null != obstacle)
            ? obstacle.shape switch
            {
                NavMeshObstacleShape.Capsule => obstacle.radius,
                NavMeshObstacleShape.Box => obstacle.size.magnitude * 0.5f,
                _ => 0.0f
            }
            : 0.0f;

        if (!NavMesh.SamplePosition(targetPositionOnXZ, out NavMeshHit preliminaryAnchor, obstacleRadius + 5.0f, NavMesh.AllAreas))
        {
            if (Application.isEditor) Debug.LogError("Cannot find an appopriate NavMesh destination point.");
            return;
        }

        Vector3 finalDestination = preliminaryAnchor.position;

        NavMeshPath path = new NavMeshPath();
        if (!NavMesh.CalculatePath(agent.transform.position, preliminaryAnchor.position, agent.areaMask, path) || 2 > path.corners.Length)
        {
            if (Application.isEditor) Debug.LogError("Cannot find a path to preliminary NavMesh destination point.");
            return;
        }

        int lowerBoundToQuery = 0;
        int upperBoundToQuery = path.corners.Length - 1;
        int firstCornerIndexOnLineOfSight = upperBoundToQuery;
        while (lowerBoundToQuery <= upperBoundToQuery)
        {
            int mid = (lowerBoundToQuery + upperBoundToQuery) / 2;
            if (NavMesh.Raycast(path.corners[mid], targetPositionOnXZ, out NavMeshHit testingHit, NavMesh.AllAreas)
                && Vector3.Distance(testingHit.position, targetPositionOnXZ) <= obstacleRadius + 0.5f)
            {
                firstCornerIndexOnLineOfSight = mid;
                upperBoundToQuery = mid - 1;
            }
            else
            {
                lowerBoundToQuery = mid + 1;
            }
        }

        if (NavMesh.Raycast(path.corners[firstCornerIndexOnLineOfSight], targetPositionOnXZ, out NavMeshHit hit, NavMesh.AllAreas))
        {
            Vector3 pushDir = (path.corners[firstCornerIndexOnLineOfSight] - hit.position).normalized;
            finalDestination = hit.position + (pushDir * (agent.radius + 0.1f));
        }

        NavMeshPath finalPath = new NavMeshPath();
        if (agent.CalculatePath(finalDestination, finalPath)) agent.SetPath(finalPath);
    }
}

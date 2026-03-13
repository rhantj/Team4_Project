using Reworked;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BehaviourTreeRunner))]
[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ResourceHarvestBehaviour))]
[RequireComponent(typeof(WorkerResourceUnloadBehaviour))]
public class WorkerBehaviourTreeConfigurator : MonoBehaviour
{
    [ReadOnly(true)][SerializeField] private ItemIOArea m_UnloadArea;

    BehaviourTreeRunner m_BehaviourTreeRunner;

    private void Awake()
    {
        if (!TryGetComponent(out m_BehaviourTreeRunner) && Application.isEditor) Debug.LogError("Cannot find BehaviourTreeRunner");
    }

    private void Start()
    {
        if (TryGetComponent(out ResourceHarvestBehaviour resourceHarvestBehaviour)) m_BehaviourTreeRunner.SetBlackboard("ResourceTypes", resourceHarvestBehaviour.TargetResourceTypes);
        else if (Application.isEditor) Debug.LogError("Cannot find the ResourceHarvestBehaviour component.");

        if (TryGetComponent(out Inventory inventory)) m_BehaviourTreeRunner.SetBlackboard("Inventory", inventory);
        else if (Application.isEditor) Debug.LogError("Cannot find the Inventory component.");

        if (TryGetComponent(out Rigidbody rb)) m_BehaviourTreeRunner.SetBlackboard("Rigidbody", rb);
        else if (Application.isEditor) Debug.LogError("Cannot find the Rigidbody component.");

        if (TryGetComponent(out IHarvester harvester)) m_BehaviourTreeRunner.SetBlackboard("Harvester", harvester);
        else if (Application.isEditor) Debug.LogError("Cannot find the IHarvester component.");

        if (TryGetComponent(out WorkerResourceUnloadBehaviour unloader)) m_BehaviourTreeRunner.SetBlackboard("WorkerResourceUnloadBehaviour", unloader);
        else if (Application.isEditor) Debug.LogError("Cannot find the WorkerResourceUnloadBehaviour component.");

        if (TryGetComponent(out NavMeshAgent agent)) m_BehaviourTreeRunner.SetBlackboard("NavMeshAgent", agent);
        else if (Application.isEditor) Debug.LogError("Cannot find the NavMeshAgent component.");

        if (TryGetComponent(out NavMeshObstacle obstacle)) m_BehaviourTreeRunner.SetBlackboard("NavMeshObstacle", obstacle);
        else if (Application.isEditor) Debug.LogError("Cannot find the NavMeshObstacle component.");

        m_BehaviourTreeRunner.SetBlackboard("TargetPickupArea", m_UnloadArea);
    }
}

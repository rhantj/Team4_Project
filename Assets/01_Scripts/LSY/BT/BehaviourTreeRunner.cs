using UnityEngine;
using BehaviourTree;

public class BehaviourTreeRunner : MonoBehaviour
{
    [ReadOnly][SerializeReference] private Blackboard m_Blackboard;
    [ReadOnly(true)][SerializeField] private BehaviourTreeSO m_BehaviourTree;

    private void OnEnable()
    {
        m_Blackboard = new Blackboard();
        m_Blackboard.Set("GameObject", gameObject);
        m_BehaviourTree?.m_Root?.Initialize(m_Blackboard);
    }

    private void FixedUpdate()
    {
        m_BehaviourTree?.m_Root?.Tick(m_Blackboard);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!(m_BehaviourTree?.m_Root?.Validate() ?? false)) Debug.LogError("Behaviour Tree has not been validated.");
    }
#endif

    public void SetBlackboard<T>(string key, T value) => m_Blackboard.Set(key, value);
}

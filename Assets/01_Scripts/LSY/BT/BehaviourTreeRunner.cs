using UnityEngine;
using BehaviourTree;

public class BehaviourTreeRunner : MonoBehaviour
{
    [ReadOnly][SerializeReference] private Blackboard m_Blackboard;
    [ReadOnly(true)][SerializeField] private BehaviourTreeSO m_BehaviourTree;

    private void OnEnable()
    {
        m_Blackboard = new Blackboard();
        m_Blackboard.Set("GameObject", this);
        m_BehaviourTree?.m_Root?.Initialize(m_Blackboard);
    }

    private void FixedUpdate()
    {
        m_BehaviourTree?.m_Root?.Tick(m_Blackboard);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (m_BehaviourTree?.m_Root?.Validate() ?? false) Debug.LogWarning("Behaviour Tree has not been validated.");
    }
#endif
}

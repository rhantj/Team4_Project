using UnityEngine;

namespace BehaviourTree
{
    [CreateAssetMenu(fileName = "BehaviourTreeSO", menuName = "Scriptable Objects/Behaviour Tree/Behaviour Tree Object")]
    public class BehaviourTreeSO : ScriptableObject
    {
        [ReadOnly(true)][SerializeReference] public Root m_Root;
    }
}

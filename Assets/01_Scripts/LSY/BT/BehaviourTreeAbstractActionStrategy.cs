using UnityEngine;

namespace BehaviourTree
{
    public abstract class BehaviourTreeActionStrategy : ScriptableObject
    {
        public abstract void Initialize(Blackboard blackboard);
        public abstract INode.EState Tick(Blackboard blackboard);
    }
}

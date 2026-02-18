using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public sealed class Leaf : INode
    {
        [field: ReadOnly(true)][field: SerializeField] public string Name { get; private set; }

        [ReadOnly(true)][SerializeField] private BehaviourTreeActionStrategy m_ActionObject;

        public bool Validate() => null != m_ActionObject;

        public void Initialize(Blackboard blackboard) => m_ActionObject.Initialize(blackboard);

        public INode.EState Tick(Blackboard blackboard) => m_ActionObject.Tick(blackboard);
    }
}

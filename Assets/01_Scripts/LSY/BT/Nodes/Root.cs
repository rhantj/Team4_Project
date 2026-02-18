using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public sealed class Root : INode
    {
        [field: ReadOnly(true)][field: SerializeField] public string Name { get; private set; }

        [ReadOnly(true)][SerializeReference] private INode m_Node;

        public bool Validate() => m_Node?.Validate() ?? false;
        public void Initialize(Blackboard blackboard) => m_Node.Initialize(blackboard);
        public INode.EState Tick(Blackboard blackboard) => m_Node?.Tick(blackboard) ?? INode.EState.Failure;

    }
}

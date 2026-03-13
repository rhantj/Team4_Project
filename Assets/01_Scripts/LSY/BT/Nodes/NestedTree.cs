using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public sealed class NestedTree : INode
    {
        public string Name => m_NestedBehaviourTree.name;

        [SerializeField] private BehaviourTreeSO m_NestedBehaviourTree;

        public bool Validate() => m_NestedBehaviourTree?.m_Root?.Validate() ?? false;
        public void Initialize(Blackboard blackboard) => m_NestedBehaviourTree.m_Root.Initialize(blackboard);
        public INode.EState Tick(Blackboard blackboard) => m_NestedBehaviourTree.m_Root.Tick(blackboard);
    }
}

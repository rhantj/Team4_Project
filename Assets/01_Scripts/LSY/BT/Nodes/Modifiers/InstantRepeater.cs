using System;
using UnityEngine;

namespace BehaviourTree
{
    [Tooltip("Blocking repeater which returns success when every iteration has succeded. Returns failure instantly if any iteration failed or is in running state.")]
    [Serializable]
    public sealed class InstantRepeater : Modifier
    {
        [ReadOnly(true)][Min(1)][SerializeField] private int m_InitialCount;
        [ReadOnly(true)][SerializeField] private string m_NextCountKey;
        [Tooltip("Optional node to evaludate status.")][ReadOnly(true)][SerializeReference] private INode m_EvaluationNode;
        [field: ReadOnly(true)][field: SerializeReference] protected override INode Node { get; set; }

        public override bool Validate() => (m_EvaluationNode?.Validate() ?? true) && base.Validate();

        public override void Initialize(Blackboard blackboard)
        {
            blackboard.Set(m_NextCountKey, m_InitialCount);
            m_EvaluationNode?.Initialize(blackboard);
            base.Initialize(blackboard);
        }

        public override INode.EState Tick(Blackboard blackboard)
        {
            if (INode.EState.Success != (m_EvaluationNode?.Tick(blackboard) ?? INode.EState.Success)) return INode.EState.Failure;
            if (!blackboard.TryGetValue(m_NextCountKey, out int count) || count < 0) return INode.EState.Failure;

            for (int i = 0; i < count; i++) if (INode.EState.Success != Node.Tick(blackboard)) return INode.EState.Failure;
            return INode.EState.Success;
        }
    }
}

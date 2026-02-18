using System;
using UnityEngine;

namespace BehaviourTree
{
    [Tooltip("Non-blocking repeater which returns success when every iteration has succeded. Returns failure instantly if any iteration failed.")]
    [Serializable]
    public sealed class Repeater : Modifier
    {
        [ReadOnly(true)][Min(1)][SerializeField] private int m_InitialCount;
        [ReadOnly(true)][SerializeField] private string m_NextCountKey;
        [Tooltip("Optional node to evaludate status.")][ReadOnly(true)][SerializeReference] private INode m_EvaluationNode;
        [field: ReadOnly(true)][field: SerializeReference] protected override INode Node { get; set; }

        private int m_IterationLeft;

        public override bool Validate() => (m_EvaluationNode?.Validate() ?? true) && base.Validate();

        public override void Initialize(Blackboard blackboard)
        {
            blackboard.Set(m_NextCountKey, m_InitialCount);
            m_IterationLeft = 0;

            m_EvaluationNode?.Initialize(blackboard);
            base.Initialize(blackboard);
        }

        public override INode.EState Tick(Blackboard blackboard)
        {
            if (INode.EState.Success != (m_EvaluationNode?.Tick(blackboard) ?? INode.EState.Success)) return INode.EState.Failure;
            if (0 == m_IterationLeft && !blackboard.TryGetValue(m_NextCountKey, out m_IterationLeft) || m_IterationLeft < 0) return INode.EState.Failure;
            if (0 == m_IterationLeft) return INode.EState.Success;

            switch (Node.Tick(blackboard))
            {
                case INode.EState.Success:
                    m_IterationLeft--;
                    return INode.EState.Success;
                case INode.EState.Failure:
                    m_IterationLeft = 0;
                    return INode.EState.Failure;
                case INode.EState.Running:
                    return INode.EState.Running;
                default:
                    throw new InvalidOperationException("Invalid node state.");
            }
        }
    }
}

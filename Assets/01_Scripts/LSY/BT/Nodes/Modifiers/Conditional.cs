using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public sealed class Conditional : Modifier
    {
        [ReadOnly(true)][SerializeField] private string m_ConditionalKey;
        [ReadOnly(true)][SerializeField] private INode.EState m_ResultOnFailCondition;
        [Tooltip("Optional node to evaludate status.")][ReadOnly(true)][SerializeReference] private INode m_EvaluationNode;
        [field: ReadOnly(true)][field: SerializeReference] protected override INode Node { get; set; }

        public override bool Validate() => (m_EvaluationNode?.Validate() ?? true) && base.Validate();

        public override void Initialize(Blackboard blackboard)
        {
            m_EvaluationNode?.Initialize(blackboard);
            base.Initialize(blackboard);
        }

        public override INode.EState Tick(Blackboard blackboard)
        {
            if (INode.EState.Success != (m_EvaluationNode?.Tick(blackboard) ?? INode.EState.Success)) return INode.EState.Failure;
            if (!blackboard.TryGetValue(m_ConditionalKey, out bool isConditionTrue)) return INode.EState.Failure;
            else return isConditionTrue ? Node.Tick(blackboard) : m_ResultOnFailCondition;
        }
    }
}

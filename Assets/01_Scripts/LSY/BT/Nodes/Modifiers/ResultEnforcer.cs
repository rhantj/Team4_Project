using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public sealed class ResultEnforcer : Modifier
    {
        [ReadOnly(true)][SerializeField] private INode.EState m_SuccessTarget;
        [ReadOnly(true)][SerializeField] private INode.EState m_FailureTarget;
        [ReadOnly(true)][SerializeField] private INode.EState m_RunningTarget;
        [field: ReadOnly(true)][field: SerializeReference] protected override INode Node { get; set; }

        public override INode.EState Tick(Blackboard blackboard)
        {
            return Node.Tick(blackboard) switch
            {
                INode.EState.Success => m_SuccessTarget,
                INode.EState.Failure => m_FailureTarget,
                INode.EState.Running => m_RunningTarget,
                _                    => throw new NotImplementedException()
            };
        }
    }
}

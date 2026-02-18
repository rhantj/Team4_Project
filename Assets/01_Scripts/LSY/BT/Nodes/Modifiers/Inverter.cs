using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public sealed class Inverter : Modifier
    {
        [field: ReadOnly(true)][field: SerializeReference] protected override INode Node { get; set; }

        public override INode.EState Tick(Blackboard blackboard)
        {
            return Node.Tick(blackboard) switch
            {
                INode.EState.Success => INode.EState.Failure,
                INode.EState.Running => INode.EState.Running,
                INode.EState.Failure => INode.EState.Success,
                _                    => throw new NotImplementedException()
            };
        }
    }
}

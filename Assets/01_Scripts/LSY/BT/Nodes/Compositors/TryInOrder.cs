using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public sealed class TryInOrder : Compositor
    {
        [field: ReadOnly(true)][field: SerializeReference] protected override List<INode> Nodes { get; set; }

        public override INode.EState Tick(Blackboard blackboard)
        {
            if (null == Nodes) return INode.EState.Failure;

            foreach (INode node in Nodes)
            {
                INode.EState state = node.Tick(blackboard);
                if (INode.EState.Failure != state) return state;
            }

            return INode.EState.Failure;
        }
    }
}

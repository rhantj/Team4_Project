using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public sealed class Random : Compositor
    {
        [field: ReadOnly(true)][field: SerializeReference] protected override List<INode> Nodes { get; set; }

        private Unity.Mathematics.Random random;

        public override void Initialize(Blackboard blackboard)
        {
            random = new Unity.Mathematics.Random((uint)DateTime.Now.Ticks);
            base.Initialize(blackboard);
        }

        public override INode.EState Tick(Blackboard blackboard)
        {
            int index = random.NextInt(0, Nodes.Count - 1);
            return Nodes[index].Tick(blackboard);
        }
    }
}

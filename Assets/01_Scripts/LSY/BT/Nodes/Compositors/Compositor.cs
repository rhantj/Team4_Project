using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class Compositor : INode
    {
        [field: ReadOnly(true)][field: SerializeField] public string Name { get; protected set; }

        protected abstract List<INode> Nodes { get; set; }

        public bool Validate() => Nodes?.TrueForAll(node => node?.Validate() ?? false) ?? false;
        public virtual void Initialize(Blackboard blackboard) => Nodes.ForEach(node => node.Initialize(blackboard));
        public abstract INode.EState Tick(Blackboard blackboard);
    }
}

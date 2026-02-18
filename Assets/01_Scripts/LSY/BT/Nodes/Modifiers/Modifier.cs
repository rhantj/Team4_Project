using System;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class Modifier : INode
    {
        [field: ReadOnly(true)][field: SerializeField] public string Name { get; protected set; }

        protected abstract INode Node { get; set; }

        public virtual bool Validate() => Node?.Validate() ?? false;
        public virtual void Initialize(Blackboard blackboard) => Node.Initialize(blackboard);
        public abstract INode.EState Tick(Blackboard blackboard);
    }
}

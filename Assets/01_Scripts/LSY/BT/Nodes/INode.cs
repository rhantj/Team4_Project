using System;
using UnityEngine;

namespace BehaviourTree
{
    public interface INode
    {
        public enum EState
        {
            Success,
            Failure,
            Running
        }

        string Name { get; }

        bool Validate();
        void Initialize(Blackboard blackboard);
        public EState Tick(Blackboard blackboard);
    }
}

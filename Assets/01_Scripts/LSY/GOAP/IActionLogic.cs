using UnityEngine;

namespace GOAP
{
    public interface IActionLogic
    {
        bool IsComplete { get; }
        bool IsPerformable { get; }
        float Cost { get; }

        void Initialize(Agent agent, Blackboard blackboard)
        {
            // Default No Operation
        }

        void Start()
        {
            // Default No Operation
        }

        void FixedUpdate()
        {
            // Default No Operation
        }

        void Stop()
        {
            // Default No Operation
        }
    }
}

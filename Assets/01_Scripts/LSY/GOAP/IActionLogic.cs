using UnityEngine;

namespace GOAP
{
    public interface IActionLogic
    {
        float EstimateCost(Agent agent, Blackboard blackboard);
        bool IsPreconditionMet(Agent agent, Blackboard blackboard);
        void Execute(Agent agent, Blackboard blackboard);
    }
}

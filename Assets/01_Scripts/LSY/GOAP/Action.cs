using System;
using UnityEngine;

namespace GOAP
{
    [Serializable]
    public class Action : ISerializationCallbackReceiver
    {
        [field: SerializeField] public string ActionName { get; private set; }
        [field: Tooltip("Temporary Logic Interface and need to be replaced with scripting system compatible with addressable system.")]
        [ReadOnly(true)][field: SerializeReference] IActionLogic logic;

        public float EstimateCost(Agent agent, Blackboard blackboard)
        {
            bool isPreconditionMet = logic?.IsPreconditionMet(agent, blackboard) ?? false;
            return isPreconditionMet ? logic.EstimateCost(agent, blackboard) : float.NaN;
        }

        public void Execute(Agent agent, Blackboard blackboard)
        {
            bool isPreconditionMet = logic?.IsPreconditionMet(agent, blackboard) ?? false;
            if (!isPreconditionMet) throw new NotImplementedException(); // TODO: call callback to re-estimate
            logic.Execute(agent, blackboard);
        }

        public void OnBeforeSerialize()
        {
            //
        }

        public void OnAfterDeserialize()
        {
            //
        }
    }
}

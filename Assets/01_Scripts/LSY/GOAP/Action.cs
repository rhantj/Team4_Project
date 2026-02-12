using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP
{
    [CreateAssetMenu(fileName = "GOAP-Action", menuName = "Scriptable Objects/GOAP/Action")]
    public class Action : ScriptableObject, ISerializationCallbackReceiver
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public List<ICondition> Preconditions { get; private set; }
        [field: SerializeField] public List<ICondition> Effects { get; private set; }
        [field: Tooltip("Temporary Logic Interface, need to be replaced with scripting system compatible with addressable system.")]
        [ReadOnly(true)][field: SerializeReference] IActionLogic logic;

        public bool IsComplete => logic.IsComplete;
        public float Cost => logic.Cost;

        public void Initialize(Agent agent, Blackboard blackboard)
        {
            bool isPreconditionMet = Preconditions?.All(c => c.IsConditionMet(blackboard)) ?? false;
            if (!isPreconditionMet) throw new NotImplementedException(); // TODO: call callback to re-estimate
        }

        public void Start() => logic.Start();

        public void FixedUpdate()
        {

            if (logic.IsPerformable) logic.FixedUpdate();
            // TODO
        }

        public void Stop() => logic.Stop();

        public void OnBeforeSerialize()
        {
            // TODO: process scripting
        }

        public void OnAfterDeserialize()
        {
            // TODO: process scripting
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (null == logic) Debug.LogError("Action does not have logic.");
        }
#endif
    }
}

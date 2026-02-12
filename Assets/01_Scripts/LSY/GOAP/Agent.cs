using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    [RequireComponent(typeof(Rigidbody))]
    public class Agent : MonoBehaviour
    {
        [ReadOnly(true)][SerializeField] private List<Action> m_possibleActions;
        [ReadOnly(true)][SerializeField] private List<Goal> m_Goals;
        private Blackboard m_Blackboard;

        private void Awake()
        {
        }

        private void OnEnable()
        {
            m_Blackboard = new Blackboard();
        }

        private void OnDisable()
        {
            m_Blackboard = null;
        }

        private void FixedUpdate()
        {
            // do update fsm and goap
        }


        /*
            AI controller - which to choose?:
                - goap: main AI
                - fsm: simple action and state itself - use animator
            runtime data store:
                - [Done] blackboard
                - flyweight - do it later, this is not urgent
                - state/strategy as SO
            action logic:
                - need to be saved as serialized data, not compiled into binary directly...
                - use Mono Roslyn to evaluate C# code at runtime?
                - or use lua?
                - no need to start with complete structure. just start with AOT code.
        */
    }
}

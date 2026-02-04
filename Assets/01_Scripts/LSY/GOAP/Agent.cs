using UnityEngine;

namespace GOAP
{
    [RequireComponent(typeof(Rigidbody))]
    public class Agent : MonoBehaviour
    {
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
                - flyweight
                - state/strategy as SO
            action logic:
                - need to be saved as serialized data, not compiled into binary directly...
                - use MONO to evaluate C# code at runtime?
                - no need to start with complete structure. just start with AOT code.
        */
    }
}

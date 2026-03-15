using System;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public sealed class Cooldown : Modifier
    {
        [ReadOnly(true)][SerializeField] private float m_DefaultCooldown;
        [SerializeField] private INode.EState m_StateOnWait;

        private string m_LastInvokedTimeKey;
        private string m_CurrentCooldownKey;
        private string m_NextCooldownKey;

        [field: ReadOnly(true)][field: SerializeReference] protected override INode Node { get; set; }

        public override void Initialize(Blackboard blackboard)
        {
            m_LastInvokedTimeKey = Name + ".LastInvokedTime";
            m_CurrentCooldownKey = Name + ".CurrentCooldown";
            m_NextCooldownKey = Name + ".NextCooldown";

            blackboard.Set(m_LastInvokedTimeKey, float.MinValue);   // make it invoke on first run
            blackboard.Set(m_CurrentCooldownKey, m_DefaultCooldown);
            blackboard.Set(m_NextCooldownKey, m_DefaultCooldown);
            base.Initialize(blackboard);
        }

        public override INode.EState Tick(Blackboard blackboard)
        {
            if (!blackboard.TryGetValue(m_LastInvokedTimeKey, out float lastInvokedTime)) return INode.EState.Failure;
            if (!blackboard.TryGetValue(m_CurrentCooldownKey, out float cooldown)) return INode.EState.Failure;

            if (Time.time < lastInvokedTime + cooldown) return m_StateOnWait;

            if (!blackboard.TryGetValue(m_NextCooldownKey, out cooldown)) return INode.EState.Failure;
            blackboard.Set(m_LastInvokedTimeKey, Time.time);
            blackboard.Set(m_CurrentCooldownKey, cooldown);
            return Node.Tick(blackboard);
        }
    }
}

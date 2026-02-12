using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    [CreateAssetMenu(fileName = "GOAP-Goal", menuName = "Scriptable Objects/GOAP/Goal")]
    public class Goal : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: Tooltip("Tie-breaking priority")][field: SerializeField] public float Priority { get; private set; }
        [field: SerializeField] public List<ICondition> DesiredEffects { get; private set; }
    }
}

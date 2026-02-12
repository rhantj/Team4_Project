using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    [CreateAssetMenu(fileName = "AStarPlanner", menuName = "Scriptable Objects/GOAP/Planners/A* Planner")]
    public class AStarPlanner : ScriptableObject, IPlanner
    {
        public Queue<Action> Plan(Goal goal, List<Action> availableActions, Blackboard blackboard)
        {
            // TODO
            return null;
        }
    }
}

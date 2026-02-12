using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public interface IPlanner
    {
        /// <summary>
        /// Plan actions for the goal.
        /// </summary>
        /// <param name="goal">The goal being targeted.</param>
        /// <param name="availableActions">A list of actions available to execute.</param>
        /// <param name="blackboard">A local blackboard of an agent executing the plan.</param>
        /// <returns>
        /// A queue of actions required to execute sequentially to achieve the goal.
        /// If the goal is not achievable, this would be null.
        /// If the goal is already achieved, this would be empty.
        /// </returns>
        Queue<Action> Plan(Goal goal, List<Action> availableActions, Blackboard blackboard);
    }
}

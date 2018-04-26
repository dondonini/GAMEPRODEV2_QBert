using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GB_ChaseState : MonoBehaviour {

    private readonly GreenBlob_SM enemy;

    private MapManager m_mapManager;

    private Waypoint m_goalWaypoint;

	public GB_ChaseState(GreenBlob_SM _statePattern)
    {
        enemy = _statePattern;
        m_mapManager = MapManager.Instance;

        // Get current waypoint if not existing already
        if (enemy.m_currentWaypoint == null)
        {
            enemy.m_currentWaypoint = m_mapManager.FindClosestWaypoint(enemy.transform.position);
        }
    }

    public void UpdateState()
    {
        if (enemy.m_chaseTarget == null)
        {
            Debug.LogError("ERROR: enemy.m_chaseTarget does not exist!");
            return;
        }

        if (m_goalWaypoint != null)
        {

        }
        else
        {
            m_goalWaypoint = GetWaypointInDirectionOfTarget(enemy.m_chaseTarget.transform.position);
        }
    }

    #region State Transitions

    public void ToEnterState()
    {
        
    }

    public void ToChaseState()
    {
        // Can't transition to same state
    }

    public void ToExitState()
    {

    }

    #endregion

    #region State Methods

    // Find closest waypoint towards the target
    private Waypoint GetWaypointInDirectionOfTarget(Vector3 t)
    {
        float distance = 0;
        Waypoint closestWaypoint = null;

        // Check all neighbors in waypoint to see which one is closest to target
        // TODO: This code will always go for the closest waypoint. So if the enemy is in a dead end, it will never get out!
        for (int w = 0; w < enemy.m_currentWaypoint.m_neighbors.Count; w++)
        {
            Waypoint selectedWaypoint = enemy.m_currentWaypoint.m_neighbors[w];

            float distanceToTarget = Vector3.Distance(selectedWaypoint.position, t);

            if (distanceToTarget < distance)
            {
                closestWaypoint = selectedWaypoint;
                distance = distanceToTarget;
            }
        }

        return closestWaypoint;
    }

    #endregion
}

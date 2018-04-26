using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GB_ChaseState : MonoBehaviour {

    private readonly GreenBlob_SM enemy;

    private MapManager m_mapManager;

	public GB_ChaseState(GreenBlob_SM _statePattern)
    {
        enemy = _statePattern;
        m_mapManager = MapManager.Instance;

        // Get current waypoint
        if (enemy.m_currentWaypoint == null)
        {
            enemy.m_currentWaypoint = m_mapManager.FindClosestWaypoint(enemy.transform.position);
        }
    }

    public void UpdateState()
    {

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

    
    private Waypoint GetClosestWaypointToSelf

    #endregion
}

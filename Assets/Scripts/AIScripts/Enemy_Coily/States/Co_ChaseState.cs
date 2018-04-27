using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Co_ChaseState : IEnemyStates_SM {

    private readonly Coily_SM enemy;

	public Co_ChaseState(Coily_SM _statePattern)
    {
        enemy = _statePattern;
    }

    public void UpdateState()
    {
        if (!FailSafe()) return;

        if (enemy.m_waitTime <= enemy.m_chaseFreq && enemy.m_goalWaypoint != null)
        {
            enemy.UpdateJumpingAnimation();
        }
        else
        {
            enemy.UpdateGoalWaypoint(enemy.m_mapManager.GetWaypointInDirectionOfTarget(
                enemy.m_currentWaypoint, 
                enemy.m_chaseTarget.position)
                );

            enemy.m_waitTime = 0.0f;
            enemy.m_animationTime = 0.0f;
            enemy.m_isJumping = true;
        }

        enemy.m_waitTime += Time.deltaTime;
    }

    #region State Transitions

    public void ToEnterState()
    {
        
    }

    public void ToChaseState()
    {
        // Can't transition to same state
    }

    public void ToReachBottomState()
    {

    }

    public void ToExitState()
    {

    }

    #endregion

    #region State Methods

    private bool FailSafe()
    {
        if (!enemy.m_mapManager.IsNavMapReady())
        {
            //Debug.LogWarning("NAVMAP IS NOT READ!");
            return false;
        }

        // Get current waypoint if not existing already
        if (enemy.m_currentWaypoint == null)
        {
            enemy.m_currentWaypoint = enemy.m_mapManager.FindClosestWaypoint(enemy.m_enemyAnchor.transform.position);
        }

        if (enemy.m_chaseTarget == null)
        {
            Debug.LogError("ERROR: enemy.m_chaseTarget does not exist!");
            return false;
        }

        return true;
    }

    #endregion
}

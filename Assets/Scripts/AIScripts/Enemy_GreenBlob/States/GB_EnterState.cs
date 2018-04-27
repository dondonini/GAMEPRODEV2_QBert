using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GB_EnterState : IEnemyStates_SM
{

    private readonly GreenBlob_SM enemy;

    // This height is relative to the spawn block
    private float m_spawnHeight = 5.0f;

    private bool m_isFirstRun = true;

    public GB_EnterState(GreenBlob_SM _statePattern)
    {
        enemy = _statePattern;
    }

    public void UpdateState()
    {
        if (enemy.m_waitTime <= enemy.m_chaseFreq && enemy.m_goalWaypoint != null)
        {
            enemy.UpdateJumpingAnimation();
        }
        else
        {
            if (m_isFirstRun)
            {
                enemy.UpdateGoalWaypoint(PrepEnemy());

                enemy.m_waitTime = 0.0f;
                enemy.m_animationTime = 0.0f;
                enemy.m_isJumping = true;

                m_isFirstRun = false;
            }
            else
            {
                ToReachBottomState();
            }

        }

        enemy.m_waitTime += Time.deltaTime;
    }

    #region State Transitions

    public void ToEnterState()
    {
        // Can't transition to same state
    }

    public void ToChaseState()
    {

    }

    public void ToReachBottomState()
    {
        enemy.m_currentState = enemy.m_reachBottomState;
    }

    public void ToExitState()
    {

    }

    #endregion

    #region State Methods

    private Waypoint PrepEnemy()
    {
        // Gather all spawn WPs
        List<Waypoint> spawnRow = enemy.m_mapManager.GetWPSecondTopRow();

        // Pick random spawn location
        Waypoint selectedSpawn = spawnRow[Random.Range(0, spawnRow.Count)];

        // Build and set enemy to starting position
        Vector3 newPos = selectedSpawn.position + new Vector3(0.0f, m_spawnHeight, 0.0f);
        enemy.m_enemyAnchor.transform.position = newPos;

        // returns the spawn WP
        return selectedSpawn;
    }

    #endregion
}

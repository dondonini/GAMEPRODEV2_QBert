using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GB_ReachBottomState : IEnemyStates_SM
{

    private readonly GreenBlob_SM enemy;

    enum Direction
    {
        Left = 0,
        Right = 1
    };

    private float m_bottomReachedWaitDuration = 1.0f;

    private int m_strafeLength = 0;
    private int m_strafeCount = 0;

    private bool m_firstRun = true;
    private Direction m_direction;

    public GB_ReachBottomState(GreenBlob_SM _statePattern)
    {
        enemy = _statePattern;

        m_firstRun = true;

        // Get random strafing length
        m_strafeLength = Random.Range(enemy.m_strafeMin, enemy.m_strafeMax);
    }

    public void StartState()
    {

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
            if (enemy.m_goalWaypoint)
            {
                enemy.m_enemyAnchor.transform.position = enemy.m_goalWaypoint.position;
            }

            // Transition to chase mode when bottom of map is hit
            if (Mathf.Approximately(enemy.m_enemyAnchor.transform.position.y, enemy.m_mapManager.GetLowestWPInMap()))
            {
                ToExitState();
                return;
            }

            enemy.UpdateGoalWaypoint(GetStrafeWaypoint());

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
        
    }

    public void ToReachBottomState()
    {
        // Can't transition to same state
    }

    public void ToExitState()
    {
        enemy.m_currentState = enemy.m_exitState;
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

        return true;
    }

    private Waypoint GetStrafeWaypoint()
    {
        // Pick a random direction when starting
        if (!m_firstRun)
        {
            m_direction = (Direction)Random.Range(0, 1);

            m_firstRun = false;
        }

        if (m_strafeCount > m_strafeLength)
        {
            m_strafeCount = 0;

            if (m_direction == Direction.Left)
            {
                m_direction = Direction.Right;
            }
            else
            {
                m_direction = Direction.Left;
            }
        }

        Vector3 vectorDirection = Vector3.zero;

        // Determine which direction to go
        if (m_direction == Direction.Right)
        {
            vectorDirection = new Vector3(-1.0f, -1.0f, 1.0f);
        }
        else
        {
            vectorDirection = new Vector3(1.0f, -1.0f, 1.0f);
        }

        // Increment strafe count
        m_strafeCount++;

        Waypoint newGoal = enemy.m_mapManager.GetWaypointInDirectionOfTarget(
                enemy.m_currentWaypoint,
                enemy.m_currentWaypoint.position + vectorDirection
                );

        // Switch directions when side of map is hit
        if (newGoal == enemy.m_currentWaypoint)
        {
            m_strafeCount = 0;

            if (m_direction == Direction.Left)
            {
                m_direction = Direction.Right;
            }
            else
            {
                m_direction = Direction.Left;
            }

            return GetStrafeWaypoint();
        }
        else
        {
            return newGoal;
        }
    }

    #endregion
}

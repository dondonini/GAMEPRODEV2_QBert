using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GB_ExitState : IEnemyStates_SM {

    enum Direction
    {
        TL,
        TR,
        BL,
        BR
    }

    private readonly GreenBlob_SM enemy;

    // This is relative to the world; not the map.
    private float m_deathHeight = -10.0f;
    private Vector3 m_goalPosition;

    private bool m_isFirstRun = true;

    public GB_ExitState(GreenBlob_SM _statePattern)
    {
        enemy = _statePattern;

       
    }

    public void UpdateState()
    {
        Object.Destroy(enemy.gameObject);
        //if (enemy.m_waitTime <= enemy.m_chaseFreq && enemy.m_goalWaypoint != null)
        //{
        //    enemy.UpdateJumpingAnimation(m_goalPosition);
        //}
        //else
        //{
        //    if (m_isFirstRun)
        //    {
        //        enemy.m_prevWaypoint = enemy.m_currentWaypoint;
        //        enemy.m_goalWaypoint = null;

        //        enemy.m_waitTime = 0.0f;
        //        enemy.m_animationTime = 0.0f;
        //        enemy.m_isJumping = true;

        //        m_isFirstRun = false;
        //    }
        //    else
        //    {
        //        ToReachBottomState();
        //    }

        //}
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

    }

    public void ToExitState()
    {
        // Can't transition to same state
    }

    #endregion

    #region State Methods

    #endregion
}

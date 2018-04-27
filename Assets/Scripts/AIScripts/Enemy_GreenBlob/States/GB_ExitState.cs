using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GB_ExitState : IEnemyStates_SM {

    private readonly GreenBlob_SM enemy;

    // This is relative to the world; not the map.
    private float m_deathHeight = -10.0f;
    private Vector3 m_goalPosition;

    private bool m_isFirstRun = true;

    private float m_launchVelocity = 200.0f;

    private float m_deleteDelay = 5.0f;

    public GB_ExitState(GreenBlob_SM _statePattern)
    {
        enemy = _statePattern;

    }

    public void StartState()
    {
        LaunchPlayer(enemy.m_direction);
    }

    public void UpdateState()
    {
        if (enemy.m_waitTime >= m_deleteDelay)
        {
            Object.Destroy(enemy.gameObject);
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

    }

    public void ToExitState()
    {
        // Can't transition to same state
    }

    #endregion

    #region State Methods

    private void LaunchPlayer(GreenBlob_SM.Direction d)
    {
        // Get launch direction
        Vector3 newDirection = Vector3.zero;

        if (d == GreenBlob_SM.Direction.TL)
        {
            newDirection = new Vector3(1.0f, 1.0f, 0.0f);
        }
        else if (d == GreenBlob_SM.Direction.TR)
        {
            newDirection = new Vector3(-1.0f, 1.0f, 0.0f);
        }
        else if (d == GreenBlob_SM.Direction.BL)
        {
            newDirection = new Vector3(1.0f, 1.0f, 0.0f);
        }
        else if (d == GreenBlob_SM.Direction.BR)
        {
            newDirection = new Vector3(-1.0f, 1.0f, 0.0f);
        }

        // Disabling kinematic
        Rigidbody rigidBody = enemy.GetComponent<Rigidbody>();
        rigidBody.isKinematic = false;

        // Launch player
        rigidBody.AddForce(newDirection * m_launchVelocity);

    }

    #endregion
}

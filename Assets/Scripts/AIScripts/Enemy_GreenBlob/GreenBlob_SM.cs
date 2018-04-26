using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBlob_SM : MonoBehaviour {

    [Tooltip("The speed the AI is going to chase the player in seconds.")]
    public float m_chaseSpeed = 1.0f;

    [HideInInspector]
    public Transform m_chaseTarget = null;

    [ReadOnly]
    public Waypoint m_currentWaypoint = null;

    // //////
    // States
    // //////
    [HideInInspector]
    public EnemyStates_SM m_currentState;
    private EnemyStates_SM m_previousState;

    [HideInInspector]
    public GB_ChaseState m_chaseState;

    

    private void Awake()
    {
        m_chaseState = new GB_ChaseState(this);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        m_currentState.UpdateState();

        if (m_previousState != null && m_previousState != m_currentState)
        {
            Debug.Log("GreenBlob state changed! " + m_previousState + " -> " + m_currentState);
        }

        m_previousState = m_currentState;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coily_SM : MonoBehaviour {

    public enum Direction
    {
        TL, //  ( 1,  1,  -1)
        TR, //  (-1,  1,  -1)
        BL, //  ( 1, -1,   1)
        BR  //  (-1, -1,   1)
    }

    [Tooltip("The speed the AI is going to chase the player in seconds.")]
    public float m_chaseFreq = 1.0f;

    public float m_animationDuration = 0.5f;

    public float m_jumpArchHeight = 1.0f;

    public int m_strafeMin = 1;
    public int m_strafeMax = 3;

    public GameObject m_enemyAnchor;
    public SpriteRenderer m_enemyVisual;
    public Animator m_animator;

    [Space]

    [ReadOnly]
    public Transform m_chaseTarget = null;

    [Header("Waypoint Debugging")]

    [ReadOnly]
    public Waypoint m_prevWaypoint = null;

    [ReadOnly]
    public Waypoint m_currentWaypoint = null;

    [ReadOnly]
    public Waypoint m_goalWaypoint = null;

    [ReadOnly]
    public bool m_isBall = true;

    [ReadOnly]
    public bool m_isJumping = false;

    public Direction m_direction = Direction.BL;

    [HideInInspector]
    public float m_animationTime = 0.0f;
    [HideInInspector]
    public float m_waitTime = Mathf.Infinity;

    // //////
    // States
    // //////
    [HideInInspector]
    public IEnemyStates_SM m_currentState;
    private IEnemyStates_SM m_previousState;

    [HideInInspector]
    public Co_EnterState m_enterState;
    [HideInInspector]
    public Co_ChaseState m_chaseState;
    [HideInInspector]
    public Co_ReachBottomState m_reachBottomState;

    [HideInInspector]
    public MapManager m_mapManager;

    private void OnValidate()
    {
        // Prevent animation duration from surpassing the chase speed
        if (m_chaseFreq < m_animationDuration)
        {
            m_animationDuration = m_chaseFreq;
        }

        if (m_strafeMin < 1)
        {
            m_strafeMin = 1;
        }
    }

    void Awake()
    {
        m_enterState        = new Co_EnterState(this);
        m_chaseState        = new Co_ChaseState(this);
        m_reachBottomState  = new Co_ReachBottomState(this);
    }

    // Use this for initialization
    void Start () {

        m_currentState = m_enterState;

        if (!m_chaseTarget)
        {
            m_chaseTarget = GameObject.FindGameObjectWithTag("Player").transform;
        }

        m_mapManager = MapManager.instance;

    }
	
	// Update is called once per frame
	void Update () {

        if (m_previousState != null && m_previousState != m_currentState)
        {
            Debug.Log("Coily state changed! " + m_previousState + " -> " + m_currentState);

            // Initiate start
            m_currentState.StartState();
        }

        m_currentState.UpdateState();

        m_previousState = m_currentState;
	}

    // Updates movement animation
    // This is actually horrible and inefficent. Please don't judge.
    public void UpdateJumpingAnimation()
    {
        Vector3 startPos = Vector3.zero;
        Vector3 endPos = m_goalWaypoint.position;

        m_animator.SetBool("isBall", m_isBall);
        m_animator.SetBool("Jumping", m_isJumping);

        if (m_prevWaypoint)
            startPos = m_prevWaypoint.position;
        else
            startPos = m_enemyAnchor.transform.position;

        

        // Stop from animation when not moving
        if (startPos != endPos)
        {
            // Animating
            if (m_animationTime <= m_animationDuration)
            {
                float p = m_animationTime / m_animationDuration;

                // Get top of arch
                float highestPos = Mathf.Max(startPos.y, endPos.y);
                float topOfArch = highestPos + m_jumpArchHeight;

                // Getting half way point
                Vector3 halfPoint = new Vector3(
                    Mathf.Lerp(startPos.x, endPos.x, 0.5f),
                    topOfArch,
                    Mathf.Lerp(startPos.z, endPos.z, 0.5f)
                    );

                // Final Positions
                float newX, newY, newZ = 0;

                // Animation code (LMAO this is bad)

                // FIRST HALF OF ARCH
                if (p < 0.25f)
                {
                    float newP = p / 0.25f;

                    newX = EasingFunction.Linear(startPos.x, halfPoint.x, newP);
                    newY = EasingFunction.EaseOutCirc(startPos.y, halfPoint.y, newP);
                    newZ = EasingFunction.Linear(startPos.z, halfPoint.z, newP);
                }
                // SECOND HALF OF ARCH
                else
                {
                    float newP = (p - 0.25f) / 0.75f;

                    newX = EasingFunction.Linear(halfPoint.x, endPos.x, Mathf.Clamp01(newP / 0.35f));
                    newY = EasingFunction.EaseOutBounce(halfPoint.y, endPos.y, newP);
                    newZ = EasingFunction.Linear(halfPoint.z, endPos.z, Mathf.Clamp01(newP / 0.35f));
                }

                if (Mathf.Clamp01((p - 0.25f) / 0.75f / 0.35f) == 1.0f)
                {
                    m_isJumping = false;
                }

                m_enemyAnchor.transform.position = new Vector3(newX, newY, newZ);

                m_animationTime += Time.deltaTime;
            }
            // Reached end
            else
            {
                m_enemyAnchor.transform.position = m_goalWaypoint.position;
            }
        }
    }

    public void UpdateGoalWaypoint(Waypoint nWP)
    {
        // Perserve previous waypoint
        m_prevWaypoint = m_currentWaypoint;

        // Get new goal waypoint
        m_goalWaypoint = nWP;

        // Changing current waypoint to new waypoint
        m_currentWaypoint = m_goalWaypoint;

        // Updating occupation on previous and new waypoints
        m_goalWaypoint.SetOccupent(m_enemyAnchor);
        if (m_prevWaypoint) m_prevWaypoint.SetEmpty();

        if (m_goalWaypoint && m_prevWaypoint)
        {
            // Calculating direction
            Vector3 newDirection = m_goalWaypoint.position - m_prevWaypoint.position;
            newDirection.Normalize();

            if (newDirection.x > 0 && newDirection.y > 0 && newDirection.z < 0)
            {
                // Going up left
                m_direction = Direction.TL;
            }
            else if (newDirection.x < 0 && newDirection.y > 0 && newDirection.z < 0)
            {
                // Going up right
                m_direction = Direction.TR;
            }
            else if (newDirection.x > 0 && newDirection.y < 0 && newDirection.z > 0)
            {
                // Going down left
                m_direction = Direction.BL;
            }
            else if (newDirection.x < 0 && newDirection.y < 0 && newDirection.z > 0)
            {
                // Going down right
                m_direction = Direction.BR;
            }
        }

        if (m_direction == Direction.TL)
        {
            m_animator.SetInteger("Direction", 0);
        }
        else if (m_direction == Direction.TR)
        {
            m_animator.SetInteger("Direction", 1);
        }
        else if (m_direction == Direction.BL)
        {
            m_animator.SetInteger("Direction", 2);
        }
        else if (m_direction == Direction.BR)
        {
            m_animator.SetInteger("Direction", 3);
        }
    }
}

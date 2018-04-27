using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    enum Direction
    {
        TL, //  ( 1,  1,  -1)
        TR, //  (-1,  1,  -1)
        BL, //  ( 1, -1,   1)
        BR  //  (-1, -1,   1)
    }

    public float m_moveFreq = 0.5f;
    public float m_animationDuration;
    public float m_jumpArchHeight;

    [Space]

    [ReadOnly]
    public Waypoint m_prevWaypoint = null;

    [ReadOnly]
    public Waypoint m_currentWaypoint = null;

    [ReadOnly]
    public Waypoint m_goalWaypoint = null;

    private bool m_isMoving = false;
    private bool m_isJumping = false;

    private Vector3 startPos;
    private Vector3 endPos;

    private float m_animationTime;
    private float m_waitTime;

    private MapManager m_mapManager;
    private GameInfo m_gameInfo;

    private void OnValidate()
    {
        // Prevent animation duration from surpassing the chase speed
        if (m_moveFreq < m_animationDuration)
        {
            m_animationDuration = m_moveFreq;
        }
    }

    // Use this for initialization
    void Start () {

        m_mapManager = MapManager.instance;
        m_gameInfo = GameInfo.instance;
	}
	
	// Update is called once per frame
	void Update () {
        InputManager();


        if (m_isMoving)
        {
            if (m_waitTime <= m_moveFreq && m_goalWaypoint != null)
            {
                UpdateJumpingAnimation();
            }
            else
            {
                m_isMoving = false;
            }
        }

        m_waitTime += Time.deltaTime;
    }

    void InputManager()
    {
        if (m_isMoving) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            m_goalWaypoint = m_mapManager.FindClosestWaypoint(transform.position + new Vector3(-1, 1, -1) * 0.6f);
            StartMovement();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            m_goalWaypoint = m_mapManager.FindClosestWaypoint(transform.position + new Vector3(1, 1, -1) * 0.6f);
            StartMovement();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            m_goalWaypoint = m_mapManager.FindClosestWaypoint(transform.position + new Vector3(1, -1, 1) * 0.6f);
            StartMovement();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            m_goalWaypoint = m_mapManager.FindClosestWaypoint(transform.position + new Vector3(-1, -1, 1) * 0.6f);
            StartMovement();
        }
    }

    void StartMovement()
    {
        m_isMoving = true;
        m_isJumping = true;
        m_waitTime = 0.0f;
        m_animationTime = 0.0f;
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
        m_goalWaypoint.SetOccupent(gameObject);
        if (m_prevWaypoint) m_prevWaypoint.SetEmpty();
    }

    // Updates movement animation
    // This is actually horrible and inefficent. Please don't judge.
    public void UpdateJumpingAnimation()
    {
        Vector3 startPos = Vector3.zero;
        Vector3 endPos = m_goalWaypoint.position;

        if (m_prevWaypoint)
            startPos = m_prevWaypoint.position;
        else
            startPos = transform.position;



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
                    ChangeBlockColor(m_mapManager.GetMapPartFromWaypoint(m_goalWaypoint));
                }

                transform.position = new Vector3(newX, newY, newZ);

                m_animationTime += Time.deltaTime;
            }
            // Reached end
            else
            {
                transform.position = m_goalWaypoint.position;
            }
        }
    }

    void ChangeBlockColor(GameObject b)
    {
        b.GetComponent<Renderer>().material = m_gameInfo.m_mapChangeColor;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    enum Direction
    {
        TL, //  ( 1,  1,  -1)
        TR, //  (-1,  1,  -1)
        BL, //  ( 1, -1,   1)
        BR  //  (-1, -1,   1)
    }

    public float m_moveFreq = 0.5f;
    public float m_animationDuration = 0.5f;
    public float m_jumpArchHeight = 0.2f;
    public float m_launchVelocity = 200.0f;
    public int m_lives = 3;

    [Header("Waypoint Data")]

    [ReadOnly]
    public Waypoint m_prevWaypoint = null;

    [ReadOnly]
    public Waypoint m_currentWaypoint = null;

    [ReadOnly]
    public Waypoint m_goalWaypoint = null;

    [Header("Attachments")]
    public GameObject p_FUCK;
    public ScoringUI m_scoringUI;

    private bool m_isMoving = false;
    private bool m_isJumping = false;
    private bool m_isDead = false;

    private Vector3 startPos;
    private Vector3 endPos;

    private float m_animationTime;
    private float m_waitTime;

    private Direction m_direction = Direction.TL;

    private MapManager m_mapManager;
    private GameInfo m_gameInfo;
    private GameManager m_gameManager;

    private Animator m_characterAnimator;

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

        Initialize();

    }

    void Initialize()
    {
        m_mapManager = MapManager.instance;
        m_gameInfo = GameInfo.instance;
        m_gameManager = GameManager.instance;

        m_characterAnimator = GetComponent<Animator>();

        // Activating inital block
        m_goalWaypoint = m_mapManager.FindClosestWaypoint(transform.position);
        ChangeBlockColor(m_mapManager.GetMapPartFromWaypoint(m_goalWaypoint));
    }

    // Update is called once per frame
    void Update () {
        if (m_isDead) return;

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

        if (m_isJumping)
        {
            m_characterAnimator.SetBool("Jumping", true);
        }
        else
        {
            m_characterAnimator.SetBool("Jumping", false);
        }

        m_waitTime += Time.deltaTime;
    }

    void InputManager()
    {
        if (m_isMoving) return;

        // Up Right
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Keypad9))
        {
            UpdateGoalWaypoint(m_mapManager.FindClosestWaypoint(transform.position + new Vector3(-1, 1, -1) * 0.6f));
            m_characterAnimator.SetInteger("Direction", 1);
            StartMovement();
        }

        // Up Left
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            UpdateGoalWaypoint(m_mapManager.FindClosestWaypoint(transform.position + new Vector3(1, 1, -1) * 0.6f));
            m_characterAnimator.SetInteger("Direction", 0);
            StartMovement();
        }

        // Down Left
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            UpdateGoalWaypoint(m_mapManager.FindClosestWaypoint(transform.position + new Vector3(1, -1, 1) * 0.6f));
            m_characterAnimator.SetInteger("Direction", 3);
            StartMovement();
        }

        // Down Right
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            UpdateGoalWaypoint(m_mapManager.FindClosestWaypoint(transform.position + new Vector3(-1, -1, 1) * 0.6f));
            m_characterAnimator.SetInteger("Direction", 2);
            StartMovement();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_gameManager.PauseGame(!m_gameManager.IsGamePaused());
            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint cp in collision)
        {
            if (cp.otherCollider.CompareTag("Enemy") && !m_isDead)
            {
                OnDeath();
                break;
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Enemy") && !m_isDead)
    //    {
    //        OnDeath();
    //    }
    //}

    void StartMovement()
    {
        m_isMoving = true;

        if (m_goalWaypoint == m_prevWaypoint)
        {
            OnDeath();
            return;
        }

        m_isJumping = true;
        m_waitTime = 0.0f;
        m_animationTime = 0.0f;
    }

    void OnDeath()
    {
        if (m_isDead) return;

        m_isDead = true;


        if (m_isMoving && m_goalWaypoint == m_prevWaypoint)
        {
            LaunchPlayer(m_direction);
        }
        else
        {
            p_FUCK.SetActive(true);
            m_gameManager.FreezeGame(true);
        }

        m_lives--;

        m_gameManager.OnPlayerDeath();
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
        m_gameManager.SetBlock(b, true);
    }

    private void LaunchPlayer(Direction d)
    {
        // Get launch direction
        Vector3 newDirection = Vector3.zero;

        if (d == Direction.TL)
        {
            newDirection = new Vector3(1.0f, 1.0f, 0.0f);
        }
        else if (d == Direction.TR)
        {
            newDirection = new Vector3(-1.0f, 1.0f, 0.0f);
        }
        else if (d == Direction.BL)
        {
            newDirection = new Vector3(1.0f, 1.0f, 0.0f);
        }
        else if (d == Direction.BR)
        {
            newDirection = new Vector3(-1.0f, 1.0f, 0.0f);
        }

        // Disabling kinematic
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        //rigidBody.isKinematic = false;

        rigidBody.constraints = RigidbodyConstraints.None;


        // Launch player
        rigidBody.AddForce(newDirection * m_launchVelocity);

    }
}

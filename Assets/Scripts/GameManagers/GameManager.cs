using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    [Tooltip("The wait before the game starts")]
    public float m_startDelay = 1.0f;

    [Tooltip("The amount of points you get every time you change a tile")]
    public int m_activatePointAmount = 25;

    public float m_redBlobSpawnFreq = 3.0f;
    public float m_greenBlobSpawnFreq = 7.0f;
    public float m_coilySpawnFreq = 2.0f;

    [Space]
    [Header("Prefabs")]
    public GameObject p_Coily;
    public GameObject p_GreenBlob;
    public GameObject p_RedBlob;

    [Header("Attachments")]
    public ScoringUI m_scoringUI;
    public Canvas m_PauseScreen;
    public Canvas m_GameOverScreen;
    public PlayerController m_player;

    // Runtime Variables
    private float m_percentageDone = 0.0f;
    private float m_timer = 0.0f;
    private float m_rbTimer = 0.0f;
    private float m_gbTimer = 0.0f;
    private float m_coTimer = 0.0f;
    private bool m_isGamePaused = false;
    private bool m_isGameOver = false;

    // Current Coily mob in the map;
    private GameObject m_currentCoily = null;

    private Vector3 m_spawnPosition = new Vector3(0.0f, 100.0f, 0.0f);

    // Global Variables
    private MapManager m_mapManager;
    private GameInfo m_gameInfo;

	// Use this for initialization
	void Awake () {

        if (!instance)
        {
            instance = this;
        }

        if (instance != this)
        {
            Destroy(this);
        }

        // Initialization
        Initialize();
    }
	
    void Initialize()
    {
        // Collecting global scripts
        m_mapManager = MapManager.instance;
        m_gameInfo = GameInfo.instance;
    }

    private void Update()
    {
        if (m_player.m_lives == 0)
        {
            m_isGameOver = true;
            m_GameOverScreen.enabled = true;
        }

        if (m_isGameOver)
        {
            if (m_timer >= 3.0f)
            {
                FreezeGame(false);
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                m_timer += Time.unscaledDeltaTime;
            }
            return;
        }

        if (m_timer >= m_startDelay)
        {
            UpdateGame();
        }
        else
        {
            m_timer += Time.deltaTime;
        }
    }

    private void UpdateGame()
    {
        // Spawn Coily
        if (m_coTimer >= m_coilySpawnFreq && !m_currentCoily)
        {
            // Spawn
            SpawnCoily();

            // Reset timer
            m_coTimer = 0.0f;
        }
        else if (!m_currentCoily)
        {
            // Update timer when coily is dead
            m_coTimer += Time.deltaTime;
        }

        // Spawn Red Blob
        if(m_rbTimer >= m_redBlobSpawnFreq)
        {
            // Spawn
            SpawnRedBlob();

            // Reset timer
            m_rbTimer = 0.0f;
        }
        else
        {
            // Update timer when coily is dead
            m_rbTimer += Time.deltaTime;
        }

        // Spawn Green Blob
        if (m_gbTimer >= m_greenBlobSpawnFreq)
        {
            // Spawn
            SpawnGreenBlob();

            // Reset timer
            m_gbTimer = 0.0f;
        }
        else
        {
            // Update timer when coily is dead
            m_gbTimer += Time.deltaTime;
        }
    }

    public void PauseGame(bool _set)
    {
        if (_set)
        {
            m_isGamePaused = true;
            m_PauseScreen.enabled = true;
            FreezeGame(true);
        }
        else
        {
            m_isGamePaused = false;
            m_PauseScreen.enabled = false;
            FreezeGame(false);
        }
        
    }

    public void FreezeGame(bool _set)
    {
        if (_set)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    public bool IsGamePaused()
    {
        return m_isGamePaused;
    }

    public void OnPlayerDeath()
    {
        m_isGameOver = true;
    }

    // /////////////
    #region Spawners
    // /////////////

    private void SpawnCoily()
    {
        if (m_currentCoily) return;

        GameObject newCoily = Instantiate(p_Coily) as GameObject;

        newCoily.transform.position = m_spawnPosition;

        m_currentCoily = newCoily;
    }

    private void SpawnRedBlob()
    {
        GameObject newRedBlob = Instantiate(p_RedBlob) as GameObject;

        newRedBlob.transform.position = m_spawnPosition;
    }


    private void SpawnGreenBlob()
    {
        GameObject newGreenBlob = Instantiate(p_GreenBlob) as GameObject;

        newGreenBlob.transform.position = m_spawnPosition;
    }

    #endregion

    // //////////////////////
    #region Getters & Setters
    // //////////////////////

    #endregion

    // ////////////////////
    #region SetBlock Method
    // ////////////////////

    public void SetBlock(Waypoint _waypoint, bool _activate = false)
    {
        GameObject mapPart = m_mapManager.GetMapPartFromWaypoint(_waypoint);

        if (mapPart)
        {
            SetBlock(mapPart, _activate);
        }
    }

    public void SetBlock(GameObject _part, bool _activate = false)
    {
        BlockInfo partBlockInfo = _part.GetComponent<BlockInfo>();

        if (_activate)
        {
            if (!partBlockInfo.IsActivated)
            {
                partBlockInfo.IsActivated = true;
                m_gameInfo.Score += m_activatePointAmount;
            }
        }
        else
        {
            partBlockInfo.IsActivated = false;
            
            
        }
        partBlockInfo.UpdateVisual();
    }

    #endregion
}

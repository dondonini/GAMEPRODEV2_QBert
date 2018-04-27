using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapManager : MonoBehaviour {

    public static MapManager instance = null;

    // //////////////////
    [Header("Variables")]
    // //////////////////

    [Tooltip("Discover distance when searching for nearby waypoints.")]
    public float m_searchTolerance = 2.0f;

    // /////////////////////////////
    [Header("Assignable Variables")]
    // /////////////////////////////

    public GameObject m_groundPrefab;
    public GameObject m_waypointPrefab;

    public Transform m_mapFolder;
    public Transform m_waypointsFolder;

    

    // /////////////////
    // Runtime Variables
    // /////////////////

    // Reference Lists
    private List<Waypoint> m_navPointMapGrid;
    private List<GameObject> m_mapParts;
    private List<BlockInfo> m_mapInfo;
    private List<Waypoint> m_secondTopRow;

    // Map Bounds
    private Bounds m_mapBounds;
    private float m_topWPHeight = -Mathf.Infinity;
    private float m_bottomWPHeight = Mathf.Infinity;

    // Validations
    private bool m_isMapReady = false;
    private bool m_isNavMapReady = false;

    // Loading Progress Info
    private float m_progress = 0.0f;
    private int m_taskTotal = 0;
    private int m_taskProgress = 0;
    private string m_currentTask;

    // Global Scripts
    private GameInfo m_gameInfo;
    private GameManager m_gameManager;

    #region Singleton

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        InitMap();
    }
	
	

#endregion

    void InitMap()
    {
        // Initializing all variables
        m_navPointMapGrid = new List<Waypoint>();
        m_mapParts = new List<GameObject>();
        m_mapInfo = new List<BlockInfo>();
        m_mapBounds = new Bounds();

        // Collect global scripts
        m_gameInfo = GameInfo.instance;
        m_gameManager = GameManager.instance;

        if (m_mapFolder.childCount > 0)
            m_isMapReady = BuildMap(m_mapFolder);
        else
            m_isMapReady = BuildMap(7);


        if (m_isMapReady)
        {
            Debug.Log("MapManager: Map is ready!");
            m_isNavMapReady = BuildNavPointMap();
        }

        if (m_isNavMapReady)
        {
            Debug.Log("MapManager: NavMap is ready!");
        }
    }

    // ////////////
    #region Map Builder
    // ////////////

    /// <summary>
    /// Builds the map
    /// </summary>
    /// <param name="height">Height of the map</param>
    /// <returns>success</returns>
    public bool BuildMap(int height)
    {
        // Progress stats
        m_currentTask = "Building Map";
        m_taskTotal = 0;
        m_taskProgress = 0;

        float partDiagonalLength = MathExtras.CalculateDiagonalOfSquare(m_groundPrefab.transform.localScale.x);
        float partHeight = m_groundPrefab.transform.localScale.y;

        for (int r = height; r >= 0; r--)
        {
            // Calculate Y pos
            float newPosY = partHeight * r + (partHeight * 0.5f);

            // Calculate Z pos
            float newPosZ = (partDiagonalLength * 0.5f) * (height - r);

            float rowLength = partDiagonalLength * (height - r);

            for (int c = 0; c < (height - r); c++)
            {
                // Calculate X pos
                // TODO: It's still off centered!
                float newPosX = -(rowLength * 0.5f) + (partDiagonalLength * c);

                // Build new part
                GameObject newGroundPart = Instantiate(m_groundPrefab) as GameObject;
                newGroundPart.transform.position = new Vector3(newPosX, newPosY, newPosZ);
                newGroundPart.transform.eulerAngles = new Vector3(0.0f, 45.0f, 0.0f);
                newGroundPart.transform.SetParent(m_mapFolder);

                // Add part in list for future reference
                m_mapParts.Add(newGroundPart);

                BlockInfo newBlockInfo = newGroundPart.GetComponent<BlockInfo>();
                if (newBlockInfo)
                    m_mapInfo.Add(newBlockInfo);
            }
        }

        return true;
    }

    /// <summary>
    /// Collects segments of existing map
    /// </summary>
    /// <param name="map"></param>
    /// <returns></returns>
    public bool BuildMap(Transform map)
    {
        // Progress stats
        m_currentTask = "Building Map";
        m_taskTotal = 0;
        m_taskProgress = 0;

        // Breaks if map is empty
        if (map.childCount <= 0)
        {
            Debug.LogWarning("Map is empty! Build not successful.");
            return false;
        }

        // Progress stats
        m_currentTask = "Collecting Map Segments";
        m_taskTotal = map.childCount;

        // Collecting parts
        foreach (Transform segment in map)
        {
            if (segment.CompareTag("Ground") || segment.CompareTag("Teleporter"))
            {
                m_mapParts.Add(segment.gameObject);

                BlockInfo newBlockInfo = segment.GetComponent<BlockInfo>();
                if (newBlockInfo)
                    m_mapInfo.Add(newBlockInfo);
            }
            m_taskProgress++;
        }

        return true;
    }

    #endregion

    // ///////////////
    #region NavMap Builder
    // ///////////////

    /// <summary>
    /// Builds navmap based on physical map.
    /// </summary>
    public bool BuildNavPointMap()
    {
        // Progress stats
        m_currentTask = "Building NavPointMap";
        m_taskProgress = 0;
        m_taskTotal = m_mapParts.Count;


        // Building NavPointMap
        foreach (GameObject ground in m_mapParts)
        {
            // Getting top point of the ground piece
            Vector3 newPosition = ground.transform.position + new Vector3(0.0f, ground.transform.localScale.y * 0.5f, 0.0f);

            // Creating and positioning waypoint
            GameObject newWaypoint = Instantiate(m_waypointPrefab) as GameObject;
            newWaypoint.transform.position = newPosition;
            newWaypoint.transform.SetParent(m_waypointsFolder);

            // Adding waypoint to list for future reference
            Waypoint getWaypoint = newWaypoint.GetComponent<Waypoint>();

            // Set waypoint to only be used for player if tag matches
            if (newWaypoint.CompareTag("Teleporter"))
            {
                getWaypoint.m_playerWaypointOnly = true;
            }

            m_navPointMapGrid.Add(getWaypoint);

            // Check for highest point on map
            m_topWPHeight = Mathf.Max(m_topWPHeight, newPosition.y);

            // Check for lowest point on map
            m_bottomWPHeight = Mathf.Min(m_bottomWPHeight, newPosition.y);

            m_taskProgress++;
        }

        // Progress stats
        m_currentTask = "Connecting points";
        m_taskProgress = 0;
        m_taskTotal = m_navPointMapGrid.Count;

        // Connecting all waypoints to each other
        foreach (Waypoint waypoint in m_navPointMapGrid)
        {
            Waypoint[] neighboursFound = FindSurroundingNavPoints(waypoint);

            if (neighboursFound.Length == 0)
            {
                Debug.LogWarning("Waypoint at position (" + waypoint.transform.position + ") is too far from other waypoints!");
            }
            else
            {
                waypoint.m_neighbors.AddRange(neighboursFound);
            }

            m_taskProgress++;
        }

        return true;
    }

    /// <summary>
    /// Find surround navpoints on selected waypoint
    /// </summary>
    /// <param name="current">Selected waypoint</param>
    /// <returns>All surrounding waypoints</returns>
    private Waypoint[] FindSurroundingNavPoints(Waypoint current)
    {
        List<Waypoint> tempWayPoints = new List<Waypoint>();

        // Searching for waypoints nearby
        foreach (Waypoint w in m_navPointMapGrid)
        {
            if (current != w)
            {
                if (Vector3.Distance(current.transform.position, w.transform.position) < m_searchTolerance 
                    && current.transform.position.y != w.transform.position.y)
                {
                    tempWayPoints.Add(w);
                }
            }
        }

        return tempWayPoints.ToArray();
    }

    #endregion

    // ///////////////////////
    #region Map Spawning Animation
    // ///////////////////////



    #endregion

    // ///////////////////////////
    #region Collect Second Top Row
    // ///////////////////////////

    public List<Waypoint> GetWPSecondTopRow()
    {
        if (m_secondTopRow == null)
        {
            foreach (Waypoint w in m_navPointMapGrid)
            {
                if (w.position.y == m_topWPHeight)
                {
                    m_secondTopRow = new List<Waypoint>();
                    m_secondTopRow.AddRange(w.m_neighbors);
                }
            }
        }

        return m_secondTopRow;
    }

#endregion

    // ///////////////
    #region Helper Methods
    // ///////////////

    public Bounds GetMapBounds()
    {
        Bounds newBound = new Bounds();

        if (!m_isMapReady)
            return newBound;

        foreach (GameObject part in m_mapParts)
        {
            newBound.Encapsulate(part.transform.position);
        }

        return newBound;
    }

    // Find the closest waypoint from target
    public Waypoint FindClosestWaypoint(Vector3 t)
    {
        Waypoint closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Waypoint waypoint in m_navPointMapGrid)
        {
            float dist = (waypoint.transform.position - t).magnitude;

            if (dist < closestDist)
            {
                closest = waypoint;
                closestDist = dist;
            }
        }

        return closest;
    }

    // Find closest waypoint towards the target
    public Waypoint GetWaypointInDirectionOfTarget(Waypoint wp, Vector3 t)
    {
        float distance = Mathf.Infinity;
        Waypoint closestWaypoint = null;

        // Check all neighbors in waypoint to see which one is closest to target
        // TODO: This code will always go for the closest waypoint. So if the enemy is in a dead end, it will never get out!
        for (int w = 0; w < wp.m_neighbors.Count; w++)
        {
            Waypoint selectedWaypoint = wp.m_neighbors[w];

            float distanceToTarget = Vector3.Distance(selectedWaypoint.position, t);

            if (distanceToTarget < distance)
            {
                closestWaypoint = selectedWaypoint;
                distance = distanceToTarget;
            }
        }

        if (Vector3.Distance(wp.position, t) < 0.5f)
            if (Vector3.Distance(wp.position, t) < distance)
            {
                closestWaypoint = wp;
            }

        return closestWaypoint;
    }

    // Find closest waypoint towards the target
    public Waypoint GetWaypointInDirectionOfTarget(Vector3 p, Vector3 t)
    {
        float distance = Mathf.Infinity;
        Waypoint closestWaypoint = null;
        Waypoint wp = FindClosestWaypoint(p);

        // Check all neighbors in waypoint to see which one is closest to target
        // TODO: This code will always go for the closest waypoint. So if the enemy is in a dead end, it will never get out!
        for (int w = 0; w < wp.m_neighbors.Count; w++)
        {
            Waypoint selectedWaypoint = wp.m_neighbors[w];

            float distanceToTarget = Vector3.Distance(selectedWaypoint.position, t);

            if (distanceToTarget < distance)
            {
                closestWaypoint = selectedWaypoint;
                distance = distanceToTarget;
            }
        }

        if (Vector3.Distance(wp.position, t) < 0.5f)
            if (Vector3.Distance(wp.position, t) < distance)
            {
                closestWaypoint = wp;
            }

        return closestWaypoint;
    }

    public Waypoint[] GetAllWaypoints()
    {
        return m_navPointMapGrid.ToArray();
    }

    public GameObject[] GetAllGroundSegments()
    {
        return m_mapParts.ToArray();
    }

    public bool IsMapReady()
    {
        return m_isMapReady;
    }

    public bool IsNavMapReady()
    {
        return m_isNavMapReady;
    }

    public float GetLowestWPInMap()
    {
        return m_bottomWPHeight;
    }

    public GameObject GetMapPartFromWaypoint(Waypoint waypoint)
    {
        for(int i = 0; i < m_navPointMapGrid.Count; i++)
        {
            if (m_navPointMapGrid[i] == waypoint)
            {
                return m_mapParts[i];
            }
        }

        return null;
    }

#endregion
}

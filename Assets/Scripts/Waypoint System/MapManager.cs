using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    private static MapManager m_instance = null;

    // //////////////////
    [Header("Variables")]
    // //////////////////

    [Tooltip("Discover distance when searching for near by waypoints.")]
    public float m_searchTolerance = 2.0f;

    // /////////////////////////////
    [Header("Assignable Variables")]
    // /////////////////////////////

    public GameObject m_iGroundPart;
    public GameObject m_iWaypoint;

    public Transform m_mapFolder;
    public Transform m_waypointsFolder;

    // /////////////////
    // Runtime Variables
    // /////////////////

    // NavPoint Map Grid in 2D List form
    List<List<Waypoint>> m_navPointMapGrid;

	public static MapManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new MapManager();
            }
            return m_instance;
        }
    }
	
	void Awake()
    {
        m_instance = this;
    }

    void Start()
    {
        // Initializing all variables
        m_navPointMapGrid = new List<List<Waypoint>>();

        BuildMap(5);
    }

    // ///////////
    // Map Builder
    // ///////////

    /// <summary>
    /// Builds Q*Bert map
    /// </summary>
    /// <param name="height">Height of the pyramid</param>
    public void BuildMap(int height)
    {
        float highestPos = height - 0.5f;
        float partDiagonalLength = MathExtras.CalculateDiagonalOfSquare(m_iGroundPart.transform.localScale.x);
        float partHeight = m_iGroundPart.transform.localScale.y;

        for (int r = height; r > 0; r--)
        {
            // Calculate Y pos
            float newPosY = partHeight * r - (partHeight * 0.5f);

            // Calculate Z pos
            float newPosZ = (partDiagonalLength * 0.5f) * r;

            float rowLength = height - r;

            for (int c = 0; c < (height - r); c++)
            {
                // Calculate X pos
                float newPosX = -(rowLength * 0.5f) + (partDiagonalLength * c);

                // Build new part
                GameObject newGroundPart = Instantiate(m_iGroundPart) as GameObject;
                newGroundPart.transform.position = new Vector3(newPosX, newPosY, newPosZ);
                newGroundPart.transform.SetParent(m_mapFolder);
            }
        }
    }

    // //////////////
    // NavMap Builder
    // //////////////

    /// <summary>
    /// Builds navmap based on physical map.
    /// </summary>
    public void BuildNavPointMap()
    {

    }

    private void CollectAllNavPoints()
    {

    }

    /// <summary>
    /// Builds new row in navpoint map grid
    /// </summary>
    /// <returns></returns>
    private List<Waypoint> AddGridRow()
    {
        m_navPointMapGrid.Add(new List<Waypoint>());

        return m_navPointMapGrid[m_navPointMapGrid.Count - 1];
    }

    /// <summary>
    /// Unpacks the whole grid list into one 1D array.
    /// </summary>
    /// <returns></returns>
    private Waypoint[] UnpackNavMapGrid()
    {
        List<Waypoint> tempPack = new List<Waypoint>();

        // Gets every single waypoint in grid list
        if (m_navPointMapGrid != null && m_navPointMapGrid[0] != null)
        {
            foreach(List<Waypoint> r in m_navPointMapGrid)
            {
                foreach(Waypoint w in r)
                {
                    tempPack.Add(w);
                }
            }
        }

        // Returns null if grid is empty
        if (tempPack.Count != 0)
        {
            return tempPack.ToArray();
        }
        else
        {
            return null;
        }
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
        foreach (Waypoint w in UnpackNavMapGrid())
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

        if (tempWayPoints.Count != 0)
        {
            return tempWayPoints.ToArray();
        }
        else
        {
            return null;
        }
    }
}

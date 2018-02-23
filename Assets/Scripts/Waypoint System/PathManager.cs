using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour {

    public float m_walkSpeed = 1.0f;
    public float m_searchTolerance = 2.0f;

    private Stack<Waypoint> m_currentPath;
    private Vector3 m_currentWaypointPosition;
    private float m_moveTimeTotal;
    private float m_moveTimeCurrent;
    private Vector3 m_currentGoal = Vector3.zero;
    private Waypoint m_waypoint;
    private Waypoint m_currentWaypoint;
    private Waypoint m_prevWaypoint;
    

    private MapManager m_mapManager;

    private void Start()
    {
        // Initializing variables
        m_mapManager = MapManager.Instance;
    }

    // Prep and start the AI path and movements
    public void NavigateTo(Vector3 destination)
    {

        if (m_currentGoal != destination && m_currentPath != null)
            Stop();

        else if (m_currentGoal == destination)
            return;

        // Find closest waypoints for currentNode and endNode
        Waypoint currentNode = m_mapManager.FindClosestWaypoint(transform.position);
        Waypoint endNode = m_mapManager.FindClosestWaypoint(destination);

        if (currentNode == endNode)
        {
            currentNode.SetOccupent(gameObject);
            return;
        }

        m_currentPath = new Stack<Waypoint>();

        if (currentNode == null || endNode == null)
            return;

        // Initiallizing open and closed lists
        // Prevents revisiting previously checked waypoints
        WaypointList openList = new WaypointList();
        List<Waypoint> closedList = new List<Waypoint>();

        openList.Add(0, currentNode);
        currentNode.Previous = null;
        currentNode.Distance = 0f;
        m_currentGoal = destination;

        while (openList.Count > 0)
        {
            // Getting current node, move from open to closed list
            currentNode = openList.Pull();
            float dist = currentNode.Distance;
            closedList.Add(currentNode);

            if (currentNode == endNode)
                break;

            foreach (Waypoint neighbor in currentNode.m_neighbors)
            {
                if (closedList.Contains(neighbor) 
                    || openList.ContainsWaypoint(neighbor) 
                    || (neighbor.IsOccupied() && neighbor.GetOccupent().CompareTag("Player") && neighbor.GetOccupent() != gameObject) 
                    || neighbor.m_playerWaypointOnly)
                    continue;

                neighbor.Previous = currentNode;
                neighbor.Distance = dist + (neighbor.position - currentNode.position).magnitude;

                float distanceToTarget = (neighbor.position - endNode.position).magnitude;
                openList.Add(neighbor.Distance + distanceToTarget, neighbor);
            }
        }

        if (currentNode == endNode)
        {
            while (currentNode.Previous != null)
            {
                m_currentPath.Push(currentNode);
                currentNode = currentNode.Previous;
            }

            m_currentPath.Push(m_mapManager.FindClosestWaypoint(transform.position));
        }
    }

    // Stop the current path
    public void Stop()
    {
        m_currentPath = null;
        m_moveTimeTotal = 0;
        m_moveTimeCurrent = 0;
    }

    private void Update()
    {
        if (m_currentPath != null && m_currentPath.Count > 0)
        {

            if (m_moveTimeCurrent < m_moveTimeTotal)
            {
                m_moveTimeCurrent += Time.deltaTime;

                if (m_moveTimeCurrent > m_moveTimeTotal)
                    m_moveTimeCurrent = m_moveTimeTotal;

                Waypoint currentWaypoint = m_currentPath.Peek();

                //transform.position = currentWaypoint.transform.position;
                transform.position = Vector3.Lerp(m_currentWaypointPosition, currentWaypoint.position, m_moveTimeCurrent / m_moveTimeTotal);
            }
            else
            {
                m_currentPath.Peek().SetOccupent(gameObject);

                // Taking new waypoint
                Waypoint currentWaypoint = m_currentPath.Pop();

                if (m_currentPath.Peek().IsOccupied())
                {
                    Vector3 goal = m_currentPath.ElementAt(0).position;
                    Stop();
                    NavigateTo(goal);
                }
                else
                {

                    // Setting position goal
                    m_currentWaypointPosition = currentWaypoint.position;

                    currentWaypoint.SetEmpty();

                    if (m_currentPath.Count == 0)
                    {
                        Stop();
                    }
                    else
                    {
                        m_moveTimeCurrent = 0;
                        m_moveTimeTotal = m_walkSpeed;
                    }
                }
            }
        }
    }
}



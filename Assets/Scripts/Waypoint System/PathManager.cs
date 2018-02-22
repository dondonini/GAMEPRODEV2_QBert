using System;
using System.Collections;
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

        m_currentPath = new Stack<Waypoint>();
        Waypoint currentNode = m_mapManager.FindClosestWaypoint(transform.position);
        Waypoint endNode = m_mapManager.FindClosestWaypoint(destination);

        if (currentNode == null || endNode == null || currentNode == endNode)
            return;

        WaypointList openList = new WaypointList();
        List<Waypoint> closedList = new List<Waypoint>();

        openList.Add(0, currentNode);
        currentNode.Previous = null;
        currentNode.Distance = 0f;
        m_currentGoal = destination;

        while (openList.Count > 0)
        {
            currentNode = openList.GetWaypoints()[0];
            openList.RemoveAt(0);
            float dist = currentNode.Distance;
            closedList.Add(currentNode);

            if (currentNode == endNode)
                break;

            foreach (Waypoint neighbor in currentNode.m_neighbors)
            {
                if (closedList.Contains(neighbor) || openList.ContainsWaypoint(neighbor) || neighbor.IsOccupied() || neighbor.m_playerWaypointOnly)
                    continue;

                neighbor.Previous = currentNode;
                neighbor.Distance = dist + (neighbor.transform.position - currentNode.transform.position).magnitude;

                float distanceToTarget = (neighbor.transform.position - endNode.transform.position).magnitude;
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

                //transform.position = currentWaypoint.transform.position;
                m_currentWaypoint.SetOccupent(gameObject);
                transform.position = Vector3.Lerp(m_currentWaypointPosition, m_currentWaypoint.transform.position, m_moveTimeCurrent / m_moveTimeTotal);
            }
            else
            {
                // Saving previous waypoint
                m_prevWaypoint = m_currentPath.Peek();

                // Taking new waypoint
                m_currentWaypoint = m_currentPath.Pop();

                // Setting position goal
                m_currentWaypointPosition = m_currentWaypoint.transform.position;

                // Setting waypoint as occupied
                m_currentWaypoint.SetOccupent(gameObject);

                if (m_currentPath.Count == 0)
                {
                    Stop();
                }
                else
                {
                    m_prevWaypoint.SetEmpty();
                    m_prevWaypoint = m_currentWaypoint;
                    m_moveTimeCurrent = 0;
                    m_moveTimeTotal = m_walkSpeed;
                }
            }
        }
    }

    

}



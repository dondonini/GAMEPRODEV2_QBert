using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour {

    public float walkSpeed = 1.0f;
    public float m_searchTolerance = 2.0f;

    private Stack<Vector3> currentPath;
    private Vector3 currentWaypointPosition;
    private float moveTimeTotal;
    private float moveTimeCurrent;
    private Vector3 currentGoal = Vector3.zero;

    private MapManager mm;

    private void Start()
    {
        // Initializing variables
        mm = MapManager.Instance;
    }

    // Prep and start the AI path and movements
    public void NavigateTo(Vector3 destination)
    {
        
        if (currentGoal != destination && currentPath != null)
        {
            Stop();
        }
        else if (currentGoal == destination)
            return;

        currentPath = new Stack<Vector3>();
        Waypoint currentNode = mm.FindClosestWaypoint(transform.position);
        Waypoint endNode = mm.FindClosestWaypoint(destination);
        if (currentNode == null || endNode == null || currentNode == endNode)
            return;
        WaypointList openList = new WaypointList();
        List<Waypoint> closedList = new List<Waypoint>();
        openList.Add(0, currentNode);
        currentNode.previous = null;
        currentNode.distance = 0f;
        currentGoal = destination;

        while (openList.Count > 0)
        {
            currentNode = openList.GetWaypoints()[0];
            openList.RemoveAt(0);
            float dist = currentNode.distance;
            closedList.Add(currentNode);
            if (currentNode == endNode)
            {
                break;
            }
            foreach (Waypoint neighbor in currentNode.neighbors)
            {
                if (closedList.Contains(neighbor) || openList.ContainsWaypoint(neighbor))
                    continue;
                Debug.Log(currentNode);
                neighbor.previous = currentNode;
                neighbor.distance = dist + (neighbor.transform.position - currentNode.transform.position).magnitude;
                float distanceToTarget = (neighbor.transform.position - endNode.transform.position).magnitude;
                openList.Add(neighbor.distance + distanceToTarget, neighbor);
            }
        }

        if (currentNode == endNode)
        {
            while (currentNode.previous != null)
            {
                currentPath.Push(currentNode.transform.position);
                currentNode = currentNode.previous;
            }
            currentPath.Push(transform.position);
        }
    }

    // Stop the current path
    public void Stop()
    {
        currentPath = null;
        moveTimeTotal = 0;
        moveTimeCurrent = 0;
    }

    private void Update()
    {
        if (currentPath != null && currentPath.Count > 0)
        {
            if (moveTimeCurrent < moveTimeTotal)
            {
                moveTimeCurrent += Time.deltaTime;
                if (moveTimeCurrent > moveTimeTotal)
                    moveTimeCurrent = moveTimeTotal;
                transform.position = currentPath.Peek();
                //transform.position = Vector3.Lerp(currentWaypointPosition, currentPath.Peek(), moveTimeCurrent / moveTimeTotal);
            }
            else
            {
                currentWaypointPosition = currentPath.Pop();
                if (currentPath.Count == 0)
                {
                    //transform.position = currentPath.Peek();
                    Stop();
                }
                else
                {
                    moveTimeCurrent = 0;
                    moveTimeTotal = walkSpeed;
                }
            }
        }
    }

    

}



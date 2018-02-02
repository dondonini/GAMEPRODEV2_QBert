using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildWaypointMaps : MonoBehaviour
{
    public float m_maxDistance = 4.0f;

    private void Start()
    {
        foreach (GameObject waypoint in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            float dist = (waypoint.transform.position = target).magnitude;
            if (dist < closestDist)
            {
                closest = waypoint;
                closestDist = dist;
            }
        }
    }
    
	
}

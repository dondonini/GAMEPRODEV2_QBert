using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildWaypointWindow : EditorWindow {

    List<Transform> m_waypoints = new List<Transform>();
    float m_searchTolerance = 4.0f;

	public void Init()
    {
        foreach (GameObject waypoint in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            m_waypoints.Add(waypoint.transform);
        }
    }

    private void BuildMap()
    {
        foreach(Transform w in m_waypoints)
        {
            Waypoint current = w.GetComponent<Waypoint>();

            // Reset the array
            current.neighbors.Clear();

            // Searching for waypoints nearby
            foreach (Transform c in m_waypoints)
            {
                if (c != w)
                {
                    if (Vector3.Distance(w.position, c.position) < m_searchTolerance && w.position.y != c.position.y)
                    {
                        current.neighbors.Add(c.GetComponent<Waypoint>());
                    }
                }
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(" Waypoints Found: ");
        GUILayout.Label(m_waypoints.Count.ToString());
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(" Waypoint Connect Tolerance: ");
        m_searchTolerance = EditorGUILayout.FloatField(m_searchTolerance);
        GUILayout.EndHorizontal();


        if (GUILayout.Button("Build Map", GUILayout.Width(255)))
        {
            BuildMap();
        }
    }
}

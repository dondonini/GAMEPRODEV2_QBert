using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

    public List<Waypoint> neighbors;

	public Waypoint previous
    {
        get;
        set;
    }

    public float distance
    {
        get;
        set;
    }

    private void OnDrawGizmosSelected()
    {
        if (neighbors == null)
            return;
        Gizmos.color = new Color(0f, 1f, 0f);
        foreach(Waypoint neighbor in neighbors)
        {
            if (neighbor != null)
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }
}

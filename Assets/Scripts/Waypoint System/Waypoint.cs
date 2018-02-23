using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

    public List<Waypoint> m_neighbors;
    public bool m_playerWaypointOnly = false;

    [ReadOnly]
    [SerializeField]
    private GameObject m_occupent = null;

    // /////////////////
    #region Passthroughs
    // /////////////////

    public Vector3 position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }

    #endregion

    // //////////////////////
    #region Getters & Setters
    // //////////////////////

    public Waypoint Previous
    {
        get;
        set;
    }

    public float Distance
    {
        get;
        set;
    }

    public void SetOccupent(GameObject o)
    {
        m_occupent = o;
    }

    public GameObject GetOccupent()
    {
        return m_occupent;
    }

    public void SetEmpty()
    {
        m_occupent = null;
    }

    public Vector3 GetTopPosition()
    {
        return transform.position + new Vector3(0f, transform.localScale.y * 0.5f, 0f);
    }

    #endregion

    public bool IsOccupied()
    {
        return m_occupent != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_neighbors == null)
            return;

        Gizmos.color = new Color(0f, 1f, 0f);

        foreach(Waypoint neighbor in m_neighbors)
        {
            if (neighbor != null)
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }
}

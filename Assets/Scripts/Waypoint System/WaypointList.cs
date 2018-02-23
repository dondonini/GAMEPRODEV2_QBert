using System.Collections.Generic;
using UnityEngine;

public class WaypointList
{
    private List<WListValue> l;

    private int m_count = 0;

    // Constructors
    public WaypointList()
    {
        l = new List<WListValue>();
    }

    // Modifiers
    public void Add(float d, Waypoint w)
    {
        l.Add(new WListValue(d, w));
        Sort();
        m_count = l.Count;
    }

    public void RemoveAt(int i)
    {
        l.RemoveAt(i);
        Sort();
        m_count = l.Count;
    }

    public void Sort()
    {
        l.Sort((PosA, PosB) => PosA.d.CompareTo(PosB.d));
    }

    // ///////
    // Helpers
    // ///////

    public bool ContainsWaypoint(Waypoint w)
    {
        foreach(WListValue wList in l)
        {
            Waypoint current = wList.w;

            if (current == w)
                return true;
        }
        return false;
    }

    public Waypoint Pull()
    {
        Waypoint topWaypoint = l[0].w;
        RemoveAt(0);
        return topWaypoint;
    }

    // ///////
    // Getters
    // ///////

    public float[] GetDistances()
    {
        List<float> build = new List<float>();

        foreach(WListValue wList in l)
        {
            build.Add(wList.d);
        }

        return build.ToArray();
    }

    public Waypoint[] GetWaypoints()
    {
        List<Waypoint> build = new List<Waypoint>();

        foreach (WListValue wList in l)
        {
            build.Add(wList.w);
        }

        return build.ToArray();
    }

    public int Count
    {
        get
        {
            return m_count;
        }
    }

    
}

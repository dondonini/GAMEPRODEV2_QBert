using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadUp_In : BaseMapSpawn {

    private float duration = 2.0f;

    private Vector3 m_startPosOffset = new Vector3(0.0f, -100.0f, 0.0f);
    private List<List<Transform>> m_rows;
    private List<Vector3> m_startPos;
    private List<Vector3> m_endPos;
    private MapManager m_mapManager;

    public QuadUp_In()
    {
        m_mapManager = MapManager.Instance;
        m_rows = new List<List<Transform>>();
        m_endPos = new List<Vector3>();

        SaveEndPositions();
        SaveStartPositions();

        SortSegmentsByRows();
    }

    void SaveStartPositions()
    {
        GameObject[] segments = m_mapManager.GetAllGroundSegments();

        for (int i = 0; i < segments.Length; i++)
        {
            m_startPos.Add(segments[i].transform.position + m_startPosOffset);
        }
    }

    void SaveEndPositions()
    {
        GameObject[] segments = m_mapManager.GetAllGroundSegments();

        for (int i = 0; i < segments.Length; i++)
        {
            m_endPos.Add(segments[i].transform.position);
        }
    }

    private void SortSegmentsByRows()
    {
        foreach (GameObject segment in m_mapManager.GetAllGroundSegments())
        {
            Transform currentTransform = segment.transform;

            if (m_rows.Count == 0)
            {
                m_rows.Add(new List<Transform>());
                m_rows[0].Add(currentTransform);
                continue;
            }

            bool foundMatch = false;

            for (int i = 0; i < m_rows.Count; i++)
            {
                if (m_rows[i][0].position.y == currentTransform.position.y)
                {
                    m_rows[i].Add(currentTransform);
                    foundMatch = true;
                }
            }

            if (foundMatch)
                continue;

            m_rows.Add(new List<Transform>());
            m_rows[m_rows.Count - 1].Add(currentTransform);
        }
    }

    public void UpdateAnimation()
    {

    }

    public void SetMapToStartPosition()
    {
        GameObject[] segments = m_mapManager.GetAllGroundSegments();

        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].transform.position = m_startPos[i];
        }
    }

    public void SetMapToEndPosition()
    {
        GameObject[] segments = m_mapManager.GetAllGroundSegments();

        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].transform.position = m_endPos[i];
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTester : MonoBehaviour {

    public Transform m_destination;

    private Transform m_prevDest = null;

    private void Update()
    {
        if (m_destination != null && m_destination != m_prevDest)
        {
            Debug.Log("New destination set!");

            GetComponent<PathManager>().NavigateTo(m_destination.position);
        }

        m_prevDest = m_destination;
    }

}

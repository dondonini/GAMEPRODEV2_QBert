using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTester : MonoBehaviour {

    public Transform m_follow;

    private Vector3 m_destination;

    private Vector3 m_prevDest = Vector3.zero;

    private float timer = 0;

    private void Update()
    {
        m_destination = m_follow.position;

        if (timer >= 1)
        {

            if (m_destination != null && m_destination != m_prevDest)
            {
                Debug.Log("New destination set!");

                GetComponent<PathManager>().NavigateTo(m_destination);
            }

            m_prevDest = m_destination;
            timer = 0;
        }
        timer += Time.deltaTime;
    }

}

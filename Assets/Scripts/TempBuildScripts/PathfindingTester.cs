using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTester : MonoBehaviour {

    public Transform m_destination;

    private Transform m_prevDest = null;

    private float timer = 0;

    private void Update()
    {
        if (timer >= 1)
        {

            if (m_destination != null && m_destination != m_prevDest)
            {
                Debug.Log("New destination set!");

                GetComponent<PathManager>().NavigateTo(m_destination.position);
            }

            //m_prevDest = m_destination;
            timer = 0;
        }
        timer += Time.deltaTime;
    }

}

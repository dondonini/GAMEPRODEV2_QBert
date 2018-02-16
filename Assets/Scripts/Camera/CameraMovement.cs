using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {


    // /////////////////
    // Runtime Variables
    // /////////////////

    MapManager m_mapManager = null;
    Camera m_camera;
    Bounds m_mapBounds;

	// Use this for initialization
	void Start () {

        // Initializing Variables
        m_mapManager = MapManager.Instance;
        m_camera = Camera.main;
        m_mapBounds = new Bounds();
    }
	
	void FixedUpdate () {

        // Getting map bounds info when map is created
		if (m_mapManager.IsMapReady() && m_mapBounds.size == Vector3.zero)
        {
            m_mapBounds = m_mapManager.GetMapBounds();
        }

        if (m_mapBounds.size != Vector3.zero)
        {
            AlignCameraToMap();

        }
	}

    void AlignCameraToMap()
    {
        //TODO: Get camera script from that project you made for Keerit. Specifically the part where it finds the vertical FOV.
        float newXPos = m_mapBounds.center.x;
        float newYPos = 10.0f;
        float newZPos = 20.0f;

        m_camera.transform.position = new Vector3(newXPos, newYPos, newZPos);
    }
}

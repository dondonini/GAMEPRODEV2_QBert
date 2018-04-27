using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraMovement : MonoBehaviour {

    // /////////////////////////////
    [Header("Assignable Variables")]
    // /////////////////////////////

    [Tooltip("The offset distance from the center of the map to the player")]
    [Range(0.0f, 1.0f)]
    public float m_pullStrength = 0.8f;

    public float m_smoothTime = 0.124f;

    [Tooltip("Angle of the camera to the map")]
    [Range(0.1f, 90.0f)]
    public float m_cameraAngle = 30.0f;

    [SerializeField]
    private bool m_debug = false;

    // /////////////////////////////
    [Header("References")]
    // /////////////////////////////

    [SerializeField]
    private Transform m_lookAtTransform;

    [SerializeField]
    private Transform m_player;

    // /////////////////
    // Runtime Variables
    // /////////////////

    const float M_CAMERA_DISTANCE_FROM_MAP = 50.0f;
    MapManager m_mapManager = null;
    Camera m_camera;
    Bounds m_mapBounds;
    Vector3 m_velocity;

	// Use this for initialization
	void Start () {

        // Initializing Variables
        m_mapManager = MapManager.instance;
        m_camera = Camera.main;
        m_mapBounds = new Bounds();
        m_velocity = Vector3.zero;

        // Stops the script from running if there's no LookAt Transform. Just for safe measure
        if (m_lookAtTransform == null)
        {
            Debug.LogError("LookAt Transform is not assigned! Disabling script...");
            enabled = false;
        }

        m_lookAtTransform.position = Vector3.zero;
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

            SmoothPoint();
        }
	}

    /// <summary>
    /// Positions camera to be aligned with the map at a certain angle
    /// </summary>
    void AlignCameraToMap()
    {
        float newXPos = m_mapBounds.center.x;
        float newYPos = (m_mapBounds.center.y) + Mathf.Sin(m_cameraAngle * Mathf.Deg2Rad) * M_CAMERA_DISTANCE_FROM_MAP;
        float newZPos = (m_mapBounds.center.z) + Mathf.Cos(m_cameraAngle * Mathf.Deg2Rad) * M_CAMERA_DISTANCE_FROM_MAP;

        m_camera.transform.position = new Vector3(newXPos, newYPos, newZPos);
    }

    /// <summary>
    /// Smoothly aims at the target based on offset
    /// </summary>
    void SmoothPoint()
    {
        Vector3 offsetLine = Vector3.LerpUnclamped(m_mapBounds.center, m_player.position, m_pullStrength);

        m_lookAtTransform.position = Vector3.SmoothDamp(m_lookAtTransform.position, offsetLine, ref m_velocity, m_smoothTime);

        m_camera.transform.LookAt(m_lookAtTransform.position);
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (m_debug && EditorApplication.isPlaying)
        {
            // Full line
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(m_mapBounds.center, m_player.position);

            // Offset line
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Vector3.LerpUnclamped(m_mapBounds.center, m_player.position, m_pullStrength), 0.05f);

            // Target
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(m_player.position, 0.05f);

            // Camera Aim
            Gizmos.color = Color.green;
            Gizmos.DrawLine(m_camera.transform.position, m_lookAtTransform.position);

            // Point to target
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(m_lookAtTransform.position, Vector3.LerpUnclamped(m_mapBounds.center, m_player.position, m_pullStrength));
        }
    }

#endif
}

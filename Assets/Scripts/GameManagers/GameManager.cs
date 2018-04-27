using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    private MapManager m_mapManager;
    private GameInfo m_gameInfo;

	// Use this for initialization
	void Start () {

        if (instance != this)
        {
            Destroy(this);
        }

        if (!instance)
        {
            instance = this;
        }

        // Initialization
        Initialize();
    }
	
    void Initialize()
    {
        // Collecting global scripts
        m_mapManager = MapManager.instance;
        m_gameInfo = GameInfo.instance;
    }

    // //////////////////////
    #region Getters & Setters
    // //////////////////////

    #endregion

    // ////////////////////
    #region SetBlock Method
    // ////////////////////

    public void SetBlock(Waypoint _waypoint, bool _activate = false)
    {
        GameObject mapPart = m_mapManager.GetMapPartFromWaypoint(_waypoint);

        if (mapPart)
        {
            SetBlock(mapPart, _activate);
        }
    }

    public void SetBlock(GameObject _part, bool _activate = false)
    {
        BlockInfo partBlockInfo = _part.GetComponent<BlockInfo>();

        if (_activate)
        {
            partBlockInfo.IsActivated = true;
            
        }
        else
        {
            partBlockInfo.IsActivated = false;
            
            
        }
        partBlockInfo.UpdateVisual();
    }

    #endregion
}

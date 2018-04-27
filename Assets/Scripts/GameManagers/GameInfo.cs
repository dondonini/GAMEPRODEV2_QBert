using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour {

    public static GameInfo instance = null;

    public Material m_mapBaseColor;
    public Material m_mapChangeColor;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        Init();
    }

    void Init()
    {

    }
}

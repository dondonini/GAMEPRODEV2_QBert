using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour {

    public static GameInfo instance = null;

    [SerializeField, ReadOnly]
    private int m_score;

    [Header("Materials")]
    public Material m_mapBaseColor;
    public Material m_mapChangeColor;

    [Header("Attachments")]
    public ScoringUI m_scoringUI;

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

    // Edit the score
    public int Score
    {
        get
        {
            return m_score;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            m_score = value;

            m_scoringUI.UpdateP1Score(m_score);
        }
    }
}

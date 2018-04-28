using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringUI : MonoBehaviour {

    public int m_player1Lives = 3;

    [Header("Player 1 Scoring")]
    public Text m_p1Scoring;
    public Image m_p1Life0;
    public Image m_p1Life1;
    public Image m_p1Life2;
    

    // Update Player 1's score
    public void UpdateP1Score(int _newScore)
    {
        m_p1Scoring.text = _newScore.ToString();
    }

    public void UpdateP1Life(int _newLives)
    {
        _newLives = Mathf.Clamp(_newLives, 0, 3);

        if (_newLives == 0)
        {
            m_p1Life0.enabled = false;
            m_p1Life1.enabled = false;
            m_p1Life2.enabled = false;
        }
        else if (_newLives == 1)
        {
            m_p1Life0.enabled = false;
            m_p1Life1.enabled = false;
            m_p1Life2.enabled = true;
        }
        else if (_newLives == 2)
        {
            m_p1Life0.enabled = false;
            m_p1Life1.enabled = true;
            m_p1Life2.enabled = true;
        }
        else if (_newLives == 3)
        {
            m_p1Life0.enabled = true;
            m_p1Life1.enabled = true;
            m_p1Life2.enabled = true;
        }
    }
}

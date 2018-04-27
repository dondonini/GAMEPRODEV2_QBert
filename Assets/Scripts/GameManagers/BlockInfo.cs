using UnityEngine;

public class BlockInfo : MonoBehaviour {

    [SerializeField, ReadOnly]
    private bool m_isActive = false;
    private Renderer m_renderer;

    private GameInfo m_gameInfo;

    private void Start()
    {
        // Collecting global scripts
        m_gameInfo = GameInfo.instance;

        m_renderer = GetComponent<Renderer>();
    }

    public bool IsActivated
    {
        get
        {
            return m_isActive;
        }
        set
        {
            m_isActive = value;
        }
    }

    public void UpdateVisual()
    {
        if (m_isActive)
        {
            m_renderer.material = m_gameInfo.m_mapChangeColor;
        }
        else
        {
            m_renderer.material = m_gameInfo.m_mapBaseColor;
        }
    }
}

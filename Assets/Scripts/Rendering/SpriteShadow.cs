using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SpriteShadow : MonoBehaviour {

    [SerializeField]
    private bool m_castShadows = false;
    [SerializeField]
    private bool m_recieveShadows = false;

    Renderer r;

	// Use this for initialization
	void OnValidate()
    {
        r = GetComponent<Renderer>();

        UpdateRenderer();

    }

    void UpdateRenderer()
    {
        if (m_castShadows)
        {
            r.shadowCastingMode = ShadowCastingMode.TwoSided;
        }
        else
        {
            r.shadowCastingMode = ShadowCastingMode.Off;
        }

        r.receiveShadows = m_recieveShadows;
    }
}

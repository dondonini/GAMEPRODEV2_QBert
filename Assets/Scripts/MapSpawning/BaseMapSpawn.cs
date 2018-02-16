using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMapSpawn : MonoBehaviour {

    public I_MapSpawner currentAnimation;

    private I_MapSpawner previousAnimation;

    private void Start()
    {
        currentAnimation = null;
    }

    private void Update()
    {
        currentAnimation.UpdateAnimation();

        if (previousAnimation != null)
        {
            if (previousAnimation != currentAnimation)
            {
                Debug.Log("Map animation set: " + currentAnimation);
            }
        }
        else
        {
            Debug.Log("Map animation is now idle.");
        }

        previousAnimation = currentAnimation;
    }
}

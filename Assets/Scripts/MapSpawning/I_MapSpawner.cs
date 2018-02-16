using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_MapSpawner {

    void UpdateAnimation();

    float GetAnimationPercentage();

    void SetMapToStartPosition();

    void SetMapToEndPosition();

}

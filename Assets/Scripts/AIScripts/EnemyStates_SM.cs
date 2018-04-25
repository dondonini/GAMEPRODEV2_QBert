using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyStates_SM {

    // Updates the state
    void UpdateState();

    // State when entering map
    void ToEnterState();

    // State when following player
    void ToFollowState();

    // State when leaving map
    void ToExitState();
}

public interface IEnemyStates_SM {

    // Updates the state
    void UpdateState();

    // State when entering map
    void ToEnterState();

    // State when chasing the player
    void ToChaseState();

    // State to reach the bottom (Applicable to Coily only)
    void ToReachBottomState();

    // State when leaving map
    void ToExitState();
}

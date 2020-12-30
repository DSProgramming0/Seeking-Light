using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    //TRACKS PLAYERS STATES
    public static PlayerStates instance;

    public PlayerInteractionStates currentPlayerInteractionState;
    public PushOrPullStates currentPushOrPullState;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentPlayerInteractionState = PlayerInteractionStates.NOTINTERACTING;
        currentPushOrPullState = PushOrPullStates.NULL;
    }

    void Update()
    {
        if(currentPlayerInteractionState == PlayerInteractionStates.NOTINTERACTING)
        {
            currentPushOrPullState = PushOrPullStates.NULL;
        }
    }

}

public enum PlayerInteractionStates
{
    INTERACTING,
    NOTINTERACTING
}

public enum PushOrPullStates
{
    NULL,
    PUSHING,
    PULLING
}

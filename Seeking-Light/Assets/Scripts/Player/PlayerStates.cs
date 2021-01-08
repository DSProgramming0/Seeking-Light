using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    //TRACKS PLAYERS STATES
    public static PlayerStates instance;

    [SerializeField] private AnimHook thisAnim;

    public PlayerConverstaionStates currentConverstaionState;
    public PlayerFlashlightStates currentPlayerFlashlightState;
    public PlayerConditionStates currentPlayerConditionState;
    public PlayerInteractionStates currentPlayerInteractionState;
    public PushOrPullStates currentPushOrPullState;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentConverstaionState = PlayerConverstaionStates.NOT_IN_CONVERSATION;
        currentPlayerFlashlightState = PlayerFlashlightStates.FLASHLIGHT_OFF;
        currentPlayerConditionState = PlayerConditionStates.ALIVE;
        currentPlayerInteractionState = PlayerInteractionStates.NOTINTERACTING;
        currentPushOrPullState = PushOrPullStates.NULL;
    }

    void Update()
    {
        if(currentPlayerInteractionState == PlayerInteractionStates.NOTINTERACTING)
        {
            currentPushOrPullState = PushOrPullStates.NULL;
        }

        if(currentPlayerConditionState == PlayerConditionStates.ALIVE) //If player is alive, they can control the character
        {
            if(currentConverstaionState == PlayerConverstaionStates.NOT_IN_CONVERSATION)
            {
                PlayerInfo.instance.PlayerHasControl = true;
            }
            else
            {
                PlayerInfo.instance.PlayerHasControl = false;
            }
        }
        else //If the player is not alive, they cannot
        {
            PlayerInfo.instance.PlayerHasControl = false;
            currentPlayerFlashlightState = PlayerFlashlightStates.FLASHLIGHT_OFF;
            currentPlayerInteractionState = PlayerInteractionStates.NOTINTERACTING;
        }
    }

}

public enum PlayerConverstaionStates
{
    IN_CONVERSATION,
    NOT_IN_CONVERSATION
}

public enum PlayerFlashlightStates
{
    FLASHLIGHT_ON,
    FLASHLIGHT_OFF
}

public enum PlayerConditionStates
{
    ALIVE,
    DEAD
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

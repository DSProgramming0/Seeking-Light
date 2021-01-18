using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private ThisInteractionIs thisInteraction;

    #region DoorPuzzle
    [SerializeField] private UITweener doorToOpen; 
    #endregion

    public void startInteraction()
    {
        switch (thisInteraction)
        {
            case ThisInteractionIs.DIALOGUE:
                Debug.Log("Dialogue started");
                break;
            case ThisInteractionIs.SWITCH:
                Debug.Log("switch pressed");
                doorToOpen.toggleDoor();
                break;
            case ThisInteractionIs.CAGE:
                Debug.Log("switch pressed");
                GameEvents.instance.LightRelease();
                break;
            default:
                Debug.LogError("Interaction state not set!!");
                break;
        }
    }  
}

public enum ThisInteractionIs
{
    DIALOGUE,
    SWITCH,
    CAGE
}


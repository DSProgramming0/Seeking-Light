using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interact : MonoBehaviour
{
    [SerializeField] private ThisInteractionIs thisInteraction;

    private bool interacted = false;

    #region DoorPuzzle
    [SerializeField] private UITweener doorToOpen;
    #endregion    

    public bool Interacted
    {
        get { return interacted; }
        set { interacted = value; }
    }

    public void startInteraction()
    {
        if(interacted == false)
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

            interacted = true;
        }        
    }  
}

public enum ThisInteractionIs
{
    DIALOGUE,
    SWITCH,
    CAGE
}


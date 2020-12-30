using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private ThisInteractionIs thisInteraction;

    public void startInteraction()
    {
        switch (thisInteraction)
        {
            case ThisInteractionIs.DIALOGUE:
                Debug.Log("Dialogue started");
                break;
            case ThisInteractionIs.SWITCH:
                Debug.Log("Switch used");
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
    SWITCH
}


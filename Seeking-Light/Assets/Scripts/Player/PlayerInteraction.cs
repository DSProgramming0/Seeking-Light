﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private AnimHook _animHook;
    private bool interactionTriggered = false;
    [SerializeField] private BoxCollider2D m_disableCollider;

    [SerializeField] private Transform rayStartPoint;
    [SerializeField] private float rayDist = 1f;

    [SerializeField] private GameObject moveableObj; //The current moveableObject that the player has selected
    [SerializeField] private Interact interactObj;
    private Vector2 dir;

    [Header("Player Flashlight")]
    private bool playerHasFlashlight = false;
    [SerializeField] private bool flashlightOn = false;
    [SerializeField] private Light2D flashlight;
    [SerializeField] private SpriteRenderer flashLightSprite;

    void Update()
    {
        dir = PlayerInfo.instance.Dir;

        Physics2D.queriesStartInColliders = false; //Stops rayCast hitting colliders on palyer (If this has errors, switch to layerMask)
        RaycastHit2D hit = Physics2D.Raycast(rayStartPoint.position, Vector2.right * transform.localScale.x, rayDist);

        if(hit.collider != null && hit.collider.tag == "MoveableObj" && Input.GetButton("Interact")) //If we have hit something & it has the MoveableObjTag && the player is holding down E
        {
            moveableObj = hit.collider.gameObject; //Set the obj

            moveableObj.GetComponent<Rigidbody2D>().gravityScale = 2f; //Change the Rigidbody gravityScale
            moveableObj.GetComponent<Rigidbody2D>().mass = 5f; 
            moveableObj.GetComponent<FixedJoint2D>().enabled = true; //Enable and set the fixed joint target to the players Rigidbody
            moveableObj.GetComponent<FixedJoint2D>().connectedBody = PlayerInfo.instance.returnRB();
            PlayerStates.instance.currentPlayerInteractionState = PlayerInteractionStates.INTERACTING; //Set the correct interactions tate

            //Determines whether the player is pushing or pulling an object
            #region Pushing Or Pulling logic
            if (PlayerInfo.instance.FacingRight != true && dir.x == -1)
            {
                PlayerStates.instance.currentPushOrPullState = PushOrPullStates.PUSHING;
            }
            else if(PlayerInfo.instance.FacingRight != true && dir.x == 1)
            {
                PlayerStates.instance.currentPushOrPullState = PushOrPullStates.PULLING;
            }

            if (PlayerInfo.instance.FacingRight == true && dir.x == 1)
            {
                PlayerStates.instance.currentPushOrPullState = PushOrPullStates.PUSHING;
            }
            else if (PlayerInfo.instance.FacingRight == true && dir.x != 1)
            {
                PlayerStates.instance.currentPushOrPullState = PushOrPullStates.PULLING;
            }

            if (PlayerInfo.instance.FacingRight == true && dir.x == 0)
            {
                PlayerStates.instance.currentPushOrPullState = PushOrPullStates.NULL;
            }
            else if (PlayerInfo.instance.FacingRight != true && dir.x == 0)
            {
                PlayerStates.instance.currentPushOrPullState = PushOrPullStates.NULL;
            }

            //ANIMATIONS
            if (PlayerStates.instance.currentPlayerInteractionState == PlayerInteractionStates.INTERACTING)
            {
                if(interactionTriggered == false)
                {
                    _animHook.setInteractionTrigger();
                    interactionTriggered = true;

                    if (m_disableCollider != null)
                        m_disableCollider.isTrigger = true;
                }

                if (PlayerStates.instance.currentPushOrPullState == PushOrPullStates.NULL)
                {
                    _animHook.setInteractionToIdle();
                }

                if (PlayerStates.instance.currentPushOrPullState == PushOrPullStates.PUSHING)
                {
                    _animHook.togglePushOrPullState(true);
                }

                if (PlayerStates.instance.currentPushOrPullState == PushOrPullStates.PULLING)
                {
                    _animHook.togglePushOrPullState(false);
                }              
            }           

            #endregion

        }
        else if (Input.GetButtonUp("Interact"))
        {   //If the player lets go of E WHILE interacting with an object
            if(hit.collider != null && hit.collider.tag == "MoveableObj")
            {
                moveableObj.GetComponent<Rigidbody2D>().gravityScale = 10f;  //Reset components and interaction state
                moveableObj.GetComponent<Rigidbody2D>().mass = 30f;  
                moveableObj.GetComponent<FixedJoint2D>().enabled = false;
                moveableObj.GetComponent<FixedJoint2D>().connectedBody = null;
                PlayerStates.instance.currentPlayerInteractionState = PlayerInteractionStates.NOTINTERACTING;

                if (m_disableCollider != null)
                    m_disableCollider.isTrigger = false;

                interactionTriggered = false;
                _animHook.isInteracting();


                moveableObj = null;
            }                      
        }

        if( hit.collider != null && hit.collider.tag == "InteractableObj") //If player is hitting an object with the interactableObj tag and presses E
        {
            interactObj = hit.collider.GetComponent<Interact>();

            if(interactObj.Interacted == false)
            {
                PromptManager.instance.spawnPrompt(GameAssets.instance.interactKeyPrompt, interactObj.transform.position, new Vector2(0, 10));

                if (Input.GetButtonDown("Interact"))
                {
                    Debug.Log("Interacting");
                    interactObj.startInteraction(); //Call the interaction method on whatever type of interactable object it is
                    PromptManager.instance.destroyPrompt();
                }
            }
            else //If this interaction has already been used
            {
                PromptManager.instance.destroyPrompt();
            }
        }
        else //If the player is no longer looking at object
        {
            if(PromptManager.instance.SpawnedPrompt != null)
            {
                PromptManager.instance.destroyPrompt();
            }
        }

        if (PlayerInfo.instance.PlayerHasFlashLight == true)
        {
            if (Input.GetButtonDown("ToggleFlashlight")) 
            {
                toggleFlashLight();
            }
        }       
    }  

    public void resetFlashlightOnRespawn()
    {
        flashlightOn = false;
        flashlight.enabled = false;
        flashLightSprite.enabled = false;
        _animHook.setPlayerFlashlight(flashlightOn);
    }

    void toggleFlashLight()
    {
        if(flashlightOn == false)
        {
            PlayerStates.instance.currentPlayerFlashlightState = PlayerFlashlightStates.FLASHLIGHT_ON;
            flashlight.enabled = true;
            flashLightSprite.enabled = true;

            flashlightOn = true;
            _animHook.setPlayerFlashlight(flashlightOn);
        }
        else
        {
            PlayerStates.instance.currentPlayerFlashlightState = PlayerFlashlightStates.FLASHLIGHT_OFF;
            flashlight.enabled = false;
            flashLightSprite.enabled = false;
            flashlightOn = false;
            _animHook.setPlayerFlashlight(flashlightOn);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(rayStartPoint.position, (Vector2)rayStartPoint.position + Vector2.right * rayDist);
    }
}

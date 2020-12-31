using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private AnimHook _animHook;

    [SerializeField] private Transform rayStartPoint;
    [SerializeField] private float rayDist = 1f;

    [SerializeField] private GameObject moveableObj; //The current moveableObject that the player has selected
    private Vector2 dir;

    [Header("Player Flashlight")]
    private bool playerHasFlashlight = false;
    [SerializeField] private bool flashlightOn = false;
    [SerializeField] private Light2D flashlight;

    void Update()
    {
        dir = PlayerInfo.instance.Dir;

        Physics2D.queriesStartInColliders = false; //Stops rayCast hitting colliders on palyer (If this has errors, switch to layerMask)
        RaycastHit2D hit = Physics2D.Raycast(rayStartPoint.position, Vector2.right * transform.localScale.x, rayDist);

        if(hit.collider != null && hit.collider.tag == "MoveableObj" && Input.GetKey(KeyCode.E)) //If we have hit something & it has the MoveableObjTag && the player is holding down E
        {
            moveableObj = hit.collider.gameObject; //Set the obj

            moveableObj.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic; //Change the RigidbodyTyPE
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
            #endregion
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {   //If the player lets go of E WHILE interacting with an object
            if(hit.collider != null && hit.collider.tag == "MoveableObj")
            {
                moveableObj.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; //Reset components and interaction state
                moveableObj.GetComponent<FixedJoint2D>().enabled = false;
                moveableObj.GetComponent<FixedJoint2D>().connectedBody = null;
                PlayerStates.instance.currentPlayerInteractionState = PlayerInteractionStates.NOTINTERACTING;


                moveableObj = null;
            }                      
        }

        if( hit.collider != null && hit.collider.tag == "InteractableObj" && Input.GetKeyDown(KeyCode.E)) //If player is hitting an object with the interactableObj tag and presses E
        {
            hit.collider.GetComponent<Interact>().startInteraction(); //Call the interaction method on whatever type of interactable object it is
        }

        if(PlayerInfo.instance.PlayerHasFlashLight == true)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                toggleFlashLight();
            }
        }       
    }  

    void toggleFlashLight()
    {
        if(flashlightOn == false)
        {
            flashlight.enabled = true;
            flashlightOn = true;
            _animHook.setPlayerFlashlight(flashlightOn);
        }
        else
        {
            flashlight.enabled = false;
            flashlightOn = false;
            _animHook.setPlayerFlashlight(flashlightOn);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(rayStartPoint.position, (Vector2)rayStartPoint.position + Vector2.right * transform.localScale.x * rayDist);
    }
}

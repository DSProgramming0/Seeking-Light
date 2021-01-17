using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CompanionControl : MonoBehaviour
{
    //place this script on the player gameobject

    public GameObject player; // in the inspector drag the gameobject the will be following the player to this field

    [Header("Movement")]
    [SerializeField] private Transform companionResetPos;
    [SerializeField] private float speed;
    [SerializeField] private float originalSpeed;
    [SerializeField] private float speedRegain;
    [SerializeField] private int followDistance;
    [SerializeField] private float maxControlDistance;
    private List<Vector3> storedPositions;
    [SerializeField] private Vector3 offset;

    [SerializeField] private StalkerEnemy stalkerEnemy;
    [SerializeField] private float stalkerDistThreshold;

    [Header("Focusing")]
    [SerializeField] private FocusObject currentFocusObj;
    [SerializeField] private float focusMeter = 0f;
    [SerializeField] private Image focusMeterSprite;

    void Awake()
    {
        storedPositions = new List<Vector3>(); //create a blank list
        speed = originalSpeed;

        if (!player)
        {
            Debug.Log("The FollowingMe gameobject was not set");
        }

        if (followDistance == 0)
        {
            Debug.Log("Please set distance higher then 0");
        }
    }

    void Update()
    {
        float distToStalker = Vector2.Distance(transform.position, stalkerEnemy.transform.position);
        if(distToStalker <= stalkerDistThreshold)
        {
            stalkerEnemy.PlayerIsHoldingBack = true;
        }
        else
        {
            stalkerEnemy.PlayerIsHoldingBack = false;
        }

        focusMeter = Mathf.Clamp(focusMeter, 0, 100);

        Movement();

        focus();

        if (Input.GetKeyDown(KeyCode.N))
        {
            switchControl();
        }
    }

    private void Movement()
    {
        if (PlayerStates.instance.currentCompanionControlState == CompanionControlStates.PLAYER_NO_CONTROL)
        {
            if (storedPositions.Count == 0)
            {
                Debug.Log("blank list");
                storedPositions.Add(player.transform.position + offset); //store the players currect position
                return;
            }
            else if (storedPositions[storedPositions.Count - 1] != player.transform.position + offset)
            {
                //Debug.Log("Add to list");
                storedPositions.Add(player.transform.position + offset); //store the position every frame
            }

            if (storedPositions.Count > followDistance)
            {
                
                transform.position = Vector3.Lerp(transform.position, storedPositions[0], speed * Time.deltaTime / speedRegain); //move
                storedPositions.RemoveAt(0); //delete the position that player just move to
            }
        }
        else
        {
            float distance = Vector2.Distance(transform.position, player.transform.position); //Stores distances from player and this/Mouse pos

            if (distance >= maxControlDistance)
            {
                transform.position = Vector3.Lerp(transform.position, companionResetPos.position, speed * Time.deltaTime / speedRegain); //Stops companion moving beyond screen or too far from palyer 
                                                                                                                                         //Also means player doesnt have to catch up to companion to control it
            }
            else
            {
                speed = Mathf.Lerp(speed, originalSpeed, 1.5f * Time.deltaTime / speedRegain); 
            }

            transform.position = Vector3.Lerp(transform.position, MouseHandler.instance.MousePos, speed * Time.deltaTime); //Moves companion Pos to mousePos
        }
    }

    private void focus()
    {
        if(PlayerStates.instance.currentCompanionControlState == CompanionControlStates.PLAYER_HAS_CONTROL)
        {          
            if(currentFocusObj != null)
            {
                if(currentFocusObj.CanFocus == true && currentFocusObj.ShouldReveal == false)
                {
                    if(PlayerStates.instance.currentPlayerInteractionState == PlayerInteractionStates.NOTINTERACTING)
                    {
                        focusMeterSprite.enabled = true;
                        if (Input.GetButton("Focus")) //If focus button is held down
                        {
                            focusMeter += 1f;  //Starts filling up focus meter
                            focusMeterSprite.fillAmount = focusMeter / 100;
                        }
                        else
                        {
                            focusMeter = Mathf.Lerp(focusMeter, 0, 1f * Time.deltaTime); //starts decreasing focus meter fill
                            focusMeterSprite.fillAmount = focusMeter / 100;
                        }

                        if (focusMeter >= 100) //If focus meter full
                        { 
                            currentFocusObj.ShouldReveal = true; //Secret will reveal
                        }
                    }
                    else
                    {
                        focusMeterSprite.enabled = false;
                        focusMeter = 0;
                    }
                }
                else
                {
                    focusMeterSprite.enabled = false;
                    focusMeter = 0;
                }                
            }
            else
            {
                focusMeterSprite.enabled = false;
                focusMeter = 0;
            }
        }
    } 

    public void switchControl() //Controls the state of the companion, Whether the player has control or not
    {
        if (PlayerStates.instance.currentCompanionControlState == CompanionControlStates.PLAYER_HAS_CONTROL)  
        {
            PlayerStates.instance.currentCompanionControlState = CompanionControlStates.PLAYER_NO_CONTROL;
        }
        else
        {
            PlayerStates.instance.currentCompanionControlState = CompanionControlStates.PLAYER_HAS_CONTROL;

        }
    }

    public void setCurrentFocusObject(FocusObject _thisObj) //Sets the current focus object the the companion is over
    {
        currentFocusObj = _thisObj;
    }

    public void clearCurrentFocusObject() //Clears current focus object
    {
        currentFocusObj = null;
    }
}

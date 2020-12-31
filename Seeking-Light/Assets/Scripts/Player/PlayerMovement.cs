using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private AnimHook thisAnim;

    private float horizontalMove = 0f;
    [SerializeField] private Vector2 dir;

    [SerializeField] private float currentSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;

    [SerializeField] private float sprintSpeedGain = 20f;
    [SerializeField] private float walkSpeedGain = 20f;
    [SerializeField] private float idleSpeedDrop = 25;

    private bool jump = false;
    private bool crouch = false;
        

    void Update()
    {

       
    }

    private void MovementInput()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        dir = new Vector2(horizontalMove, 0);
        PlayerInfo.instance.Dir = dir;
        horizontalMove = horizontalMove * currentSpeed;

        if (Input.GetButton("Sprint")) //If player holds the sprint buttong, increase speed to the run speed amount
        {
            float newSpeed = Mathf.Lerp(currentSpeed, runSpeed, sprintSpeedGain * Time.deltaTime);
            currentSpeed = newSpeed;
        }
        else if (!Input.GetButton("Sprint") && dir.magnitude != 0) //decrease or increase speed to walk speed amount over time.
        {
            float newSpeed = Mathf.Lerp(currentSpeed, walkSpeed, walkSpeedGain * Time.deltaTime);
            currentSpeed = newSpeed;
        }

        if (dir.magnitude == 0) //If palyer is still, drop to idle speed
        {
            float newSpeed = Mathf.Lerp(currentSpeed, 0, idleSpeedDrop * Time.deltaTime);
            currentSpeed = newSpeed;

            if (currentSpeed <= 0)
            {
                currentSpeed = 0;
            }
        }

        thisAnim.setPlayerSpeed(currentSpeed); //Accesses animator and sets correct values for parameters

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }

}

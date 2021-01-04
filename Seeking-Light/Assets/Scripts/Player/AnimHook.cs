using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimHook : MonoBehaviour
{
    //Accesses Animator linked to the gameObject it has been added to

    [SerializeField] private Animator thisAnimator;

    public void setPlayerSpeed(float _speed)
    {
        thisAnimator.SetFloat("Speed", _speed);
    }

    public void setDoorState(bool _bool)
    {
        thisAnimator.SetBool("DoorState", _bool);
    }

    public void setPlayerFlashlight(bool _flashlightOn)
    {
        thisAnimator.SetBool("FlashlightActive", _flashlightOn);
    }

    public void setGroundBool(bool _isGrounded)
    {
        thisAnimator.SetBool("Grounded", _isGrounded);
    }

    public void setTakeOffTrigger()
    {
        thisAnimator.SetTrigger("TakeOff");
    }

    public void setJumpingState(bool _isJumping)
    {
        thisAnimator.SetBool("isJumping", _isJumping);
    }

    public void setInteractionTrigger()
    {
        thisAnimator.SetTrigger("Interacting");
    }

    public void isInteracting()
    {
        thisAnimator.SetTrigger("stoppedInteracting");
        thisAnimator.SetBool("InteractionIdle", false);
    }

    public void resetPose(bool _shouldReset)
    {
        Debug.Log("Called");
        thisAnimator.SetBool("resetPose", _shouldReset);
    }

    public void togglePushOrPullState(bool _isPushing)
    {
        if (_isPushing)
        {
            thisAnimator.SetBool("Pushing", true);
            thisAnimator.SetBool("Pulling", false);

            thisAnimator.SetBool("InteractionIdle", false);
        }
        else
        {
            thisAnimator.SetBool("Pushing", false);
            thisAnimator.SetBool("Pulling", true);

            thisAnimator.SetBool("InteractionIdle", false);
        }
    }

    public void setInteractionToIdle()
    {
        thisAnimator.SetBool("InteractionIdle", true);

        thisAnimator.SetBool("Pushing", false);
        thisAnimator.SetBool("Pulling", false);

    }
}

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
}

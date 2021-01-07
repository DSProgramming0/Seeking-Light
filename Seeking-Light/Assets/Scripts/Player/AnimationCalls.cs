using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCalls : MonoBehaviour
{
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private Animator thisAnim;

    public void callClimbEnd()
    {
        controller.FinishLedgeClimb();
    }


}

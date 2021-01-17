using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCalls : MonoBehaviour
{
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private Animator thisAnim;

    private int footstepIndex = 0;

    public void callClimbEnd()
    {
        controller.FinishLedgeClimb();
    }

    public void playFootsteps()
    {
        if(footstepIndex == 0)
        {
            SoundManager.Play2DSound(SoundManager.Sound.PlayerFootstep_1_MUD, 1.5f, .025f);
            footstepIndex++;
        }
        else
        {
            SoundManager.Play2DSound(SoundManager.Sound.PlayerFootstep_2_MUD, 1.5f, .025f);
            footstepIndex--;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    //Called on event animations
    public void playFootstep()
    {
        SoundManager.Play2DSound(SoundManager.Sound.PlayerFootstep, 1f, .05f);
    }
}

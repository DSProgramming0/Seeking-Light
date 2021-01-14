using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private Animator cameraControl;
    [SerializeField] private ParticleSystemManager _particles;
    [SerializeField] private int currentParticleEffect = 0;

  

    public int cameraID;   

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            passCameraID();

            switch (currentParticleEffect)
            {
                case 0:
                    _particles.currentParticleEmitting = ParticleEmittingState.NONE;
                    break;
                case 1:
                    _particles.currentParticleEmitting = ParticleEmittingState.FALLING_LEAVES;
                    break;
                case 2:
                    _particles.currentParticleEmitting = ParticleEmittingState.RAINING;
                    break;
                default:
                    _particles.currentParticleEmitting = ParticleEmittingState.NONE;
                    break;
            }
        }
    }    

    private void passCameraID()
    {
        //UIManager.instance.fadeout();
        cameraControl.SetInteger("CameraID", cameraID);      
    }
}

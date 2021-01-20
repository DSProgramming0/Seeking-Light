using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private CameraEffectsController vCamEffects;
    [SerializeField] private Animator cameraControl;
    [SerializeField] private ParticleSystemManager _particles;
    [SerializeField] private int currentParticleEffect = 0;

    private CinemachineVirtualCamera thisVcam;
    private CinemachineBasicMultiChannelPerlin thisVcamEffects;

    void Awake()
    {
        vCamEffects = GetComponentInParent<CameraEffectsController>();
        thisVcam = GetComponent<CinemachineVirtualCamera>();
        thisVcamEffects = thisVcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

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
        vCamEffects.setCurrentCam(thisVcam, thisVcamEffects);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraEffectsController : MonoBehaviour
{
    private CinemachineVirtualCamera currentVcam;
    private CinemachineBasicMultiChannelPerlin currentVCamNoise;

    [SerializeField] private NoiseSettings noiseToSwitchTo; //Stores the 6D shake setting which will be switched to
    private NoiseSettings originalNoise; //Stores the original multi channel perlin profile
    
    public void setCurrentCam(CinemachineVirtualCamera _vCam, CinemachineBasicMultiChannelPerlin _vCamNoise) //Gets the correct components from whichever vCam is active and being used
    {
        currentVcam = _vCam;
        currentVCamNoise = _vCamNoise;

        originalNoise = currentVCamNoise.m_NoiseProfile;
    }

    private float shakeDuration = 0.6f; //Settings that effect shake intensity
    private float shakeAmplitude = 0.6f;
    private float shakeFrequency = 0.6f;

    private float shakeElapsedTime = 0f;
    private bool startShake = false;

    public bool StartShake
    {
        get { return startShake; }
        set { startShake = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.instance.onCameraShake += toggleShake; //Registers self to camera shake event

        StartShake = false;
        shakeElapsedTime = shakeDuration;
    }

    private void toggleShake() //Called by camera shake event
    {
        StartShake = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startShake)
        {
            shakeElapsedTime -= Time.deltaTime; //Starts shakeCountdown
        }

        if (currentVcam != null || currentVCamNoise != null)
        {
            if (startShake)
            {
                if (shakeElapsedTime > 0)
                {
                    currentVCamNoise.m_NoiseProfile = noiseToSwitchTo; //Switches to 6D shake

                    currentVCamNoise.m_AmplitudeGain = shakeAmplitude;
                    currentVCamNoise.m_FrequencyGain = shakeFrequency;

                }
                else
                {
                    Debug.Log("Switching back");
                    currentVCamNoise.m_NoiseProfile = originalNoise; //Switched back to original noise

                    shakeElapsedTime = 0f;
                    StartShake = false;

                }
            }
           
        }
    }
}

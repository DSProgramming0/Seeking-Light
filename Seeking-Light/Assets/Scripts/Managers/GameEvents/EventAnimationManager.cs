using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class EventAnimationManager : MonoBehaviour
{
    //This class controls the details of each event, what happens when it is triggered. Methods suscribe to the Actions within the GameEvents class and are called on an EventTrigger class

    [Header("Animators")]
    [SerializeField] private Animator bin1;

    [Header("Lights")]
    [SerializeField] private GameObject light1;

    void Start() //Suscribe methods here
    {
        GameEvents.instance.onBinKnockover1 += bin1Knockover;
        GameEvents.instance.onLightFlicker1 += light1Flicker;
    }

    private void bin1Knockover() 
    {
        Debug.Log("Bin knocked over");
        bin1.SetTrigger("Knockover");
        AudioSource binAudioSource = bin1.GetComponent<AudioSource>();
        SoundManager.Play3DSound(SoundManager.Sound.BinKnockover1, false, true, 6f, .4f, bin1.transform.position);

        GameEvents.instance.onBinKnockover1 -= bin1Knockover;
    }

    private void light1Flicker()
    {
        Debug.Log("Light is flickering");
        light1.GetComponent<Light2D>().enabled = true;
        light1.GetComponent<LightFlicker>().enabled = true;
        SoundManager.Play3DSound(SoundManager.Sound.LightFlicker1, true, false, 0f, .2f, light1.transform.position);

        GameEvents.instance.onLightFlicker1 -= light1Flicker;
    }
}

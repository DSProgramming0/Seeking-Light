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
    [SerializeField] private GameObject light2;

    [Header("Releasing Light Event")]
    [SerializeField] private GameObject playerCompanion;
    [SerializeField] private Animator releasedLightAnim;
    [SerializeField] private List<Joint2D> cageHingeJoints;
    [SerializeField] private Transform setPoint;
    [SerializeField] private List<DialogueTrigger> LightBuddyDialogs;

    [Header("Game Objects, Transforms & RB's")]
    [SerializeField] private Transform deadBody1;
    [SerializeField] private Rigidbody2D fallingCross;

    void Start() //Suscribe methods here
    {
        GameEvents.instance.onBinKnockover1 += bin1Knockover;
        GameEvents.instance.onLightFlicker1 += light1Flicker;
        GameEvents.instance.onLightFlicker2 += light2Flicker;
        GameEvents.instance.onLightRelease += lightRelease;
        GameEvents.instance.onCrossFall += callCrossFall;
    }

    private void lightRelease()
    {
        foreach (Joint2D joint in cageHingeJoints)
        {
            Destroy(joint);
        }

        releasedLightAnim.transform.parent = setPoint;
        releasedLightAnim.SetTrigger("released");

        LightBuddyDialogs[0].ConversationExpended = true;
        LightBuddyDialogs[1].ConversationExpended = false;
    }

    public void enableLightCompanion()
    {
        PlayerInfo.instance.PlayerHasLight = true;
        playerCompanion.SetActive(true);
        Destroy(releasedLightAnim.gameObject);
    }

    private void bin1Knockover() 
    {
        Debug.Log("Bin knocked over");
        bin1.SetTrigger("Knockover");
        AudioSource binAudioSource = bin1.GetComponent<AudioSource>();
        SoundManager.Play3DSound(SoundManager.Sound.BinKnockover1, false, true, 6f, .4f, 70f, bin1.transform.position);

        GameEvents.instance.onBinKnockover1 -= bin1Knockover;
    }

    private void light1Flicker()
    {
        Debug.Log("Light is flickering");
        light1.GetComponent<Light2D>().enabled = true;
        light1.GetComponent<LightFlicker>().enabled = true;
        SoundManager.Play3DSound(SoundManager.Sound.LightFlicker1, true, false, 0f, .2f, 90f, light1.transform.position);
    }

    private void light2Flicker()
    {
        Debug.Log("Light is flickering");
        light2.GetComponent<Light2D>().enabled = true;
        light2.GetComponent<LightFlicker>().enabled = true;
        SoundManager.Play3DSound(SoundManager.Sound.LightFlicker1, true, false, 0f, .2f,  90f, light2.transform.position);
        SoundManager.Play3DSound(SoundManager.Sound.HorrorSpike1, false, true, 8f, .3f, 90f, light2.transform.position);
    }   

    private void callCrossFall()
    {
        StartCoroutine(crossFall());
    }

    private  IEnumerator crossFall()
    {
        fallingCross.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(3f);
        fallingCross.gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
        fallingCross.bodyType = RigidbodyType2D.Static;
        fallingCross.simulated = false;
    }
}

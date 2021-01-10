using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusObject : MonoBehaviour
{
    [SerializeField] private CompanionControl companionController;

    [SerializeField] private bool canFocus = false;
    [SerializeField] private bool shouldReveal = false;

    private bool effectPlayed = false;
    [SerializeField] private ParticleSystem sparkleEffect;
    [SerializeField] private ParticleSystem revealEffect;

    public bool ShouldReveal
    {
        get { return shouldReveal; }
        set { shouldReveal = value; }
    }

    public bool CanFocus
    {
        get { return canFocus; }
        set { canFocus = value; }
    }

    void Update()
    {
        if(shouldReveal == true) //Set by the companionControl method
        {
            revealSecret();
        }        
    }

    void OnMouseEnter() //Check when the mouse if over the object area
    {
        CanFocus = true;
        companionController.setCurrentFocusObject(this);
    }

    void OnMouseExit() //Checks when the mouse has left the object area
    {
        CanFocus = false;
        companionController.clearCurrentFocusObject();
    }

    private void revealSecret() //Reveals secret
    {
        if (effectPlayed == false)
        {
            //PLAY SOME EFFECT, REVEAL SOMETHING!
            sparkleEffect.Stop();
            SoundManager.Play2DSound(SoundManager.Sound.SecretRevealed, 4f, .2f);
            revealEffect.Play();
            Debug.Log(gameObject.name + "Revealed");
            effectPlayed = true;
        }        
    }
}

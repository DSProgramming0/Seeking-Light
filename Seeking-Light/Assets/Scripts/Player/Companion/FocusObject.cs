using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusObject : MonoBehaviour
{
    [SerializeField] private CompanionControl companionController;

    [SerializeField] private bool canFocus = false;
    [SerializeField] private bool shouldReveal = false;

    [SerializeField] private bool shouldShowMessage = false;
    [SerializeField] private DoTweener promptTween;
    [SerializeField] private Text promptText;
    [TextArea(1,3)]
    [SerializeField] private string messageToDisplay;

    private bool effectPlayed = false;
    [SerializeField] private ParticleSystem sparkleEffect;
    [SerializeField] private ParticleSystem revealEffect;

    void Start()
    {
        if(shouldShowMessage == true)
        {
            promptText = promptTween.gameObject.GetComponent<Text>();
        }
    }

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
            if(shouldShowMessage == true)
            {
                promptTween.InvokeTween(false);
                promptText.text = messageToDisplay;
                shouldShowMessage = false;
            }

            sparkleEffect.Stop();
            SoundManager.Play2DSound(SoundManager.Sound.SecretRevealed, 4f, .2f);
            revealEffect.Play();
            Debug.Log(gameObject.name + "Revealed");
            effectPlayed = true;
        }        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerEnemy : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D thisRB;
    [SerializeField] private stalkerState currentState;
    [SerializeField] private AudioSource thisAudioSource;
    [SerializeField] private ParticleSystem enemyParticles;

    [Header("Componenets To Disable")]
    [SerializeField] private List<SpriteRenderer> thisSprite;
    [SerializeField] private CircleCollider2D thisCollider;  

    [Header("Current Values")]
    [SerializeField] private float currentHungerMultiplier;
    [SerializeField] private float currentSpeed;
    [SerializeField] private Vector3 currentOffset;
    private SoundManager.Sound currentSound;
    private bool currentSoundPlaying = false;

    [Header("Hunger Meter")]  
    [SerializeField] private bool playerIsHoldingBack = false;
    [SerializeField] private float hungerDecreaseAmount;
    [SerializeField] private float hungerMeter;

    [SerializeField] private float hungerMultiplerFar;
    [SerializeField] private float hungerMultiplerMid;
    [SerializeField] private float hungerMultiplerClose;


    [Header("Movement")]
    [SerializeField] private float speedFar;
    [SerializeField] private float midSpeed;
    [SerializeField] private float closeSpeed;

    [SerializeField] private Vector3 offsetFar;
    [SerializeField] private Vector3 offsetMid;
    [SerializeField] private Vector3 offsetClose;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool PlayerIsHoldingBack
    {
        get { return playerIsHoldingBack; }
        set { playerIsHoldingBack = value; }
    }

    // Update is called once per frame
    void Update()
    {
        hungerMeterGain();

        SoundManager.PlayStaticSound(thisAudioSource, currentSound);

        switch (currentState)
        {
            case stalkerState.INVISIBLE:
                currentOffset = offsetFar;
                currentSpeed = closeSpeed;
                currentHungerMultiplier = 0;
                hungerMeter = 0;
                hideEnemy(true);
                break;
            case stalkerState.FAR:
                currentOffset = offsetFar;
                currentSpeed = speedFar;
                currentHungerMultiplier = hungerMultiplerFar;
                currentSound = SoundManager.Sound.Demon_Stalk_1;
                currentSoundPlaying = false;
                hideEnemy(false);              
                break;
            case stalkerState.MID:
                currentOffset = offsetMid;
                currentSpeed = midSpeed;
                currentHungerMultiplier = hungerMultiplerMid;
                currentSound = SoundManager.Sound.Demon_Stalk_2;
                currentSoundPlaying = false;
                break;
            case stalkerState.CLOSE:
                currentOffset = offsetClose;
                currentSpeed = closeSpeed;
                currentHungerMultiplier = hungerMultiplerClose;
                currentSound = SoundManager.Sound.Demon_Stalk_3;
                currentSoundPlaying = false;
                break;
            default:
                break;
        }

        thisRB.MovePosition(Vector3.Slerp(transform.position, target.position + currentOffset, currentSpeed * Time.deltaTime));
    }

    public void hungerMeterGain()
    {
        if (playerIsHoldingBack == false)
        {
            hungerMeter = hungerMeter += currentHungerMultiplier * Time.deltaTime;

            var emission = enemyParticles.emission;
            emission.rateOverTime = 0;
            if (enemyParticles.particleCount == 0)
            {
                enemyParticles.Pause();
            }
        }
        else
        {
            hungerMeter = hungerMeter -= hungerDecreaseAmount * Time.deltaTime;

            currentSound = SoundManager.Sound.Demon_Recoil_1;

            var emission = enemyParticles.emission;
            emission.rateOverTime = 25;
            enemyParticles.Play();

            if (hungerMeter <= 0)
            {
                if (currentState == stalkerState.CLOSE)
                {
                    currentState = stalkerState.MID;
                    hungerMeter = 70f;
                }
                else if (currentState == stalkerState.MID)
                {
                    currentState = stalkerState.FAR;
                    hungerMeter = 0f;
                }
                else if(currentState == stalkerState.FAR)
                {
                    hungerMeter = 0f;
                }
            }
        }

        if (hungerMeter >= 101f)
        {
            hungerMeter = 0;
            toggleState();
        }
    }

    public void toggleState()
    {        
        if(currentState == stalkerState.FAR)
        {
            currentState = stalkerState.MID;
        }
        else if (currentState == stalkerState.MID)
        {
            currentState = stalkerState.CLOSE;
        }
        else if(currentState == stalkerState.CLOSE)
        {
            target.GetComponent<PlayerDeath>().Death();
        }
    }

    public void hideEnemy(bool _shouldHide)
    {
        if (_shouldHide)
        {
            foreach(SpriteRenderer sprite in thisSprite)
            {
                sprite.enabled = false;
            }
            thisCollider.enabled = false;
        }
        else
        {
            foreach (SpriteRenderer sprite in thisSprite)
            {
                sprite.enabled = true;
            }
            thisCollider.enabled = true;
        }
    }

    private enum stalkerState
    {
        INVISIBLE,
        FAR,
        MID,
        CLOSE
    }
}





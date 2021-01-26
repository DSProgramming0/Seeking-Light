using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemManager : MonoBehaviour
{
    public ParticleEmittingState currentParticleEmitting;
    [SerializeField] private List<ParticleSystem> particleSystems;

    [SerializeField] private AudioSource rainSound;
    private bool soundPlaying = false;

    [SerializeField] private Transform target;
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;

    [SerializeField] private float movementSmooth;

    public void switchTarget(Transform _newtarget)
    {
        target = _newtarget;
    }
   
    void Update()
    {
        switch (currentParticleEmitting)
        {
            case ParticleEmittingState.NONE:

                if(soundPlaying == true)
                {
                    GameManager.instance.callFadeOutSound(rainSound, 3f);
                    soundPlaying = false;
                }

                foreach (ParticleSystem thisParticle in particleSystems)
                {
                    thisParticle.Stop();
                }
                break;
            case ParticleEmittingState.FALLING_LEAVES:
                if (soundPlaying == true)
                {
                    GameManager.instance.callFadeOutSound(rainSound, 3f);
                    soundPlaying = false;
                }

                particleSystems[0].Play();
                if (particleSystems[1].isPlaying)
                {
                    particleSystems[1].Stop();
                }

                break;

            case ParticleEmittingState.RAINING:
                particleSystems[1].Play();

                if(soundPlaying == false)
                {
                    rainSound =  SoundManager.Play3DSound(SoundManager.Sound.Rainfall1, true, false, 0, .3f, 300f, particleSystems[1].transform.position);
                    rainSound.transform.parent = particleSystems[1].transform;
                    soundPlaying = true;
                }
            
                if (particleSystems[0].isPlaying)
                {
                    particleSystems[0].Stop();
                }

                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = new Vector2(Mathf.Lerp(transform.position.x, target.position.x + offsetX, Time.fixedDeltaTime * movementSmooth), Mathf.Lerp(transform.position.y, target.position.y + offsetY, Time.fixedDeltaTime * movementSmooth));
    }    
}

public enum ParticleEmittingState
{
    NONE,
    FALLING_LEAVES,
    RAINING

}

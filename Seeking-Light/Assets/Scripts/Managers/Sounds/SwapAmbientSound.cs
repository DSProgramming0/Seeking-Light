using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapAmbientSound : MonoBehaviour
{
    [SerializeField] private AmbientSoundType thisSoundSwap;
    private bool hasSwitched = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(hasSwitched == false)
        {
            if (collision.CompareTag("Player"))
            {
                switch (thisSoundSwap)
                {
                    case AmbientSoundType.Whispers_1:
                        SoundManager.PlayStaticSound(GameAssets.instance.mainAudioSource, SoundManager.Sound.Whispers_1);
                        break;
                    case AmbientSoundType.Whispers_2:
                        SoundManager.PlayStaticSound(GameAssets.instance.mainAudioSource, SoundManager.Sound.Whispers_2);
                        break;
                    case AmbientSoundType.Ambience_1:
                        SoundManager.PlayStaticSound(GameAssets.instance.mainAudioSource, SoundManager.Sound.Ambience1);
                        break;
                    case AmbientSoundType.KidsPlaying:
                        SoundManager.PlayStaticSound(GameAssets.instance.mainAudioSource, SoundManager.Sound.KidsPlaying);
                        break;
                    case AmbientSoundType.KidCrying:
                        SoundManager.PlayStaticSound(GameAssets.instance.mainAudioSource, SoundManager.Sound.KidsCrying);

                        break;
                    default:
                        break;
                }

                hasSwitched = true;
            }
        }       
    }

    private enum AmbientSoundType
    {
        Whispers_1,
        Whispers_2,
        
        Ambience_1,

        KidsPlaying,
        KidCrying
    }
}

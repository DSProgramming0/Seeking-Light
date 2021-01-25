using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEffect : MonoBehaviour
{
    [SerializeField] private TypeOfEffect thisEffect;

    [SerializeField] private LayerMask possibleCollisions;
    private bool hasPlayed = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasPlayed == false)
        {
            if (collision.gameObject.layer == 16)
            {
                Debug.Log("Detected ground layer");
                switch (thisEffect)
                {
                    case TypeOfEffect.HeavyObject:
                        SoundManager.Play3DSound(SoundManager.Sound.MetallicCrash1, false, true, 4f, .5f, 90f, transform.position);
                        break;
                    default:
                        break;
                }

                hasPlayed = true;
                GameEvents.instance.CameraShake();
            }
        }
        
    }

    private enum TypeOfEffect
    {
        HeavyObject,
    }
}

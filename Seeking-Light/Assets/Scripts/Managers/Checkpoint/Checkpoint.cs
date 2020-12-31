using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private RespawnManager _respawnManger;

    [SerializeField] private LightFlicker thisCheckpointLight;
    private BoxCollider2D thisTrigger;

    void Awake()
    {
        thisTrigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {            
                Debug.Log("PlayerDetected");
                thisCheckpointLight.stopEffect();
                _respawnManger.setCurrentCheckpoint(this);


                thisTrigger.enabled = false;           
        }
    }
}


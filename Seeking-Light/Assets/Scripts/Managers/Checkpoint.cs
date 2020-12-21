using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private RespawnManager _respawnManger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("detected something");

        if (collision.CompareTag("Player"))
        {
            Debug.Log("PlayerDetected");
            _respawnManger.setCurrentCheckpoint(this);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private CharacterController2D _characterController;

    [SerializeField] private List<Checkpoint> CheckpointsInLevel;
    [SerializeField] private Checkpoint currentCheckpoint;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setCurrentCheckpoint(Checkpoint thisCheckpoint)
    {
        currentCheckpoint = thisCheckpoint;
    }

    public void StartRespawn()
    {
        //Will evenutally play death screen/Animation/ragdoll

        //Reset player
        _characterController.resetPlayerAtLastCheckpoint(currentCheckpoint);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private Transform playerTrans;
    private Vector3 lastCameraPosition;

    [SerializeField] private Vector2 parrallaxEffectMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        lastCameraPosition = playerTrans.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 deltaMovement = playerTrans.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parrallaxEffectMultiplier.x, deltaMovement.y * parrallaxEffectMultiplier.y);
        lastCameraPosition = playerTrans.position;
    }
}

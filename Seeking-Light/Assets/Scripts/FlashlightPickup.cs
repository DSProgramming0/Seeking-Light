using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            //DISPLAY UI, FLASHLIGHT FOUND!
            PlayerInfo.instance.PlayerHasFlashLight = true;
            Destroy(this.gameObject);
        }
    }

}

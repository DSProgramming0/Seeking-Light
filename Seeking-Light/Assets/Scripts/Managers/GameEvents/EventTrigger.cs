using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    //Calls the correct event depending on what the "thisEvent" enum is set to.

    [SerializeField] private EventList thisEvent;

    private bool hasTriggered = false; //Can only be triggered once.

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(hasTriggered == false)
        {
            if (collision.CompareTag("Player"))
            {
                switch (thisEvent)
                {
                    case EventList.BIN_KNOCKOVER_1:
                        GameEvents.instance.BinKnockover1();
                        break;
                    case EventList.LIGHTFLICKER_1:
                        GameEvents.instance.LightFlicker1();
                        break;
                    case EventList.LIGHTFLICKER_2:
                        GameEvents.instance.LightFlicker2();
                        break;
                    case EventList.STALKERREVEAL_1:
                        GameEvents.instance.StalkerReveal();
                        break;
                    default:
                        break;
                }

                hasTriggered = true;

                Destroy(this.gameObject, 2f);
            }
        }        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    //A singleton class that stores all events throughout game.
    public static GameEvents instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    //If an event has no suscribers the system will report it.
    public event Action onLightRelease;
    public void LightRelease()
    {
        if (onLightRelease != null)
        {
            Debug.Log("Calling event");
            onLightRelease();
        }
        else
        {
            Debug.LogError(onLevelReset + "Has no suscribers!");
        }
    }

    public event Action onLevelReset;
    public void LevelReset()
    {
        if (onLevelReset != null)
        {
            onLevelReset();
        }
        else
        {
            Debug.LogError(onLevelReset + "Has no suscribers!");
        }
    }

    public event Action onStalkerReveal;
    public void StalkerReveal()
    {
        if (onStalkerReveal != null)
        {
            onStalkerReveal();
        }
        else
        {
            Debug.LogError(onStalkerReveal + "Has no suscribers!");
        }
    }

    //If an event has no suscribers the system will report it.
    public event Action onBinKnockover1;
    public void BinKnockover1()
    {
        if(onBinKnockover1 != null)
        {
            onBinKnockover1();
        }
        else
        {
            Debug.LogError(onBinKnockover1 + "Has no suscribers!");
        }
    }

    public event Action onLightFlicker1;
    public void LightFlicker1()
    {
        if (onLightFlicker1 != null)
        {
            onLightFlicker1();
        }
        else
        {
            Debug.LogError(onLightFlicker1 + "Has no suscribers!");
        }
    }

    public event Action onLightFlicker2;
    public void LightFlicker2()
    {
        if (onLightFlicker2 != null)
        {
            onLightFlicker2();
        }
        else
        {
            Debug.LogError(onLightFlicker1 + "Has no suscribers!");
        }
    }

}

public enum EventList
{
    BIN_KNOCKOVER_1,

    LIGHTFLICKER_1,
    LIGHTFLICKER_2,

    STALKERREVEAL_1,
}

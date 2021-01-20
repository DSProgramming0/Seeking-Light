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

    public event Action onCameraShake;
    public void CameraShake()
    {
        if (onCameraShake != null)
        {
            Debug.Log("CameraShaking called");
            onCameraShake();
        }
        else
        {
            Debug.LogError(onCameraShake + "Has no suscribers!");
        }
    }

    //If an event has no suscribers the system will report it.
    public event Action onLightRelease; //First interaction between player and companion
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

    public event Action onCrossFall; //First interaction between player and companion
    public void CrossFall()
    {
        if (onLightRelease != null)
        {
            Debug.Log("Calling event");
            onCrossFall();
        }
        else
        {
            Debug.LogError(onCrossFall + "Has no suscribers!");
        }
    }

    public event Action onLevelReset; //Things that need to reset after player dies
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

    public event Action onStalkerReveal; //When the stalker enemy reveals itself
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

    CROSSFALL,
}

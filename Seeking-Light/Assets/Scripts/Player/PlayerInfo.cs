using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    //USED TO STORE VALUES WHICH MULTIPLE SCRIPTS WILL USE
    public static PlayerInfo instance;

    [SerializeField] private bool playerHasControl = true;
    private bool facingRight;
    private Vector2 dir;
    private bool isClimbing;
    private bool isCrouching;
    private bool playerHasFlashlight = false;
    private Rigidbody2D thisRB;

    void Awake()
    {
        instance = this;
        thisRB = GetComponent<Rigidbody2D>();
    }

    public bool PlayerHasControl
    {
        get { return playerHasControl; }
        set { playerHasControl = value; }
    }
    //returns players current direction based on local scale
    public bool FacingRight
    {
        get { return facingRight; }
        set { facingRight = value; }
    }

    public bool IsClimbing
    {
        get { return isClimbing; }
        set { isClimbing = value; }
    }

    public bool IsCrouching
    {
        get { return isCrouching; }
        set { isCrouching = value; }
    }

    public bool PlayerHasFlashLight
    {
        get { return playerHasFlashlight; }
        set { playerHasFlashlight = value; }
    }
    
    //returns player current direction based on player input
    public Vector2 Dir
    {
        get { return dir; }
        set { dir = value;  }
    }

    public Rigidbody2D returnRB()
    {
        return thisRB;
    }
}

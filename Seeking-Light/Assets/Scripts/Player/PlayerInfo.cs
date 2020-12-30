using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    //USED TO STORE VALUES WHICH MULTIPLE SCRIPTS WILL USE
    public static PlayerInfo instance;

    private bool facingRight;
    private Vector2 dir;
    private Rigidbody2D thisRB;

    void Awake()
    {
        instance = this;
        thisRB = GetComponent<Rigidbody2D>();
    }

    //returns players current direction based on local scale
    public bool FacingRight
    {
        get { return facingRight; }
        set { facingRight = value; }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyForces : MonoBehaviour
{
    private Rigidbody2D thisRb;
    private float randomDir;
    // Start is called before the first frame update

    void Awake()
    {
        thisRb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {

        InvokeRepeating("applyForce", 3f, 15f);
    }
    
    private void applyForce()
    {
        Debug.Log("Called");
        randomDir = Random.Range(-400, 400f);
        thisRb.AddForce(new Vector2(randomDir, 0));
    }
}

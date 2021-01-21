using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTargetManagerNew : MonoBehaviour
{
    [SerializeField] private Transform IK_Target;

    [SerializeField] private float rayDistance;
    [SerializeField] private float threshold;
    [SerializeField] private float resetSpeed;
    [SerializeField] private float speed;

    [SerializeField] private Transform rayPoint;
    [SerializeField] private Transform rayPointFront;
    [SerializeField] private Transform rayPointBack;

    [SerializeField] private Transform resetPos;
    [SerializeField] private Transform resetPosBack;
    [SerializeField] private Transform resetPosFront;

    [SerializeField] private Vector2 currentPos;
    [SerializeField] private Vector2 newPoint;
    private Vector2 dir;

    [SerializeField] private LayerMask whatIsWalkable;

    [SerializeField] private IKTargetManagerNew oppositeLeg;
    [SerializeField] private bool m_Grounded = false;
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private float groundCheckRadius;

    private bool resetHit = false;
    private bool newPosFound = false;
    [SerializeField] private bool flipDir = false;
    [SerializeField] private bool facingDown = false;

    // Start is called before the first frame update
    void Start()
    {
        currentPos = produceStartRay();

        Init(); //Initialises directions rays should shoot from and where they should start       
    }

    public bool M_Grounded
    {
        get { return m_Grounded; }
        set { m_Grounded = value; }
    }

    void Init()
    {
        if (flipDir && facingDown) //If the leg is facing backwards and is walking on the ground
        {
            dir = Vector2.down; //The ray will shoot down
            rayPoint = rayPointBack; //And be using the back rayCast object
            resetPos = resetPosBack;
        }
        else if (!flipDir && facingDown)
        {
            dir = Vector2.down;
            rayPoint = rayPointFront;
            resetPos = resetPosFront;
        }
        else if (flipDir && !facingDown)
        {
            dir = Vector2.up;
            rayPoint = rayPointBack;
            resetPos = resetPosBack;
        }
        else if (!flipDir & !facingDown) //Otherwise if the leg is walking on a ceiling 
        {
            dir = Vector2.up; //The ray should shoot up
            rayPoint = rayPointFront; //And use the front rayCast object.
            resetPos = resetPosFront;
        }
    }

    // Update is called once per frame
    void Update()
    {
        groundCheck();

        if (newPosFound == false) //Switches when threshold has been crossed and leg is not in the same position as previously
        {
            MoveIK(currentPos); //If the threshold has not be crossed, the IK should remain at its current target
        }

        newPoint = findNextPoint(); // sets the new point by constanstly casting a ray in this function

        float distanceToNextPoint = Vector2.Distance(IK_Target.position, newPoint);
        Debug.Log(distanceToNextPoint);
        if (distanceToNextPoint >= threshold) //Checks distance between the IK target and the new point 
        {
            Debug.Log("Should move now");
            MoveToReset(); //If the threshold has been crossed, begin moving process
            currentPos = newPoint; //the new point now becomes the current point the IK should be at
        }
        else
        {
            newPosFound = false;
        }
    }

    private Vector2 produceStartRay()
    {
        RaycastHit2D hit = Physics2D.Raycast(rayPoint.position, dir, rayDistance, whatIsWalkable); //Shoots a ray in the specified direction, called in start function as this grounds the IK position

        return hit.point;
    }

    private Vector2 findNextPoint()
    {
        RaycastHit2D hit = Physics2D.Raycast(rayPoint.position, dir, rayDistance, whatIsWalkable); //Starts finding the next postion to move to

        if(hit.point == Vector2.zero)
        {
            transform.parent.position = transform.parent.position += Vector3.down * 30 * Time.deltaTime; //If the ray is not hitting a suitable surface, push the parent object down
        }

        return hit.point;
    }

    private void groundCheck()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, groundCheckRadius, whatIsWalkable);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;          
            }
        }       
    }

    private void MoveToReset()
    {
        IK_Target.position = Vector2.Lerp(IK_Target.position, resetPos.position, resetSpeed * Time.deltaTime); //Starts to move leg up to the reset position
        float distance = Vector2.Distance(IK_Target.position, resetPos.position);
        if(distance <= .05f)
        {
            resetHit = true; //Once in range, the bool registers as true
        }

        if(resetHit == true) //And starts calling the MoveIK function whichs moves the leg to the newPoint given by the findNextPoint function called in the update method
        {
            MoveIK(newPoint);
        }
    }

    private void MoveIK(Vector2 _newPos)
    {
        IK_Target.position = Vector2.Lerp(IK_Target.position, _newPos, speed * Time.deltaTime);  //Lerp towards the newly designated target    

        if (_newPos != currentPos)
        {
            newPosFound = true; //If the new position  is not the same as the last, a new postion has been found
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(rayPoint.position, dir * rayDistance);
        Gizmos.DrawLine(IK_Target.position, newPoint);

        Gizmos.DrawSphere(newPoint, 1f);
    }
}

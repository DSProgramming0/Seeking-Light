using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class IKTargetManager : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] private Vector2 parentPos;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Transform endJoint;
    [SerializeField] private Transform IK_Target;
    [SerializeField] private Transform resetPos;

    [Header("Raycasts")]
    [SerializeField] private Transform rayPoint1;
    [SerializeField] private Transform rayPoint2;
    [SerializeField] private float rayDistance;
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private List<Vector2> hitPoints;
    [SerializeField] private Vector2 bestPoint;
    [SerializeField] private Vector3 offset;
    
    [Header("Values")]
    private float rayCastCountDown;
    [SerializeField] private float rayCastCountDownLimit;
    private float backwardsMoveCountdown;
    [SerializeField] private float backwardsMoveCountdownLimit;

    [SerializeField] private float m_Threshold;
    [SerializeField] private float speed;   

    // Start is called before the first frame update
    void Start()
    {
        rayCastCountDown = rayCastCountDownLimit;
        backwardsMoveCountdown = backwardsMoveCountdownLimit;

        InvokeRepeating("storeParentPos", 1f, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.parent.position.x < parentPos.x) //If the last stored position is more than the current position, player is moving backwards
        {
            Debug.Log("Obj has moved backwards");
            backwardsMoveCountdown -= Time.deltaTime;
            if(backwardsMoveCountdown == 0)
            {
                castRayPoints(); //Raycast more frequently
                backwardsMoveCountdown = backwardsMoveCountdownLimit;
            }
        }

        float distanceToPlayer = Vector2.Distance(endJoint.position, playerTarget.position); //Checks distance to player

        if(distanceToPlayer <= m_Threshold) //If less that the threshold, player is in reach.
        {
            Debug.Log("Calling now!");
            IK_Target.position = Vector2.Lerp(IK_Target.position, playerTarget.position, speed * Time.deltaTime);
        }
        else
        {
            IK_Target.position = Vector2.Lerp(IK_Target.position, bestPoint, speed * Time.deltaTime); //Lerp towards the newly designated target

            float distanceToClosestPoint = Vector2.Distance(rayPoint2.position, bestPoint);

            if(distanceToClosestPoint > 30f)
            {
                castRayPoints();
                bestPoint = GetClosestPoint(); //If the closest point is too far away in between raycasts, do another raycast and get a new point
            }

        }

        rayCastCountDown -= Time.deltaTime; //Countdown timer between rays

        if (rayCastCountDown <= 0)
        {
            castRayPoints();
            rayCastCountDown = rayCastCountDownLimit;
        }
       
        if(hitPoints.Count > 1)
        {
            bestPoint = GetClosestPoint(); //Stores the point in which this tentacle should move
        }
    }

    private void castRayPoints()
    {
        //Casting 4 rays 2 up, 2 down in different positions
        RaycastHit2D hitFrontUp = Physics2D.Raycast(rayPoint2.position + offset, Vector2.up, rayDistance, whatIsTarget);
        RaycastHit2D hitFrontDown = Physics2D.Raycast(rayPoint2.position + offset, -Vector2.up, rayDistance, whatIsTarget);
        RaycastHit2D hitBackUp = Physics2D.Raycast(rayPoint1.position + offset, Vector2.up, rayDistance, whatIsTarget);
        RaycastHit2D hitBackDown = Physics2D.Raycast(rayPoint1.position + offset, -Vector2.up, rayDistance, whatIsTarget);

        //Add them to the list of potential targets
        hitPoints.Add(hitFrontUp.point);
        hitPoints.Add(hitFrontDown.point);
        hitPoints.Add(hitBackUp.point);
        hitPoints.Add(hitBackUp.point);

        if (hitPoints.Count >= 8) //If the list is greater than 8 vector 2 values, remove the first 4 values
        {
            hitPoints.RemoveRange(0, 4);
        }
    } 

    private void storeParentPos() //Stores the parents postion at intervals of half a second
    {
        parentPos = transform.parent.position;
    }

    Vector2 GetClosestPoint() //returns the closest point within the list of hitPoints
    {
        Vector2 bestTarget = IK_Target.position;
        float closestDistanceSqr = Mathf.Infinity;
        Vector2 currentPosition = rayPoint2.position;
        foreach (Vector2 potentialTarget in hitPoints)
        {
            Vector2 directionToTarget = potentialTarget - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(rayPoint2.position, Vector2.up * rayDistance);
        Gizmos.DrawRay(rayPoint2.position, -Vector2.up * rayDistance);
        Gizmos.DrawRay(rayPoint1.position, Vector2.up * rayDistance);
        Gizmos.DrawRay(rayPoint1.position, -Vector2.up * rayDistance);
    }
}

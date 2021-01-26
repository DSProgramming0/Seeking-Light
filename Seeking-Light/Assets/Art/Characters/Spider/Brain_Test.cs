using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain_Test : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsWalkable;

    [SerializeField] private List<LegMover_Test> legMovers;

    [SerializeField] private Transform middleHeightRay;
    [SerializeField] private Transform heightRay1;
    [SerializeField] private Transform heightRay2;
    [SerializeField] private Transform LeftRay;
    [SerializeField] private Transform RightRay;

    private Vector2 heightRay1Pos;
    private Vector2 heightRay2Pos;
    private Vector2 middleRayHitPos;
    private Vector2 leftRayHitPos;
    private Vector2 rightRayHitPos;

    [SerializeField] private float bestHeight;
    [SerializeField] private float heightThreshold;
    [SerializeField] private float offsetY;
    [SerializeField] private float rayDist;

    public LayerMask WhatIsWalkable
    {
        get { return whatIsWalkable; }
    }

    public Vector2 LeftRayHitPos
    {
        get { return leftRayHitPos; }
    }

    public Vector2 RightRayHitPos
    {
        get { return rightRayHitPos; }
    }

    // Update is called once per frame
    void Update()
    {
        castRays();
        checkLegStates();
    }

    void LateUpdate()
    {
        alterBodyHeight();
    }

    private void castRays()
    {

        RaycastHit2D hitLeft = Physics2D.Raycast(LeftRay.position, Vector2.down * rayDist, whatIsWalkable);
        RaycastHit2D hitRight = Physics2D.Raycast(RightRay.position, Vector2.down * rayDist, whatIsWalkable);
        RaycastHit2D hitDown = Physics2D.Raycast(middleHeightRay.position, Vector2.down * rayDist, whatIsWalkable);
        RaycastHit2D hitHeight1 = Physics2D.Raycast(heightRay1.position, Vector2.down * rayDist, whatIsWalkable);
        RaycastHit2D hitHeight2 = Physics2D.Raycast(heightRay2.position, Vector2.down * rayDist, whatIsWalkable);

        if (hitLeft)
        {
            leftRayHitPos = hitLeft.point;
        }

        if (hitRight)
        {
            rightRayHitPos = hitRight.point;
        }

        if (hitDown)
        {
            middleRayHitPos = hitDown.point;
        }

        if (hitHeight1)
        {
            heightRay1Pos = hitHeight1.point;
        }

        if (hitHeight2)
        {
            heightRay2Pos = hitHeight2.point;
        }
    }

    private void alterBodyHeight()
    {
        float heightPos = middleRayHitPos.y + offsetY;
        float heightPos1Left = heightRay1Pos.y + offsetY;
        float heightPos2Right = heightRay2Pos.y + offsetY;

        if (heightPos > heightPos1Left && heightPos > heightPos2Right)
        {
            bestHeight = heightPos;
        }
        else if (heightPos1Left > heightPos && heightPos1Left > heightPos2Right)
        {
            bestHeight = heightPos1Left;
        }
        else if (heightPos2Right > heightPos && heightPos2Right > heightPos1Left)
        {
            bestHeight = heightPos2Right;
        }
        else
        {
            bestHeight = heightPos;
        }

        if (Mathf.Abs(heightRay1Pos.y - middleRayHitPos.y) > heightThreshold || Mathf.Abs(heightRay2Pos.y - middleRayHitPos.y) > heightThreshold)
        {
            Debug.Log("Changing height");
            transform.parent.position = new Vector2(transform.parent.position.x, Mathf.Lerp(transform.parent.position.y, bestHeight, 2 * Time.deltaTime));
        }
    }

    private void checkLegStates()
    {
        foreach (LegMover_Test thisLeg in legMovers)
        {
            if (thisLeg.CanMove)
            {
                if (thisLeg.RightSide)
                {
                    thisLeg.UpdateCurrentPos(rightRayHitPos);
                }
                else
                {
                    thisLeg.UpdateCurrentPos(leftRayHitPos);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (leftRayHitPos != Vector2.zero || rightRayHitPos != Vector2.zero)
        {
            Gizmos.DrawLine(LeftRay.position, leftRayHitPos);
            Gizmos.DrawLine(RightRay.position, rightRayHitPos);
            Gizmos.DrawLine(middleHeightRay.position, middleRayHitPos);
            Gizmos.DrawLine(heightRay1.position, heightRay1Pos);
            Gizmos.DrawLine(heightRay2.position, heightRay2Pos);

            Gizmos.DrawWireSphere(leftRayHitPos, .25f);
            Gizmos.DrawWireSphere(rightRayHitPos, .25f);
            Gizmos.DrawWireSphere(middleRayHitPos, .25f);
            Gizmos.DrawWireSphere(heightRay1Pos, .25f);
            Gizmos.DrawWireSphere(heightRay2Pos, .25f);

        }
    }
}

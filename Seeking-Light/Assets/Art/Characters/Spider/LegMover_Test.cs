using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegMover_Test : MonoBehaviour
{
    [SerializeField] private Brain_Test _brain;

    [SerializeField] private Transform thisLegTarget;

    private Vector2 currentPos;

    private bool startPosFixed = false;
    [SerializeField] private bool rightSide;
    [SerializeField] private bool canMove = false;

    [SerializeField] private float speed;
    [SerializeField] private float threshold;

    void Start()
    {
        startRay();
        canMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        checkDistanceToTarget();
        setLegPos(currentPos);
    }

    #region Getters
    public bool RightSide
    {
        get { return rightSide; }
    }

    public bool CanMove
    {
        get { return canMove; }
    }

    private void setLegPos(Vector2 _Pos)
    {
        thisLegTarget.position = Vector2.Lerp(thisLegTarget.position, _Pos, speed * Time.deltaTime);

        startPosFixed = true;
    }

    #endregion

    private void checkDistanceToTarget()
    {
        float distanceToTarget;

        if (rightSide)
        {
            distanceToTarget = Vector2.Distance(thisLegTarget.position, _brain.RightRayHitPos);
        }
        else
        {
            distanceToTarget = Vector2.Distance(thisLegTarget.position, _brain.LeftRayHitPos);
        }

        if (startPosFixed)
        {
            if (distanceToTarget < threshold)
            {
                canMove = false;
            }
            else
            {
                canMove = true;
            }
        }
    }

    public void UpdateCurrentPos(Vector2 _newPos)
    {
        currentPos = _newPos;
    }

    private void startRay()
    {
        RaycastHit2D startHit = Physics2D.Raycast(transform.position, Vector2.down * 1.5f, _brain.WhatIsWalkable);

        if (startHit)
        {
            if (startPosFixed == false)
            {
                currentPos = startHit.point;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (_brain.RightRayHitPos != Vector2.zero || _brain.LeftRayHitPos != Vector2.zero)
        {
            if (rightSide)
            {
                Gizmos.DrawLine(thisLegTarget.position, _brain.RightRayHitPos);
            }
            else
            {
                Gizmos.DrawLine(thisLegTarget.position, _brain.LeftRayHitPos);
            }
        }
    }
}

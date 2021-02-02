using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepper : MonoBehaviour
{
    [SerializeField] Transform homeTransform;
    [SerializeField] Transform target;

    [SerializeField] float rayYoffset;
    [SerializeField] float heightOffset;

    [SerializeField] float stepOverShootFraction;
    [SerializeField] float stepAtDistance;
    [SerializeField] float stepAtAngle = 135f;

    [SerializeField] float moveDuration;

    [SerializeField] LayerMask groundLayerMask;
    RaycastHit2D hit;
    
    public bool Moving { get; private set; }
    Coroutine moveCoroutine;

    void Awake()
    {
        TryMove();
    }

    public void TryMove()
    {
        if (Moving) return;

        float distFromHome = Vector3.Distance(target.position, homeTransform.position);
        float angleFromHome = Quaternion.Angle(target.rotation, homeTransform.rotation);

        if (distFromHome > stepAtDistance)
        {
            if(GetGroundedEndPosition(out Vector3 endPos, out Vector3 endNormal))
            {
                Quaternion endRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(homeTransform.forward, endNormal), endNormal);
                StartCoroutine(MoveToHome(endPos, endRot, moveDuration));
            }
        }
    }

    private bool GetGroundedEndPosition(out Vector3 position, out Vector3 normal)
    {
        Vector3 towardHome = (homeTransform.position - transform.position).normalized;

        float overShootDistance = stepAtDistance * stepOverShootFraction;
        Vector3 overShootVector = towardHome * overShootDistance;

        Vector3 raycastOrigin = homeTransform.position + overShootVector + homeTransform.up * rayYoffset;
        hit = Physics2D.Raycast(raycastOrigin, -homeTransform.up, Mathf.Infinity, groundLayerMask);

        if(hit.point != null)
        {
            position = hit.point;
            normal = hit.normal;
            return true;
        }

        position = Vector3.zero;
        normal = Vector3.zero;
        return false;
    }

    private IEnumerator MoveToHome(Vector3 endPoint, Quaternion endRot, float moveTime)
    {
        Moving = true;

        Quaternion startRot = target.rotation;
        Vector3 startPoint = target.position;

        endPoint += homeTransform.up * heightOffset;
      
        Vector3 centerPoint = (startPoint + endPoint) / 2f;

        centerPoint += homeTransform.up * Vector3.Distance(startPoint, endPoint) / 2f;

        float timeElapsed = 0;

        do
        {
            timeElapsed += Time.deltaTime;

            float normalizedTime = timeElapsed / moveDuration;
            normalizedTime = Easing.EaseInOutCubic(normalizedTime);

            target.position = Vector3.Lerp(Vector3.Lerp(startPoint, centerPoint, normalizedTime), Vector3.Lerp(centerPoint, endPoint, normalizedTime), normalizedTime );
            transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            yield return null;
        }
        while (timeElapsed < moveDuration);

        Moving = false;
    }

    void OnDrawGizmosSelected()
    {
        if (Moving)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(target.position, 0.25f);
        Gizmos.DrawLine(target.position, homeTransform.position);
        Gizmos.DrawWireCube(homeTransform.position, Vector3.one * 0.1f);
        Gizmos.DrawSphere(hit.point, 0.5f);

    }
}

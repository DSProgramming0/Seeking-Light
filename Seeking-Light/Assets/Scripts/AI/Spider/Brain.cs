using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    [Header("IK")]
    [SerializeField] private Transform IK;
    [SerializeField] private List<LegStepper> legs;      

    [Header("Movement")]
    [SerializeField] private Transform target;
    

    [SerializeField] private float moveSpeed;

    [SerializeField] private float moveAcceleration;
    [SerializeField] private float heightAlterSpeed;

    [SerializeField] private float minDistToTarget;
    [SerializeField] private float maxDistToTarget;

    Vector3 currentVelocity;

    [SerializeField] LayerMask groundLayerMask;

    [Header("HeightAlteration")]
    [SerializeField] RaycastHit2D hit0;
    [SerializeField] RaycastHit2D hit1;
    [SerializeField] RaycastHit2D hit2;
    
    [SerializeField] Transform HeightRay0;
    [SerializeField] Transform HeightRay1;
    [SerializeField] Transform HeightRay2;

    [SerializeField] Vector2 heightRayPos0;
    [SerializeField] Vector2 heightRayPos1;
    [SerializeField] Vector2 heightRayPos2;

    [SerializeField] float bobbingSpeed;
    [SerializeField] float rayDist;
    [SerializeField] float bestHeight;
    [SerializeField] float yOffset;


    // Start is called before the first frame update
    void Awake()
    {
        IK.parent = null;

        StartCoroutine(LegUpdateCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        rootMotionUpdate();
        //castHeightRay();
        RaycastGround();
    }

    private IEnumerator LegUpdateCoroutine()
    {
        while (true)
        {
            do
            {
                legs[0].TryMove();
                legs[3].TryMove();

                yield return null;


            } while (legs[0].Moving || legs[3].Moving);

            do
            {
                legs[1].TryMove();
                legs[2].TryMove();

                yield return null;
            } while (legs[1].Moving || legs[2].Moving);
        }
    }

    //void castHeightRay()
    //{
    //    hit0 = Physics2D.Raycast(HeightRay0.position, -HeightRay0.up, rayDist, groundLayerMask);
    //    hit1 = Physics2D.Raycast(HeightRay1.position, -HeightRay1.up, rayDist, groundLayerMask);
    //    hit2 = Physics2D.Raycast(HeightRay2.position, -HeightRay2.up, rayDist, groundLayerMask);

    //    heightRayPos0 = hit0.point;
    //    heightRayPos1 = hit1.point;
    //    heightRayPos2 = hit2.point;

    //    float ray0Value = heightRayPos0.y + yOffset;
    //    float ray1Value = heightRayPos1.y + yOffset;
    //    float ray2Value = heightRayPos2.y + yOffset;

    //    if(ray0Value > ray1Value && ray0Value > ray2Value)
    //    {
    //        bestHeight = ray0Value;
    //    }

    //    if (ray1Value > ray0Value && ray1Value > ray0Value)
    //    {
    //        bestHeight = ray1Value;
    //    }

    //    if (ray2Value > ray0Value && ray2Value > ray1Value)
    //    {
    //        bestHeight = ray2Value;
    //    }

    //    transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, bestHeight, 1 - Mathf.Exp(-heightAlterSpeed * Time.deltaTime)));
    //}

    private void RaycastGround()
    {
        // int earthLayer = 1 << 8;
        var startPosition = transform.position;
        var groundRaycast = Physics2D.Raycast(startPosition, Vector3.down, 15f, groundLayerMask);
        Debug.DrawRay(startPosition, Vector3.down * 15f, Color.red);

        float pingPongValue = Mathf.PingPong(Time.time * bobbingSpeed, 5f);
        float newPos = groundRaycast.point.y + yOffset;
        transform.position = new Vector3(startPosition.x, Mathf.Lerp(startPosition.y, newPos + pingPongValue, 1 - Mathf.Exp(-heightAlterSpeed * Time.deltaTime)));
        ;
        // Debug.Log(groundRaycast.distance);
    }   

    void rootMotionUpdate()
    {
        Vector3 towardTarget = target.position - transform.position;
        Vector3 towardTargetProjected = Vector3.ProjectOnPlane(towardTarget, transform.up);
        Vector3 targetVelocity = Vector3.zero;

        float distToTarget = Vector3.Distance(transform.position, target.position);

        if(distToTarget > maxDistToTarget)
        {
            targetVelocity = moveSpeed * towardTargetProjected.normalized;           
        }
        else if(distToTarget < minDistToTarget)
        {
            targetVelocity = moveSpeed * -towardTargetProjected.normalized;          
        }
        
        currentVelocity.x = Mathf.Lerp(currentVelocity.x, targetVelocity.x, 1 - Mathf.Exp(-moveAcceleration * Time.deltaTime));

        transform.position = new Vector2(transform.position.x + currentVelocity.x * Time.deltaTime, transform.position.y); 

    }


    void OnDrawGizmos()
    {
        Gizmos.DrawRay(HeightRay0.position, Vector2.down * rayDist);
        Gizmos.DrawRay(HeightRay1.position, Vector2.down * rayDist);
        Gizmos.DrawRay(HeightRay2.position, Vector2.down * rayDist);

        Gizmos.DrawWireSphere(hit0.point, .15f);
        Gizmos.DrawWireSphere(hit1.point, .15f);
        Gizmos.DrawWireSphere(hit2.point, .15f);
    }

}

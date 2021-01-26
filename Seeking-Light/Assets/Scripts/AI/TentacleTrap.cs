using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleTrap : MonoBehaviour
{
    [SerializeField] private Transform IK_TARGET;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private List<Transform> targetPositions;

    [SerializeField] private int index;
    [SerializeField] private float speed;
    [SerializeField] private float m_Threshold;

    Coroutine MoveIE;
    // Start is called before the first frame update
    void Start()
    {
        MoveTentactle();
    }

    void MoveTentactle()
    {
        StartCoroutine(MovePlayer1Coroutine());
    }

    private void Update()
    {
        float distance = Vector2.Distance(IK_TARGET.position, playerTarget.position);
        Debug.Log(distance);
        if(distance <= m_Threshold)
        {
            Debug.Log("Is Attacking");
            StopCoroutine(MovePlayer1Coroutine());

            IK_TARGET.position = Vector3.MoveTowards(IK_TARGET.position, playerTarget.position, speed * 2 * Time.deltaTime);
        }
    }
    IEnumerator MovePlayer1Coroutine()
    {
        foreach (Transform _item in targetPositions)
        {
            Vector3 itemPos = _item.transform.position;
            while (Vector3.Distance(IK_TARGET.position, itemPos) > .0001)
            {
                IK_TARGET.position = Vector3.MoveTowards(IK_TARGET.position, itemPos, speed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(.15f);
        }

        StartCoroutine(MovePlayer1Coroutine());
    }    
}

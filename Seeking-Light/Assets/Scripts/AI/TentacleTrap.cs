using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleTrap : MonoBehaviour
{
    [SerializeField] private Transform IK_TARGET;
    [SerializeField] private Transform Player;
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
        }

        StartCoroutine(MovePlayer1Coroutine());
    }

    private void attackPlayer()
    {
        IK_TARGET.position = Vector3.MoveTowards(IK_TARGET.position, Player.position, 30f * Time.deltaTime);

    }
}

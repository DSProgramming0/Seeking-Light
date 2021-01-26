using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Test : MonoBehaviour
{
    [SerializeField] private float dir;
    // Start is called before the first frame update
    void Start()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SwitchDir"))
        {
            if (dir == 0.06f)
            {
                dir = -0.06f;
            }
            else
            {
                dir = 0.06f;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(transform.position.x + dir, transform.position.y);
    }
}

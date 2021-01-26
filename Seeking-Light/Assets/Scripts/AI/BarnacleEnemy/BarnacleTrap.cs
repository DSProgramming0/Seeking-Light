using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnacleTrap : MonoBehaviour
{
    [SerializeField] private AnimHook thisAnim;

    [SerializeField] private List<Rigidbody2D> links;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Transform player = collision.transform.parent;

            foreach(Rigidbody2D link in links)
            {
                link.bodyType = RigidbodyType2D.Static;
            }

            player.parent = this.transform;
            player.GetComponent<PlayerDeath>().caughtByBarnacle();
            thisAnim.startBarnaclePull();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

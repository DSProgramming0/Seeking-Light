using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryCheck : MonoBehaviour
{
    [SerializeField] private GameObject sectionToDisable;
    [SerializeField] private GameObject sectionToEnable;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (sectionToDisable != null)
            {
                GameManager.instance.disableSection(sectionToDisable);
            }
            if (sectionToEnable != null)
            {
                GameManager.instance.enableSection(sectionToEnable);
            }
        }
    }
}

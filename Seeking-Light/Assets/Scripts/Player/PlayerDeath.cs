using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private GameObject ragdollBody;

    [Header("Original Body")]
    [SerializeField] private GameObject playerBody;
    [SerializeField] private List<Behaviour> componentsToDisable;
    [SerializeField] private List<Collider2D> collidersToDisable;
    [SerializeField] private Rigidbody2D originalRigidBody;
    

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            activateRagdoll();
        }
    }

    public void activateRagdoll()
    {
        foreach(Behaviour component in componentsToDisable)
        {
            component.enabled = false;
        }

        foreach (Collider2D collider in collidersToDisable)
        {
            collider.enabled = false;
        }

        originalRigidBody.bodyType = RigidbodyType2D.Static;

        ragdollBody.SetActive(true);
        playerBody.SetActive(false);
      
    }
}

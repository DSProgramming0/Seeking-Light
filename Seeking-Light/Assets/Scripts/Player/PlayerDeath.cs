using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private AnimHook thisAnim;
    [SerializeField] private PlayerInteraction playerInteractions;

    [Header("Original Body")]
    [SerializeField] private List<SpriteRenderer> bodyPartSprites;
    [SerializeField] private List<Behaviour> componentsToDisable;
    [SerializeField] private List<Collider2D> collidersToDisable;
    [SerializeField] private Rigidbody2D originalRigidBody;

    [SerializeField] private GameObject flashLightDropped;
    [SerializeField] private GameObject flashLightToDrop;
    [SerializeField] private GameObject flashlightDropPoint;
    [SerializeField] private bool hasFlashlightDropped = false;

    [Header("Ragdoll Body")]
    [SerializeField] private GameObject ragdollBody;
    [SerializeField] private List<Rigidbody2D> ragdollRigidBodies;
    [SerializeField] private Rigidbody2D ragdollRB;
    [SerializeField] private Vector2 dirToPush;
    [SerializeField] private float pushForce;
    [SerializeField] private List<GameObject> bodyParts;
    [SerializeField] List<Vector3> bodyPartsPos;
    [SerializeField] List<Quaternion> bodyPartsRot;  
    
    void Start()
    {
        for (int i = 0; i < bodyParts.Count; i++) //Gets player body part positions and rotations to be used again later
        {
            bodyPartsPos.Add(bodyParts[i].transform.localPosition);
            bodyPartsRot.Add(bodyParts[i].transform.localRotation);
        }
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    Death();
        //}
    }

    public void Death()
    {
        thisAnim.resetPose(true); //Sets animator to an idle state (Prevents glitch where player would be stuck on last frame before death!)

        if(PlayerStates.instance.currentPlayerFlashlightState == PlayerFlashlightStates.FLASHLIGHT_ON)
        {
            if (hasFlashlightDropped == false)
            {
                flashLightDropped = Instantiate(flashLightToDrop, flashlightDropPoint.transform.position, Quaternion.identity);
                Object.Destroy(flashLightDropped, 4f);

                hasFlashlightDropped = true;
            }
        }
       

        PlayerStates.instance.currentPlayerConditionState = PlayerConditionStates.DEAD; //Sets player state

        Vector2 lastDir = PlayerInfo.instance.Dir; //Gets the players direction at time of death

        #region Disabling player original body
        foreach (Behaviour component in componentsToDisable) //Disables corresponding components on Player parent
        {
            component.enabled = false;
        }

        foreach (Collider2D collider in collidersToDisable) //Disables colliders related to the players original body
        {
            collider.enabled = false;
        }

        playerInteractions.resetFlashlightOnRespawn();
        originalRigidBody.bodyType = RigidbodyType2D.Static; //Disables  original rigid body
        originalRigidBody.simulated = false;

        foreach(SpriteRenderer sprite in bodyPartSprites) //Disables each body part sprite. Player original body is now invisible
        {
            sprite.enabled = false;
        }
        #endregion

        #region enabling ragdoll body Physics
        foreach (Rigidbody2D bodyPartRB in ragdollRigidBodies) //Enables each rigidbody attached to the players ragdoll body parts
        {
            bodyPartRB.bodyType = RigidbodyType2D.Dynamic;
            bodyPartRB.simulated = true;
        }

        ragdollBody.SetActive(true); //Shows the ragdoll body
        #endregion

        //Uses direction moving to determine which way the ragdoll show be forced to fall
        if (lastDir.x >= 0)
        {
            ragdollRB.AddForce(dirToPush * pushForce);
        }
        else
        {
            ragdollRB.AddForce(-dirToPush * pushForce);
        }

        StartCoroutine(startReset());
    }

    private IEnumerator startReset()
    {      
        yield return new WaitForSeconds(2f);
        //Disable any forces on body parts by making each body parts rigid body static. This stops any weird shaking when player dies
        foreach (Rigidbody2D bodyPartRB in ragdollRigidBodies)
        {
            bodyPartRB.bodyType = RigidbodyType2D.Static;
            bodyPartRB.simulated = false;
        }

        yield return new WaitForSeconds(2f);
        UIManager.instance.fadeout(); //Fades the screen while the player is being reset
        yield return new WaitForSeconds(1f);

        RespawnManager.instance.StartRespawn();
        GameEvents.instance.LevelReset();

        ragdollBody.SetActive(false); //Disables ragdoll body

        #region re enabling player original body
        //Reset all components on player
        foreach (Behaviour component in componentsToDisable) //Re enables all components that were disabled before
        {
            component.enabled = true;
        }

        foreach (Collider2D collider in collidersToDisable) //Re enables colliders attached to player main body
        {
            collider.enabled = true;
        }

        originalRigidBody.bodyType = RigidbodyType2D.Dynamic; //Re enables player rigidBody
        originalRigidBody.simulated = true;

        foreach (SpriteRenderer sprite in bodyPartSprites) //Re enables player original bodies sprites
        {
            sprite.enabled = true;
        }
        #endregion

        //Reposition Ragdoll local transform (To parent respawned position)
        ragdollBody.transform.position = transform.localPosition;
        ragdollBody.transform.rotation = transform.localRotation;

        for (int i = 0; i < bodyParts.Count; i++)
        {
            bodyParts[i].transform.localPosition = bodyPartsPos[i]; //Resets each body part to the correct position and rotation which was stored in the start method.
            bodyParts[i].transform.localRotation = bodyPartsRot[i];
        }

        //Reactivate player sprite

        yield return new WaitForSeconds(.15f);
        thisAnim.resetPose(false); //Stops playing idle animation (Prevents glitch where player would be stuck on last frame before death!)
        flashLightDropped = null;
        hasFlashlightDropped = false;

        StopCoroutine(startReset());
    }
}

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class CharacterController2D : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;
    [SerializeField] private AnimHook thisAnim;
    [SerializeField] private PlayerDeath playerDeath;
    [Header("Ground and ceiling checks")]
    const float k_GroundedRadius = 1.1f; // Radius of the overlap circle to determine if grounded
    [SerializeField]	private bool m_Grounded = true;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up  
    [SerializeField] private PhysicsMaterial2D slipMaterial;
    [SerializeField] private PhysicsMaterial2D noSlipMaterial;
    [SerializeField] private LayerMask m_WhatIsGround;          // A mask determining what is ground to the character
    [SerializeField] private LayerMask m_WhatIsLedge;
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings                   
    [SerializeField] private List<Collider2D> CollidersToDisable; // A collider that will be disabled when crouching

    [Header("Movement")]   
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = 4F;            // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;

    [SerializeField] private float horizontalMove = 0f;
    [SerializeField] private Vector2 dir;
    [SerializeField] private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    private Vector3 m_Velocity = Vector3.zero;

    [SerializeField] private float currentSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;

    [SerializeField] private float sprintSpeedGain = 20f;
    [SerializeField] private float walkSpeedGain = 20f;
    [SerializeField] private float idleSpeedDrop = 25;

    private bool flipDisabled = false;
    private bool jump = false;
    private bool crouch = false;    
    [SerializeField] private bool m_wasCrouching = false;

    [Header("CoyoteTime")]
    [SerializeField] private float hangTime = .2f;
    private float hangCounter;

    [Header("JumpGenerosity")]
    [SerializeField] private float jumpBufferLength = .1f;
    private float jumpBufferCount;

    [Header("Ladders")]
    [SerializeField] private Transform rayStartPoint;
    [SerializeField] private float climbSpeed;
    private float inputVertical;
    [SerializeField] private float RayDistance;
    public LayerMask whatIsLadder;
    [SerializeField] private bool isClimbing = false;

    [Header("LedgeClimbing")]
    public float ledgeClimbXoffset1 = 0f;
    public float ledgeClimbYoffset1 = 0f;
    public float ledgeClimbXoffset2 = 0f;
    public float ledgeClimbYoffset2 = 0f;

    private Vector2 ledgePosBot;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;
    private bool isTouchingWall;
    private bool isTouchingLedge;
    private bool canClimbLedge = false;
    private bool ledgeDetected;
    [SerializeField] private float wallCheckDistance;
    public Transform wallCheck;
    public Transform ledgeDetector;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool updateDirection = false;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

    private void Start()
    {
        StartCoroutine(waitToUpdateDirection());
    }
 
    private void FixedUpdate()
    {
        if (PlayerInfo.instance.PlayerHasControl)
        {
            groundCheck();

            Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);  //MOVE IS CALLED HERE
        }        
    }

    void Update()
    {
        PlayerInfo.instance.PlayerIsGrounded = m_Grounded;

        if (PlayerInfo.instance.PlayerHasControl)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;

            //Debug.Log("Player has control");
            if (PlayerStates.instance.currentPlayerInteractionState != PlayerInteractionStates.INTERACTING)
            {
                Jump();
                checkLedgeClimb();
                ClimbLadder();
            }

            MovementInput();
            checkSurrounding();          
        }
        else
        {
            //Debug.Log("Player does not have control");
            stopMovement();
            thisAnim.setPlayerSpeed(0);
            rb.bodyType = RigidbodyType2D.Static;
        }    

    }

    #region Standard Movement
    private void MovementInput()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        dir = new Vector2(horizontalMove, 0);
        PlayerInfo.instance.Dir = dir;
        horizontalMove = horizontalMove * currentSpeed;

        if (Input.GetButton("Sprint") && dir.magnitude >= .9f || dir.magnitude <= -.9f) //If player holds the sprint button, increase speed to the run speed amount
        {
            float newSpeed = Mathf.Lerp(currentSpeed, runSpeed, sprintSpeedGain * Time.deltaTime);
            currentSpeed = newSpeed;
        }
        else if (!Input.GetButton("Sprint") && dir.magnitude >= .9f || dir.magnitude <= -.9f) //decrease or increase speed to walk speed amount over time.
        {
            float newSpeed = Mathf.Lerp(currentSpeed, walkSpeed, walkSpeedGain * Time.deltaTime);
            currentSpeed = newSpeed;
        }

        if (dir.magnitude == 0) //If palyer is still, drop to idle speed
        {
            float newSpeed = Mathf.Lerp(currentSpeed, 0, idleSpeedDrop * Time.deltaTime);
            currentSpeed = newSpeed;

            if (currentSpeed <= 0)
            {
                currentSpeed = 0;
            }
        }

        thisAnim.setPlayerSpeed(currentSpeed); //Accesses animator and sets correct values for parameters

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }
    } 

    private void stopMovement()
    {
        currentSpeed = 0;        
    }

    public void Move(float move, bool crouch, bool jump)
	{
        if (PlayerInfo.instance.PlayerHasControl)
        {
            if (updateDirection == true)
            {
                PlayerInfo.instance.FacingRight = m_FacingRight;
            }

            // If crouching, check to see if the character can stand up
            if (!crouch)
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                if (PlayerStates.instance.currentPlayerInteractionState == PlayerInteractionStates.NOTINTERACTING && canClimbLedge == false)
                {
                    // If crouching
                    if (crouch)
                    {
                        if (!m_wasCrouching)
                        {
                            m_wasCrouching = true;
                            OnCrouchEvent.Invoke(true);
                        }

                        // Reduce the speed by the crouchSpeed multiplier
                        move *= m_CrouchSpeed;

                        // Disable one of the colliders when crouching
                        if (CollidersToDisable[0] != null)
                            CollidersToDisable[0].isTrigger = true;
                    }
                    else
                    {
                        // Enable the collider when not crouching
                        if (CollidersToDisable[0] != null)
                            CollidersToDisable[0].isTrigger = false;

                        if (m_wasCrouching)
                        {
                            m_wasCrouching = false;
                            OnCrouchEvent.Invoke(false);
                        }
                    }
                }

                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(move * 10f, rb.velocity.y);
                // And then smoothing it out and applying it to the character
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

                if (PlayerStates.instance.currentPlayerInteractionState != PlayerInteractionStates.INTERACTING)
                {
                    // If the input is moving the player right and the player is facing left...
                    if (move > 0 && !m_FacingRight)
                    {
                        // ... flip the player.
                        Flip();
                    }
                    // Otherwise if the input is moving the player left and the player is facing right...
                    else if (move < 0 && m_FacingRight)
                    {
                        // ... flip the player.
                        Flip();
                    }
                }
            }

            PlayerInfo.instance.IsCrouching = m_wasCrouching;
        }
    }

    void Jump()
    {
        if (PlayerInfo.instance.PlayerHasControl && canClimbLedge == false)
        {
            //Hangtime when jumping off edges of platforms
            if (m_Grounded)
            {
                hangCounter = hangTime;
            }
            else
            {
                hangCounter -= Time.deltaTime;
            }

            //Manager jump buffer
            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferCount = jumpBufferLength;
            }
            else
            {
                jumpBufferCount -= Time.deltaTime;
            }

            //Controllable jumps/ Hold to do a higher jump
            // If the player should jump...
            if (jumpBufferCount >= 0 && hangCounter > 0)
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                thisAnim.setTakeOffTrigger();

                rb.velocity = new Vector2(rb.velocity.x, m_JumpForce);
                jumpBufferCount = 0f;
            }

            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f);
            }
        }
    } 

    private void groundCheck()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;
        jump = true;
        thisAnim.setJumpingState(true);
        thisAnim.setGroundBool(m_Grounded);     

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                jump = false;
                thisAnim.setGroundBool(m_Grounded);
                thisAnim.setJumpingState(false);
                if (!wasGrounded)
                    OnLandEvent.Invoke();

            }
        }         

        if(m_Grounded == false)
        {
            CollidersToDisable[1].sharedMaterial = slipMaterial;
        }
        else
        {
            CollidersToDisable[1].sharedMaterial = noSlipMaterial;
        }
    }
    #endregion

    #region LadderClimbing
    void ClimbLadder()
    {
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hitInfo = Physics2D.Raycast(rayStartPoint.position, Vector2.right * transform.localScale.x, RayDistance, whatIsLadder);
        inputVertical = Input.GetAxisRaw("Vertical");

        if (hitInfo.collider != null)
        {
            //Debug.Log("LadderDetected");
            if (inputVertical != 0)
            {
                isClimbing = true;
            }
        }
        else
        {
            isClimbing = false;
        }

        if (isClimbing && hitInfo.collider != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, inputVertical * climbSpeed);
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 15f;
        }

        PlayerInfo.instance.IsClimbing = isClimbing;
    }

    #endregion

    #region LedgeClimbing
    private void checkLedgeClimb()
    {
        if (ledgeDetected & !canClimbLedge & jump )
        {
            canClimbLedge = true;

            if (m_FacingRight)
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXoffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYoffset1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXoffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYoffset2);

            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXoffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYoffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXoffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYoffset2);
            }

            PlayerInfo.instance.PlayerHasControl = false;
            flipDisabled = true;

            thisAnim.setClimbState(canClimbLedge);

            foreach (Collider2D collider in CollidersToDisable)
            {
                collider.isTrigger = true;
            }
        }
        if (canClimbLedge)
        {
            transform.position = ledgePos1;
        }
    }

    public void FinishLedgeClimb()
    {
        Debug.Log("Finishing Ledge climb");
        canClimbLedge = false;
        transform.position = ledgePos2;
        PlayerInfo.instance.PlayerHasControl = true;
        flipDisabled = false;
        ledgeDetected = false;

        foreach (Collider2D collider in CollidersToDisable)
        {
            collider.isTrigger = false;
        }

        thisAnim.setClimbState(canClimbLedge);
    }

    private void checkSurrounding()
    {
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right * transform.localScale.x, wallCheckDistance, m_WhatIsLedge);
        isTouchingLedge = Physics2D.Raycast(ledgeDetector.position, transform.right * transform.localScale.x, wallCheckDistance, m_WhatIsLedge);

        if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
    }
    #endregion

    private void Flip()
	{
        if (flipDisabled == false)
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
	}

    public void resetPlayerAtLastCheckpoint(Checkpoint lastCheckpointHit)
    {
        transform.position = lastCheckpointHit.transform.position;
    }

    private IEnumerator waitToUpdateDirection()
    {
        yield return new WaitForSeconds(2f);
        updateDirection = true;

        StopCoroutine(waitToUpdateDirection());

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(wallCheck.position, transform.right * wallCheckDistance);
        Gizmos.DrawRay(ledgeDetector.position, transform.right * wallCheckDistance);        
    }
}

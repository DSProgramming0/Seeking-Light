using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;
    [SerializeField] private AnimHook thisAnim; 

    [Header("Ground and ceiling checks")]
    const float k_GroundedRadius = 1.1f; // Radius of the overlap circle to determine if grounded
    [SerializeField]	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up  
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

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
		bool wasGrounded = m_Grounded;
		m_Grounded = false;
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
                thisAnim.setGroundBool(m_Grounded);
                thisAnim.setJumpingState(false);
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}

        Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;

        ClimbLadder();        
    }

    void Update()
    {
        if (PlayerStates.instance.currentPlayerInteractionState != PlayerInteractionStates.INTERACTING)
        {
            Jump();
        }

        MovementInput();
    }

    private void MovementInput()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        dir = new Vector2(horizontalMove, 0);
        PlayerInfo.instance.Dir = dir;
        horizontalMove = horizontalMove * currentSpeed;

        if (Input.GetButton("Sprint")) //If player holds the sprint buttong, increase speed to the run speed amount
        {
            float newSpeed = Mathf.Lerp(currentSpeed, runSpeed, sprintSpeedGain * Time.deltaTime);
            currentSpeed = newSpeed;
        }
        else if (!Input.GetButton("Sprint") && dir.magnitude != 0) //decrease or increase speed to walk speed amount over time.
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

    public void Move(float move, bool crouch, bool jump)
	{
        if(updateDirection == true)
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
            if(PlayerStates.instance.currentPlayerInteractionState == PlayerInteractionStates.NOTINTERACTING)
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
                    if (m_CrouchDisableCollider != null)
                        m_CrouchDisableCollider.isTrigger = true;
                }
                else
                {
                    // Enable the collider when not crouching
                    if (m_CrouchDisableCollider != null)
                        m_CrouchDisableCollider.isTrigger = false;

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

            if(PlayerStates.instance.currentPlayerInteractionState != PlayerInteractionStates.INTERACTING)
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

    void Jump()
    {
        //Hangtime when jumping off edges of platforms
        if(m_Grounded)
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

    void ClimbLadder()
    {
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hitInfo = Physics2D.Raycast(rayStartPoint.position, Vector2.right * transform.localScale.x, RayDistance, whatIsLadder);
        
        if(hitInfo.collider != null)
        {
            //Debug.Log("LadderDetected");
            if (Input.GetKeyDown(KeyCode.W))
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
            inputVertical = Input.GetAxisRaw("Vertical");
            rb.velocity = new Vector2(rb.velocity.x, inputVertical * climbSpeed);
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 15f;
        }

        PlayerInfo.instance.IsClimbing = isClimbing;
    }   
    
    private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
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

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(rayStartPoint.position, Vector2.right * RayDistance);
    //}
}

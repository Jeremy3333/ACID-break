using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class CharacterHandler : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField]
    InputActionAsset inputActions;
    InputAction m_dashAction;
    InputAction m_jumpAction;
    InputAction m_moveAction;
    InputAction m_lookAction;
    Vector2 lookInput;
    float moveInput;

    [SerializeField]
    Transform cameraTransform;

    [SerializeField]
    GameObject rotationPoint;

    Grappling grappling;

    Swording swording;

    Rigidbody2D rb;

    BoxCollider2D bc;

    [SerializeField]
    float moveSpeed = 20f;


    [SerializeField]
    float acceleration = 7f;

    [SerializeField]
    float decceleration = 7f;

    [SerializeField]
    float velPow = 0.9f;

    [SerializeField]
    float jumpForce = 15f;

    [SerializeField]
    float dashVelocityAmplitude = 25.0f;
    [SerializeField]
    float dashCooldown = 1.0f;

    [SerializeField]
    float gravityScale = 2.5f;
    [SerializeField]
    float GravityMultiplier = 2.3f;

    float moveDir = 0.0f;

    bool isFacingRight = true;

    LayerMask collisionLayer;
    // For jumping
    [SerializeField] LayerMask groundLayers;
    [SerializeField] LayerMask screenLayers;

    bool touchGround;

    bool doubleJumpReady;
    // For dashing
    float dashCooldownTimer;
    Vector2 dashDirection;

    bool isDashing;
    bool leftStop;
    bool rightStop;

    bool canDash = true;

    [SerializeField] float dashMaxTimer = 0.2f;
    [SerializeField] float GrappleDashMaxTimer = 0.5f;
    float dashTimer;

    bool grappleDashReady = true;

    [SerializeField] float grappleDashForce = 20f;

    bool isGrapple = false;

    PlayerAnimator playerAnimator;
    HealthManager healthManager;

    LayerMask playerLayer;
    LayerMask UntangiblePlayerLayer;

    private bool playingFootStep = false;
    public float footStepSpeed = 0.3f;

    AudioManager audioManager;

    void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }
    void onDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }
    void Awake()
    {
        m_jumpAction = inputActions.FindAction("Jump");
        m_dashAction = inputActions.FindAction("Dash");
        m_moveAction = inputActions.FindAction("Move");
        m_lookAction = inputActions.FindAction("Look");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grappling = rotationPoint.GetComponent<Grappling>();
        swording = GetComponentInChildren<Swording>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        playerAnimator = GetComponent<PlayerAnimator>();

        playerLayer = LayerMask.NameToLayer("Player");
        UntangiblePlayerLayer = LayerMask.NameToLayer("PlayerUntangible");
        healthManager = GetComponent<HealthManager>();
        
        audioManager = GetComponentInChildren<AudioManager>();
    }

    public void Jump()
    {
        if (!touchGround && !doubleJumpReady) return;
        
        if(!touchGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            audioManager.PlayClipRand(audioManager.jump);
            doubleJumpReady = false;
            return;
        }
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

        audioManager.PlayClipRand(audioManager.jump);
    }

    void GroundDash()
    {
        if (Time.time > dashCooldownTimer && !isDashing)
        {
            dashCooldownTimer = Time.time + dashCooldown;
            isDashing = true;
            dashTimer = dashMaxTimer;
            dashDirection = new Vector2(isFacingRight ? 1f : -1f, 0f).normalized;

            // Reset velocity and apply constant dash speed
            rb.linearVelocity = dashDirection * dashVelocityAmplitude;
            rb.gravityScale = 0f; // Disable gravity during dash

            playerAnimator.Dash(true, !isFacingRight); // Trigger dash animation
            
            becomeUntangible();
            audioManager.PlayClipRand(audioManager.jump);
        }
    }

    // when starting a grapple, the player linear velocity should be perpendicular to the direction of the grapple point. in the facing direction
    void startGrapple()
    {
        if(touchGround) return; // only apply the grapple velocity if we're in the air, otherwise the player will be launched in the opposite direction of the grapple point
        Vector2 grappleDir = (Vector2)grappling.getGrappleProjectilePosition();
        if (grappleDir == null) return;

        grappleDir = (grappleDir - rb.position).normalized;
        Vector2 perpendicularDir = new Vector2(-grappleDir.y, grappleDir.x); // 90° counter‑clockwise
        if (Vector2.Dot(perpendicularDir, rb.linearVelocity) < 0)
            perpendicularDir = -perpendicularDir;

        rb.linearVelocity = perpendicularDir * moveSpeed * 1.2f; // Set initial velocity for smooth swinging
    }

    void GrappleDash()
    {
        if (grappleDashReady)
        {
            grappleDashReady = false;

            // Direction from player to grapple point
            Vector2 grappleDir = ((Vector2)grappling.getGrappleProjectilePosition() - rb.position).normalized;
            
            Vector2 perpendicularDir = new Vector2(-grappleDir.y, grappleDir.x);
            float mo = m_moveAction.ReadValue<float>();

            if (
                (mo == 0 && Vector2.Dot(perpendicularDir, rb.linearVelocity) < 0) ||
                (mo > 0 && grappleDir.y > 0) ||
                (mo < 0 && grappleDir.y < 0)
            )
                perpendicularDir = -perpendicularDir;
            

            dashTimer = GrappleDashMaxTimer;

            rb.linearVelocity = Vector2.zero; // Reset current velocity before applying dash force
            rb.AddForce(perpendicularDir * grappleDashForce, ForceMode2D.Impulse);

            audioManager.PlayClipRand(audioManager.jump);
        }
    }

    void Dash(Vector2 direction)
    {
        Vector2 desiredDash = new Vector2(
            direction.x * dashVelocityAmplitude,
            Mathf.Max(0, rb.linearVelocity.y)
        );

        isDashing = true;
        dashDirection = direction.normalized;
        rb.linearVelocity = dashDirection * dashVelocityAmplitude;
        rb.gravityScale = 0f;

        playerAnimator.Dash(true, !isFacingRight);
    }

    void UpdateValues()
    {
        touchGround = CheckGround();
        if(touchGround)
            doubleJumpReady = true;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
    }

    void StartFootSteps()
    {
        playingFootStep = true;
        InvokeRepeating(nameof(PlayFootStep), 0f, footStepSpeed);
    }

    void StopFootSteps()
    {
        playingFootStep = false;
        CancelInvoke(nameof(PlayFootStep));
    }
    
    void PlayFootStep()
    {
        audioManager.PlayClipRand(audioManager.walk);
    }

    void CheckFootSteps()
    {
        if(rb.linearVelocity.magnitude > 1f && touchGround && !playingFootStep)
        {
            StartFootSteps();
        }
        else if(rb.linearVelocity.magnitude < 1f || !touchGround)
        {
            StopFootSteps();
        }
    }

    void UpdateMovement()
    {

        if (isDashing) return; 
        if(isGrapple)
        {
            if (m_dashAction.WasPressedThisFrame() && !touchGround && canDash)
            {
                GrappleDash();
            }
            return;
        }
        
        // Direction gauche/droite
        moveInput = m_moveAction.ReadValue<float>();
        moveDir = moveInput;
        if(moveDir > 0 && !isFacingRight)
        {
            Flip();
        }
        else if(moveDir < 0 && isFacingRight)
        {
            Flip();
        }

        if(leftStop && moveDir > 0)
        {
            moveDir = 0;
        } else if(rightStop && moveDir < 0)
        {
            moveDir = 0;
        }
        
        if (m_jumpAction.WasPressedThisFrame())
            Jump();

        if (m_dashAction.WasPressedThisFrame() && canDash)
            GroundDash();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateValues();

        UpdateMovement();

        CheckFootSteps();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // ===== DASH STATE =====
        if (isDashing)
        {
            // Maintain constant velocity
            rb.linearVelocity = dashDirection * dashVelocityAmplitude;
            rb.gravityScale = 0f;

            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                playerAnimator.Dash(false, !isFacingRight);
                becomeTangible();
                // Gravity will be restored in the normal flow next frame
            }
            return; // Skip normal movement & gravity
        }

        // ===== MOUVEMENT =====
        float targetSpeed = moveDir * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;

        float accelerationRate;
        if (Mathf.Abs(targetSpeed) > 0.01f)
            accelerationRate = acceleration;
        else
            accelerationRate = isGrapple ? 0f : decceleration;

        float movement =
            Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, velPow)
            * Mathf.Sign(speedDiff);

        rb.AddForce(movement * Vector2.right);

        // ===== GRAVITÉ =====
        if (rb.linearVelocity.y < 0 && !isGrapple)
            rb.gravityScale = gravityScale * GravityMultiplier;
        else
            rb.gravityScale = gravityScale;
    }


    public bool CheckGround()
    {
        Vector2 bottomCenter = (Vector2)transform.position + Vector2.down * bc.bounds.extents.y;
        float skinWidth = 0.01f;
        Vector2 origin = bottomCenter + Vector2.down * skinWidth;
        Vector2 boxSize = new Vector2(bc.bounds.size.x * 0.9f, 0.02f);
        float distance = 0.1f;

        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, distance, groundLayers);

        // Optional: check surface angle to avoid sticking to steep walls
        if (hit)
        {
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            if (angle < 45f) // adjust slope limit as needed
                return true;
        }
        return false;
    }

    public void setIsGrapple(bool p_isGrapple)
    {
        isGrapple = p_isGrapple;
        grappleDashReady = true;
        moveDir = 0f;
        if (isGrapple)
        {
            startGrapple();
        } else
        {
            leftStop = false;
            rightStop = false;
        }
    }

    public void setCanDash(bool p_canDash)
    {
        canDash = p_canDash;
    }

    public void moveToDirection(float len, Vector2 pos)
    {
        Vector2 dir = (pos - (Vector2)transform.position).normalized;
        rb.position += dir * len;
    }

    public BoxCollider2D getScreenCollider()
    {
        // if he doesn't collide with a screen collider return null use screenLayers to only check screen colliders
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, bc.bounds.size, 0f, screenLayers);
        foreach (Collider2D hit in hits)        {
            return hit as BoxCollider2D;
        }
        return null;   
    }

    public void becomeUntangible()
    {
        gameObject.layer = UntangiblePlayerLayer;
        healthManager.setIsHittable(false);
    }

    public void becomeTangible()
    {
        gameObject.layer = playerLayer;
        healthManager.setIsHittable(true);
    }

    public void stopMovementNotInDirection(Vector2 dir)
    {
        Vector2 velocity = rb.linearVelocity;
        // use leftStop and rightStop to only stop the velocity in one direction, if the player is moving in the opposite direction of the dash, don't stop the velocity
        if (dir.x > 0 && !rightStop)
        {
            velocity.x = Mathf.Max(0, velocity.x);
            rightStop = true;
        }
        else if (dir.x < 0 && !leftStop)
        {
            velocity.x = Mathf.Min(0, velocity.x);
            leftStop = true;
        }
        rb.linearVelocity = velocity;
    }
}

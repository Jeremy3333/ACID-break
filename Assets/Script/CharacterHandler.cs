using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections.Generic;

public enum CharacterMode
{
    Neutral,
    Jetpack,
    BigGun,
    BaseballBat,
    Lasso
}

public class CharacterHandler : MonoBehaviour
{
    [SerializeField]
    Transform cameraTransform;

    [SerializeField]
    GameObject rotationPoint;

    Grappling grappling;

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
    float doubleJumpForce = 15f;

    [SerializeField]
    float dashForce = 25.0f;
    [SerializeField]
    float dashCooldown = 1.0f;

    [SerializeField]
    float gravityScale = 2.5f;
    [SerializeField]
    float GravityMultiplier = 2.3f;

    Keyboard keyboard;

    float moveDir = 0.0f;

    LayerMask collisionLayer;
    // For jumping
    [SerializeField] LayerMask groundLayers;

    bool touchGround;

    bool doubleJumpReady;
    // For dashing
    float dashCooldownTimer;
    Vector2 dashDirection;

    bool isDashing;

    [SerializeField] float dashMaxTimer = 0.2f;
    float dashTimer;

    bool grappleDashReady = true;

    bool isGrapple = false;

    PlayerAnimator playerAnimator;

    [SerializeField]
    UIDocument uiDocument;

    Dictionary<string, VisualElement> modeIcons = new Dictionary<string, VisualElement>();

    Vector2 modeSelectionStartPos;

    CharacterMode currentMode = CharacterMode.BigGun;

    [SerializeField]
    CombatModeManager modeManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keyboard = Keyboard.current;
        if(keyboard == null)
            Debug.LogError("No kayboard detected");
        grappling = rotationPoint.GetComponent<Grappling>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        playerAnimator = GetComponent<PlayerAnimator>();

        if (uiDocument != null)
        {
            // Assuming you have VisualElements with these names in your UI Document
            modeIcons["Jetpack"] = uiDocument.rootVisualElement.Q<VisualElement>("JetPack");
            modeIcons["BigGun"] = uiDocument.rootVisualElement.Q<VisualElement>("BigGun");
            modeIcons["BaseballBat"] = uiDocument.rootVisualElement.Q<VisualElement>("BaseballBat");
            modeIcons["Lasso"] = uiDocument.rootVisualElement.Q<VisualElement>("Lasso");
        }
        else
        {
            Debug.LogError("UI Document is not assigned.");
        }
    }

    void Jump()
    {
        if (!touchGround && !doubleJumpReady) return;
        if (!touchGround) // If not grounded, use up the double jump
        {
            doubleJumpReady = false;

            // Stronger jump at a more controllable angle
            Vector2 desiredJump = new Vector2(moveDir, 1);
            desiredJump = desiredJump * doubleJumpForce;
            // Also cancel current force
            rb.AddForce(desiredJump - rb.linearVelocity, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

    }

    void GroundDash()
    {
        if (Time.time > dashCooldownTimer && moveDir != 0 && !isDashing)
        {
            dashCooldownTimer = Time.time + dashCooldown;
            isDashing = true;
            dashTimer = dashMaxTimer;
            dashDirection = new Vector2(moveDir, 0f).normalized;

            // Reset velocity and apply constant dash speed
            rb.linearVelocity = dashDirection * dashForce;
            rb.gravityScale = 0f; // Disable gravity during dash

            playerAnimator.Dash(); // Trigger dash animation
        }
    }

    void GrappleDash()
    {
        if (grappleDashReady)
        {
            grappleDashReady = false;

            // Dash tangentially to the grapple direction facing the direction of the the current velocity
            Vector2 grappleDir = ((Vector2)grappling.getGrappleProjectilePosition() - rb.position).normalized;
            Vector2 tangentDir = new Vector2(-grappleDir.y, grappleDir.x);
            if (Vector2.Dot(tangentDir, rb.linearVelocity) < 0)
                tangentDir = -tangentDir;
            Dash(tangentDir);
        }
    }

    void Dash(Vector2 direction)
    {
        Vector2 desiredDash = new Vector2(
            direction.x * dashForce,
            Mathf.Max(0, rb.linearVelocity.y)
        );

        rb.AddForce(desiredDash - rb.linearVelocity, ForceMode2D.Impulse);

        playerAnimator.Dash();
    }

    void updateMode()
    {
        Vector2 currentMousePos = Mouse.current.position.ReadValue();
        Vector2 delta = currentMousePos - modeSelectionStartPos;

        if (delta.magnitude < 50f)
            return;

        if (delta.x > 0 && delta.y > 0)
        {
            currentMode = CharacterMode.BigGun;
        }
        else if (delta.x <= 0 && delta.y > 0)
        {
            currentMode = CharacterMode.Jetpack;
        }
        else if (delta.x <= 0 && delta.y <= 0)
        {
            currentMode = CharacterMode.BaseballBat;
        }
        else if (delta.x > 0 && delta.y <= 0)
        {
            currentMode = CharacterMode.Lasso;
        }
    }

    void updateUI()
    {
        switch (currentMode)
        {
            case CharacterMode.Neutral:
                modeIcons["Jetpack"].AddToClassList("Unselected");
                modeIcons["BigGun"].AddToClassList("Unselected");
                modeIcons["BaseballBat"].AddToClassList("Unselected");
                modeIcons["Lasso"].AddToClassList("Unselected");
                break;
            case CharacterMode.Jetpack:
                modeIcons["Jetpack"].RemoveFromClassList("Unselected");
                modeIcons["BigGun"].AddToClassList("Unselected");
                modeIcons["BaseballBat"].AddToClassList("Unselected");
                modeIcons["Lasso"].AddToClassList("Unselected");
                break;
            case CharacterMode.BigGun:
                modeIcons["Jetpack"].AddToClassList("Unselected");
                modeIcons["BigGun"].RemoveFromClassList("Unselected");
                modeIcons["BaseballBat"].AddToClassList("Unselected");
                modeIcons["Lasso"].AddToClassList("Unselected");
                break;
            case CharacterMode.BaseballBat:
                modeIcons["Jetpack"].AddToClassList("Unselected");
                modeIcons["BigGun"].AddToClassList("Unselected");
                modeIcons["BaseballBat"].RemoveFromClassList("Unselected");
                modeIcons["Lasso"].AddToClassList("Unselected");
                break;
            case CharacterMode.Lasso:
                modeIcons["Jetpack"].AddToClassList("Unselected");
                modeIcons["BigGun"].AddToClassList("Unselected");
                modeIcons["BaseballBat"].AddToClassList("Unselected");
                modeIcons["Lasso"].RemoveFromClassList("Unselected");
                break;
        }
    }

    void ApplySelectedMode()
    {
        switch (currentMode)
        {
            case CharacterMode.BaseballBat:
                modeManager.SwitchMode(modeManager.swordMode as ICombatMode);
                break;
            case CharacterMode.BigGun:
                modeManager.SwitchMode(modeManager.gunMode as ICombatMode);
                break;
            case CharacterMode.Lasso:
                modeManager.SwitchMode(modeManager.grapplingGunMode as ICombatMode);
                break;
        }
    }

    void ModeSwitch()
    {
        if (keyboard.tabKey.wasPressedThisFrame)
        {
            // Afficher l'interface
            uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
            // Stocker la position initiale de la souris
            modeSelectionStartPos = Mouse.current.position.ReadValue();
        }
        
        if (keyboard.tabKey.isPressed)
        {
            updateMode();
            updateUI();
        }
        
        if (keyboard.tabKey.wasReleasedThisFrame)
        {
            ApplySelectedMode();
            uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        }
    }

    void UpdateValues()
    {
        touchGround = CheckGround();
        if(touchGround)
            doubleJumpReady = true;
    }

    void UpdateMovement()
    {
        if (isDashing) return; 
        if(isGrapple)
        {
            if (keyboard.leftShiftKey.wasPressedThisFrame)
            {
                GrappleDash();
            }
            return;
        }
        // Direction gauche/droite
        moveDir = 0f;
        if (keyboard.aKey.isPressed) moveDir -= 1f;
        if (keyboard.dKey.isPressed) moveDir += 1f;

        if (keyboard.spaceKey.wasPressedThisFrame)
            Jump();

        if (keyboard.leftShiftKey.wasPressedThisFrame)
            GroundDash();
    }

    // Update is called once per frame
    void Update()
    {

        UpdateValues();

        UpdateMovement();

        ModeSwitch();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // ===== DASH STATE =====
        if (isDashing)
        {
            // Maintain constant velocity
            rb.linearVelocity = dashDirection * dashForce;
            rb.gravityScale = 0f;

            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
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
        else if (!isGrapple)
            rb.gravityScale = gravityScale;
    }


    bool CheckGround()
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
    }

    public void moveToDirection(float len, Vector2 pos)
    {
        Vector2 dir = (pos - (Vector2)transform.position).normalized;
        rb.position += dir * len;
    }
}

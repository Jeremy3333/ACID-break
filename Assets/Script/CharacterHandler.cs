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

    Rigidbody2D character;

    [SerializeField]
    float moveSpeed = 10f;


    [SerializeField]
    float acceleration = 7f;

    [SerializeField]
    float decceleration = 7f;

    [SerializeField]
    float velPow = 0.9f;

    [SerializeField]
    float jumpForce = 13.5f;
    [SerializeField]
    float doubleJumpForce = 18f;

    [SerializeField]
    float dashForce = 25.0f;
    [SerializeField]
    float dashCooldown = 1.0f;

    [SerializeField]
    float gravityScale = 1f;
    [SerializeField]
    float GravityMultiplier = 2f;

    Keyboard keyboard;

    float moveDir = 0.0f;

    public Transform groundCheck;
    public float radius;
    public LayerMask collisionLayer;
    // For jumping
    public bool isGrounded;
    public bool doubleJumpReady;
    // For dashing
    public float cantDashUntil;

    public SpringJoint2D joint;

    private PlayerAnimator playerAnimator;

    [SerializeField]
    private UIDocument uiDocument;

    Dictionary<string, VisualElement> modeIcons = new Dictionary<string, VisualElement>();

    Vector2 modeSelectionStartPos;

    CharacterMode currentMode = CharacterMode.BigGun;

    [SerializeField]
    CombatModeManager modeManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keyboard = Keyboard.current;
        character = GetComponent<Rigidbody2D>();
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
        if ((!isGrounded && !doubleJumpReady) || character == null) return;
        if (!isGrounded && doubleJumpReady) // If not grounded, use up the double jump
        {
            doubleJumpReady = false;

            // Stronger jump at a more controllable angle
            Vector2 desiredJump = new Vector2(moveDir, 1);
            desiredJump = desiredJump * doubleJumpForce;
            // Also cancel current force
            character.AddForce(desiredJump - character.linearVelocity, ForceMode2D.Impulse);
        }
        else
        {
            character.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

    }

    void Dash()
    {
        if (Time.time > cantDashUntil && moveDir != 0)
        {
            cantDashUntil = Time.time + dashCooldown;
            Vector2 desiredDash =
                new Vector2(moveDir * dashForce, Mathf.Max(0, character.linearVelocity.y));
            character.AddForce(desiredDash - character.linearVelocity, ForceMode2D.Impulse);

            playerAnimator.Dash();
        }
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

    void ModeSwitch()
    {
        if (keyboard.tabKey.wasPressedThisFrame)
        {
            // Display the UI
            uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
            // Store the initial mouse position when the tab key is pressed
            modeSelectionStartPos = Mouse.current.position.ReadValue();
        }
        if (keyboard.tabKey.isPressed)
        {
            updateMode();
            // Update the UI based on the current mode
            updateUI();
        }
        if (keyboard.tabKey.wasReleasedThisFrame)
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
            // Hide the UI
            uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();

        if (keyboard == null)
        {
            Debug.LogError("Keyboard input is not available.");
            return;
        }

        // Direction gauche/droite
        moveDir = 0f;
        if (keyboard.aKey.isPressed) moveDir -= 1f;
        if (keyboard.dKey.isPressed) moveDir += 1f;

        if (keyboard.spaceKey.wasPressedThisFrame)
            Jump();

        if (keyboard.leftShiftKey.isPressed)
            Dash();
    }

    void FixedUpdate()
    {
        if (character == null) return;

        // ===== MOUVEMENT =====
        float targetSpeed = moveDir * moveSpeed;
        float speedDiff = targetSpeed - character.linearVelocity.x;

        float accelerationRate;
        if (Mathf.Abs(targetSpeed) > 0.01f)
            accelerationRate = acceleration;
        else
            accelerationRate = joint.enabled ? 0f : decceleration;

        float movement =
            Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, velPow)
            * Mathf.Sign(speedDiff);

        character.AddForce(movement * Vector2.right);

        // ===== GRAVITÉ =====
        if (character.linearVelocity.y < 0)
            character.gravityScale = gravityScale * GravityMultiplier;
        else
            character.gravityScale = gravityScale;
    }


    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            radius,
            collisionLayer
        );
        if (!doubleJumpReady && isGrounded)
        {
            doubleJumpReady = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, radius);
    }
}

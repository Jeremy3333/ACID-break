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

public class CharacterHandler1 : MonoBehaviour
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
    float jumpForce = 300f;

    [SerializeField]
    float gravityScale = 1f;
    [SerializeField]
    float GravityMultiplier = 2f;
    
    Keyboard keyboard;

    const float epsilon = 0.01f;

    float moveDir = 0.0f;

    public Transform groundCheck;
    public float radius;
    public LayerMask collisionLayer;
    public bool isGrounded;
    
    public SpringJoint2D joint;

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
        if (!isGrounded || character == null) return;
        
        character.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    void updateMode()
    {
        Vector2 currentMousePos = Mouse.current.position.ReadValue();
        Vector2 delta = currentMousePos - modeSelectionStartPos;

        if (delta.magnitude < 50f)
            return;
        
        if(delta.x > 0 && delta.y > 0)
        {
            currentMode = CharacterMode.BigGun;
        }
        else if(delta.x <= 0 && delta.y > 0)
        {
            currentMode = CharacterMode.Jetpack;
        }
        else if(delta.x <= 0 && delta.y <= 0)
        {
            currentMode = CharacterMode.BaseballBat;
        }
        else if(delta.x > 0 && delta.y <= 0)
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
            Debug.LogError("(Custom error) Keyboard input is not available.");
            return;
        }

        moveDir = 0f;
        if (keyboard.aKey.isPressed) moveDir -= 1f;
        if (keyboard.dKey.isPressed) moveDir += 1f;
        
        if (keyboard.spaceKey.wasPressedThisFrame)
            Jump();
        
        ModeSwitch();
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
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, radius);
    }
}

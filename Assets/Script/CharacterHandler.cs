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
    float moveSpeed = 5f;
    
    [SerializeField]
    float acceleration = 30f;

    [SerializeField]
    private UIDocument uiDocument;

    Dictionary<string, VisualElement> modeIcons = new Dictionary<string, VisualElement>();

    Vector2 modeSelectionStartPos;
    
    Rigidbody2D character;
    
    Keyboard keyboard;

    CharacterMode currentMode = CharacterMode.Neutral;

    float targetVelocityX = 0f;

    float epsilon = 0.01f;

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
        if (character == null) return;

        if(Mathf.Abs(character.linearVelocity.y) < epsilon)
        {
            character.AddForce(new Vector2(0, 300f));
        }
    }

    void Movement()
    {
        float moveDir = 0f;
        if (keyboard.aKey.isPressed) moveDir -= 1f;
        if (keyboard.dKey.isPressed) moveDir += 1f;
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }

        targetVelocityX = moveDir * moveSpeed;
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
            // Hide the UI
            uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (keyboard == null)
        {
            Debug.LogError("Keyboard input is not available.");
            return;
        }

        ModeSwitch();
        Movement();
    }

    void FixedUpdate()
    {
        if (character == null) return;

        float newVx = Mathf.MoveTowards(character.linearVelocity.x, targetVelocityX, acceleration * Time.fixedDeltaTime);
        character.linearVelocity = new Vector2(newVx, character.linearVelocity.y);
    }
}
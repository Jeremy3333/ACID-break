using UnityEngine;
using UnityEngine.InputSystem; // Add this namespace

public class CharacterHandler : MonoBehaviour
{
    [SerializeField]
    Transform cameraTransform;
    
    Rigidbody2D character;
    
    [SerializeField]
    float moveSpeed = 5f;
    
    [SerializeField]
    float acceleration = 30f;
    
    Keyboard keyboard;

    float targetVelocityX = 0f;

    float epsilon = 0.01f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keyboard = Keyboard.current;
        character = GetComponent<Rigidbody2D>();
    }

    void jump()
    {
        if (character == null) return;

        if(Mathf.Abs(character.linearVelocity.y) < epsilon)
        {
            character.AddForce(new Vector2(0, 300f));
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

        float moveDir = 0f;
        if (keyboard.aKey.isPressed) moveDir -= 1f;
        if (keyboard.dKey.isPressed) moveDir += 1f;
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            jump();
        }

        targetVelocityX = moveDir * moveSpeed;
    }

    void FixedUpdate()
    {
        if (character == null) return;

        float newVx = Mathf.MoveTowards(character.linearVelocity.x, targetVelocityX, acceleration * Time.fixedDeltaTime);
        character.linearVelocity = new Vector2(newVx, character.linearVelocity.y);
    }
}
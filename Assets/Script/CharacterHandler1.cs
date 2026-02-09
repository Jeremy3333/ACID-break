using UnityEngine;
using UnityEngine.InputSystem; // Add this namespace

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

    float epsilon = 0.01f;

    float moveDir = 0.0f;

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
            character.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
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

        moveDir = 0f;
        if (keyboard.aKey.isPressed) moveDir -= 1f;
        if (keyboard.dKey.isPressed) moveDir += 1f;
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            jump();
        }
    }

    void FixedUpdate()
    {
        if (character == null) return;
        #region Run
        float targetSpeed = moveDir * moveSpeed;

        float speedDiff = targetSpeed - character.linearVelocity.x;
        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f)? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelerationRate, velPow) * Mathf.Sign(speedDiff);
        character.AddForce(movement * Vector2.right);
        #endregion
        #region Jump Gravity
        if(character.linearVelocity.y < 0)
        {
            character.gravityScale = gravityScale * GravityMultiplier;
        }
        else
        {
            character.gravityScale = gravityScale;
        }
        #endregion

    }
}
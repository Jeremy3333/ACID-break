using UnityEngine;

public class EnemieHandler : MonoBehaviour
{
    [SerializeField]
    Transform playerTransform;

    [SerializeField]
    float acceleration = 20f;

    float targetVelocityX = 0f;

    Rigidbody2D enemie;

    void Start()
    {
        enemie = GetComponent<Rigidbody2D>();
    }

    void MoveTowardsPlayer()
    {
        if (playerTransform != null)
        {
            float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
            targetVelocityX = direction * 2f; // Move towards the player at a speed of 2 units per second
        }
        else
        {
            targetVelocityX = 0f; // Stop moving if the player is not assigned
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer();
    }
    void FixedUpdate()
    {
        float newVx = Mathf.MoveTowards(enemie.linearVelocity.x, targetVelocityX, acceleration * Time.fixedDeltaTime);
        enemie.linearVelocity = new Vector2(newVx, enemie.linearVelocity.y);
    }
}

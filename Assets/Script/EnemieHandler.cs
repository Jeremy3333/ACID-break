using UnityEngine;

public enum EnemieType
{
    Basic,
    Gunner
}

public class EnemieHandler : MonoBehaviour
{
    [SerializeField]
    Transform playerTransform;

    float acceleration;

    float speed;

    [SerializeField]
    EnemieType enemieType;

    float targetVelocityX = 0f;

    Rigidbody2D enemieRB;

    BoxCollider2D enemieCollider;

    HealthManagerMob healthManagerMob;

    bool isFrozen = false;

    void Start()
    {
        enemieRB = GetComponent<Rigidbody2D>();
        enemieCollider = GetComponent<BoxCollider2D>();
        healthManagerMob = GetComponent<HealthManagerMob>();
        if (enemieRB == null)
        {
            Debug.LogError("Rigidbody2D component is missing on the enemie.");
        }
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned.");
        }
        switch (enemieType)
        {
            case EnemieType.Basic:
                acceleration = 25f;
                speed = 3f;
                break;
            case EnemieType.Gunner:
                acceleration = 20f;
                speed = 2f;
                break;
            default:
                Debug.LogError("Unknown EnemieType.");
                targetVelocityX = 0f;
                break;
        }
    }

    void MoveTowardsPlayer(bool moveTowards)
    {
        if (moveTowards)
        {
            targetVelocityX = (playerTransform.position.x > transform.position.x) ? speed : -speed;
        }
        else
        {
            targetVelocityX = (playerTransform.position.x > transform.position.x) ? -speed : speed;
        }
    }

    void BasicBehavior()
    {
        MoveTowardsPlayer(true);
    }

    void GunnerBehavior()
    {
        Vector2 directionToPlayer = playerTransform.position - transform.position;
        if(directionToPlayer.magnitude > 6f)
        {
            MoveTowardsPlayer(true);
        }
        else if(directionToPlayer.magnitude < 4f)
        {
            MoveTowardsPlayer(false);
        }
    }

    void ChooseBehavior()
    {
        switch (enemieType)
        {
            case EnemieType.Basic:
                BasicBehavior();
                break;
            case EnemieType.Gunner:
                GunnerBehavior();
                break;
            default:
                Debug.LogError("Unknown EnemieType.");
                break;
        }
    }

    public void setFrozen(bool frozen)
    {
        isFrozen = frozen;
        if (isFrozen)
        {
            targetVelocityX = 0f;
            enemieRB.bodyType = RigidbodyType2D.Kinematic;
            enemieRB.linearVelocity = Vector2.zero;
            enemieCollider.enabled = false;
        }
        else
        {
            enemieRB.bodyType = RigidbodyType2D.Dynamic;
            enemieCollider.enabled = true;
        }
    }

    public void Hit(int damage)
    {
        healthManagerMob.Hit(damage);
    }

    // Update is called once per frame
    void Update()
    {
        targetVelocityX = 0f; // Reset target velocity at the start of each frame
        if (!isFrozen)
            ChooseBehavior();
    }
    void FixedUpdate()
    {
        float newVx = Mathf.MoveTowards(enemieRB.linearVelocity.x, targetVelocityX, acceleration * Time.fixedDeltaTime);
        enemieRB.linearVelocity = new Vector2(newVx, enemieRB.linearVelocity.y);
    }
}
using UnityEngine;

public enum EnemieType
{
    Basic,
    Gunner
}

public class EnemieHandler : MonoBehaviour
{
    Transform playerTransform;

    [SerializeField] float acceleration;

    [SerializeField] float speed;
    
    [SerializeField]
    LayerMask groundLayers;
    public bool touchGround;
    public bool touchGroundfront;

    [SerializeField]
    EnemieType enemieType;

    float targetVelocityX = 0f;

    Rigidbody2D enemieRB;

    BoxCollider2D enemieCollider;

    HealthManagerMob healthManagerMob;

    public bool isFrozen = false;
    
    [Header("Gravity Settings")]
    [SerializeField] float gravityScale = 2.5f;
    [SerializeField] float gravityMultiplier = 2.3f;

    [Header("Searching Settings")]
    [SerializeField] float minMarche = 1f;
    [SerializeField] float maxMarche = 3f;
    [SerializeField] float minPause = 0.5f;
    [SerializeField] float maxPause = 2f;

    float searchTimer = 0f;
    float currentDuration = 0f;

    bool isWalking = true;
    int searchDirection = 1; // 1 = droite, -1 = gauche
    public bool asDetected = false;

    [Header("Vision Settings")]
    [SerializeField] float visionMax = 8f;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask playerLayer;

    bool isFacingRight = true;

    JumpTriggerScript[] jumpTriggers;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
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
                speed = 4f;
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
        
        searchDirection = Random.value < 0.5f ? -1 : 1;
        StartWalkingPhase();
    }

    #region Behave
    void StartWalkingPhase()
    {
        isWalking = true;
        currentDuration = Random.Range(minMarche, maxMarche);
        searchTimer = 0f;
    }

    void StartPausePhase()
    {
        isWalking = false;
        currentDuration = Random.Range(minPause, maxPause);
        searchTimer = 0f;
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
        Vector2 directionToPlayer = playerTransform.position - transform.position;
        if(directionToPlayer.magnitude > 2f)
        {
            MoveTowardsPlayer(true);
        }
        else if(directionToPlayer.magnitude < 0.5f)
        {
            MoveTowardsPlayer(false);
        }
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

    void searchingBehavior()
    {
        if (!CheckGroundFront())
        {
            searchDirection *= -1;
            targetVelocityX = 0f;
            return;
        }

        searchTimer += Time.deltaTime;

        if (isWalking)
        {
            targetVelocityX = searchDirection * speed;

            if (searchTimer >= currentDuration)
            {
                StartPausePhase();
            }
        }
        else
        {
            targetVelocityX = 0f;

            if (searchTimer >= currentDuration)
            {
                // 1 chance sur 2 de se retourner
                if (Random.value < 0.5f)
                {
                    searchDirection *= -1;
                }

                StartWalkingPhase();
            }
        }
    }

    void ChooseBehavior()
    {
        if (!asDetected)
        {
            searchingBehavior();
            return;
        }
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
        }
        else
        {
            enemieRB.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void Hit(int damage)
    {
        healthManagerMob.Hit(damage);
    }
    #endregion Behave
    
    #region Jump
    bool CheckGround()
    {
        Vector2 bottomCenter = (Vector2)transform.position + Vector2.down * enemieCollider.bounds.extents.y;
        float skinWidth = 0.01f;
        Vector2 origin = bottomCenter + Vector2.down * skinWidth;
        Vector2 boxSize = new Vector2(enemieCollider.bounds.size.x * 0.9f, 0.02f);
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
    bool CheckGroundFront()
    {
        // Centre bas du collider
        Vector2 bottomCenter = (Vector2)transform.position + Vector2.down * enemieCollider.bounds.extents.y;

        // Décalage vers l'avant selon la direction
        float direction = searchDirection;
        float forwardOffset = enemieCollider.bounds.extents.x;

        Vector2 origin = bottomCenter + Vector2.right * direction * forwardOffset;

        Vector2 boxSize = new Vector2(0.1f, 0.02f);
        float distance = 0.2f;

        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, distance, groundLayers);

        return hit.collider != null;
    }

    void Jump(float jumpForce)
    {
        if (!touchGround || (playerTransform.position.y - transform.position.y) < 5) return;
        enemieRB.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }
    #endregion Jump

    bool CanSeePlayer()
    {
        Vector2 origin = transform.position;
        int sens = isFacingRight ? 1 : -1;
        Vector2 direction = new Vector2(playerTransform.position.x, playerTransform.position.y) - origin;
        direction = direction.normalized;

        // Distance jusqu'au joueur
        float distanceToPlayer = Vector2.Distance(origin, playerTransform.position);

        if (distanceToPlayer > visionMax)
            return false;

        // Vérifie si le joueur est dans la bonne direction
        float dirToPlayer = playerTransform.position.x - transform.position.x;

        if (isFacingRight && dirToPlayer < 0)
            return false;

        if (!isFacingRight && dirToPlayer > 0)
            return false;

        // Raycast pour vérifier obstacle
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, visionMax, obstacleLayer | playerLayer);

        if (hit.collider != null)
        {
            // Si le premier objet touché est le player il est visible
            if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
            {
                return true;
            }
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        targetVelocityX = 0f; // Reset target velocity at the start of each frame
        if (!isFrozen)
        {
            touchGround = CheckGround();
            touchGroundfront = CheckGroundFront();
            if(!asDetected) asDetected = CanSeePlayer();
            ChooseBehavior();
        }
    }
    
    void FixedUpdate()
    {
        float newVx = Mathf.MoveTowards(enemieRB.linearVelocity.x, targetVelocityX, acceleration * Time.fixedDeltaTime);
        enemieRB.linearVelocity = new Vector2(newVx, enemieRB.linearVelocity.y);    
        if (targetVelocityX > 0)
            isFacingRight = true;
        else if (targetVelocityX < 0)
            isFacingRight = false;
        
        if (enemieRB.linearVelocity.y < 0)
            enemieRB.gravityScale = gravityScale * gravityMultiplier;
        else
            enemieRB.gravityScale = gravityScale;

    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.gameObject.tag == "LeftJumpTrigger" && !isFacingRight)
        {
            float jumpForce = collision.gameObject.GetComponent<JumpTriggerScript>().jumpForce;

            Jump(jumpForce);
        }
        if(collision.collider.gameObject.tag == "RightJumpTrigger" && isFacingRight)
        {
            float jumpForce = collision.gameObject.GetComponent<JumpTriggerScript>().jumpForce;

            Jump(jumpForce);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + direction * visionMax);

        if (enemieCollider == null)
            return;

        float skinWidth = 0.01f;

        Vector2 bottomCenter = (Vector2)transform.position + Vector2.down * enemieCollider.bounds.extents.y;
        Vector2 origin = bottomCenter + Vector2.down * skinWidth;

        Vector2 boxSize = new Vector2(enemieCollider.bounds.size.x * 0.9f, 0.02f);
        float distance = 0.1f;

        // Couleur de la box initiale
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(origin, boxSize);

        // Couleur de la box à la fin du cast
        Gizmos.color = Color.red;
        Vector2 castEnd = origin + Vector2.down * distance;
        Gizmos.DrawWireCube(castEnd, boxSize);

        // Ligne entre les deux (visualisation du cast)
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, castEnd);
    }
}
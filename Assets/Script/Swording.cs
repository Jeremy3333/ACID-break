using UnityEngine;
using UnityEngine.InputSystem;

public class Swording : MonoBehaviour
{
    private Camera mainCam;

    [SerializeField] Transform target;

    [Header("Attack (Left Click)")]
    public int damage = 2;
    public float attackCooldown = 0.5f;
    public float activeTime = 0.15f;

    [Header("Pogo")]
    public float pogoForce = 18f;
    public float pogoDownAngleTolerance = 30f;
    bool pogoTriggered;


    [Header("Melee Arc")]
    public float attackRadius = 1.5f; // porté de l'attaque du personnage

    [Range(0f, 360f)]
    public float attackAngle = 120f;

    [Header("Charged Attack (Right Click)")]
    [Range(0f, 360f)]
    public float chargedAttackAngle = 120f;
    public float chargedAttackChargeTime = 0.75f;
    public float chargedAttackCooldown = 0f; //temps entre chaque attaque
    public int chargedDamage = 6;
    public float chargedDashForce = 22f;
    public float chargedActiveTime = 0.22f; //duree du dash

    [Header("Charged Attack Hit Rules")]
    public float chargedHitInterval = 0.03f; // fréquence de détection dash
    private bool isChargedActive = false;
    private float chargedHitTimer = 0f;

    private readonly System.Collections.Generic.HashSet<Collider2D> chargedAlreadyHit = new System.Collections.Generic.HashSet<Collider2D>();

    [Header("References")]
    public SpriteRenderer swordSprite;
    public Collider2D swordCollider;
    public Rigidbody2D playerRb;
    CharacterHandler characterHandler;

    private bool canAttack = true;
    private float attackTimer;

    private bool canCharged = true;
    private float chargedTimer;

    public bool isCharging = false;
    private float chargeTimer = 0f;
    private bool chargeCancelled = false;

    private RigidbodyConstraints2D _savedConstraints;
    private bool _movementLocked = false;

    void Start()
    {
        mainCam = Camera.main;
        swordSprite.enabled = false;
        swordCollider.enabled = false;
        characterHandler = GetComponentInParent<CharacterHandler>();
    }

    void Update()
    {
        RotateToTarget();
        HandleAttackCooldown();
        HandleChargedCooldown();

        if (isCharging)
            UpdateCharge();

        if (isChargedActive)
        {
            chargedHitTimer += Time.deltaTime;
            if (chargedHitTimer >= chargedHitInterval)
            {
                chargedHitTimer = 0f;
                DoArcDamage(chargedDamage, chargedAttackAngle);
            }
        }

    }

    void RotateToTarget()
    {
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void HandleAttackCooldown()
    {
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0f;
            }
        }
    }

    void HandleChargedCooldown()
    {
        if (!canCharged)
        {
            chargedTimer += Time.deltaTime;
            if (chargedTimer >= chargedAttackCooldown)
            {
                canCharged = true;
                chargedTimer = 0f;
            }
        }
    }

    bool IsAttackingDown()
    {
        Vector2 attackDir = isChargedActive ? chargedDashDir : (Vector2)transform.right;

        float angle = Vector2.Angle(attackDir, Vector2.down);

        return angle <= pogoDownAngleTolerance;
    }

    public void Attack()
    {
        if(!canAttack) return;
        canAttack = false;
        pogoTriggered = false;

        swordSprite.enabled = true;
        swordCollider.enabled = true;

        DoArcDamage(damage, attackAngle);

        Invoke(nameof(EndAttack), activeTime);
    }

    void EndAttack()
    {
        swordSprite.enabled = false;
        swordCollider.enabled = false;
    }

    public void StartCharge()
    {
        if(!canCharged) return;
        isCharging = true;
        chargeTimer = 0f;
        chargeCancelled = false;
        LockMovement();
    }

    void UpdateCharge()
    {
        if (chargeCancelled)
            return;

        chargeTimer += Time.deltaTime;

        if (chargeTimer >= chargedAttackChargeTime)
        {
            ExecuteChargedAttack();
        }
    }

    public void CancelCharge()
    {
        isCharging = false;
        chargeTimer = 0f;
        chargeCancelled = true;
        UnlockMovement();
    }

    private Vector2 chargedDashDir;

    void ExecuteChargedAttack()
    {
        if (!isCharging)
            return;

        pogoTriggered = false;
        isCharging = false;
        canCharged = false;

        UnlockMovement();

        chargedAlreadyHit.Clear();
        isChargedActive = true;
        chargedHitTimer = 0f;

        chargedDashDir = (target.position - transform.position).normalized;
        swordSprite.enabled = true;
        swordCollider.enabled = true;

        Invoke(nameof(EndChargedAttack), chargedActiveTime);
    }

    void FixedUpdate()
    {
        if (isChargedActive)
        {
            playerRb.linearVelocity = chargedDashDir * chargedDashForce;
        }
    }

    void EndChargedAttack()
    {
        isChargedActive = false;
        swordSprite.enabled = false;
        swordCollider.enabled = false;
        UnlockMovement();
    }

    void DoArcDamage(int dmg, float arcAngle)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Mob"))
                continue;

            if (isChargedActive && chargedAlreadyHit.Contains(hit))
                continue;

            Vector2 dirToEnemy = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
            Vector2 facing = isChargedActive ? chargedDashDir : (Vector2)transform.right;

            float angleToEnemy = Vector2.Angle(facing, dirToEnemy);

            if (angleToEnemy <= arcAngle * 0.5f)
            {
                HealthManagerMob health = hit.GetComponent<HealthManagerMob>();
                if (health != null)
                {
                    health.Hit(dmg);
                    if (!pogoTriggered && IsAttackingDown())
                    {
                        pogoTriggered = true;
                        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0f);
                        playerRb.AddForce(Vector2.up * pogoForce, ForceMode2D.Impulse);
                    }
                    //Debug.Log("Ennemi touché pendant charge : " + hit.name + " Damage : " + dmg);
                    if (isChargedActive)
                        chargedAlreadyHit.Add(hit);
                }
            }
        }
    }

    void LockMovement()
    {
        if (_movementLocked) return;
        _movementLocked = true;

        _savedConstraints = playerRb.constraints;
        playerRb.linearVelocity = Vector2.zero;

        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void UnlockMovement()
    {
        if (!_movementLocked) return;
        _movementLocked = false;

        playerRb.constraints = _savedConstraints;
    }

    public void InterruptChargedAttack()
    {
        if (isCharging)
            CancelCharge();
    }

    // Debug visuel depuis unity
    void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;

        // Couleur du cercle (rayon)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, attackRadius);

        Vector3 left = Quaternion.Euler(0, 0, attackAngle * 0.5f) * transform.right;
        Vector3 right = Quaternion.Euler(0, 0, -attackAngle * 0.5f) * transform.right;

        // Couleur des lignes d'angle
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos, pos + left * attackRadius);
        Gizmos.DrawLine(pos, pos + right * attackRadius);
    }

}

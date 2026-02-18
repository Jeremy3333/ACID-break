using UnityEngine;
using UnityEngine.InputSystem;

public class Swording : MonoBehaviour
{
    private Camera mainCam;
    private Vector2 mousePos;

    [Header("Attack (Left Click)")]
    public int damage = 2;
    public float attackCooldown = 0.5f;
    public float activeTime = 0.15f;

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
    public Rigidbody2D playerRb;

    private bool canAttack = true;
    private float attackTimer;

    private bool canCharged = true;
    private float chargedTimer;

    private bool isCharging = false;
    private float chargeTimer = 0f;
    private bool chargeCancelled = false;

    private RigidbodyConstraints2D _savedConstraints;
    private bool _movementLocked = false;

    void Start()
    {
        mainCam = Camera.main;
        swordSprite.enabled = false;
    }

    void Update()
    {
        RotateToMouse();
        HandleAttackCooldown();
        HandleChargedCooldown();

        if (Mouse.current.leftButton.isPressed && canAttack)
            Attack();

        if (Mouse.current.rightButton.wasPressedThisFrame && canCharged)
            StartCharge();

        if (isCharging)
            UpdateCharge();

        /*
        if (Mouse.current.rightButton.wasReleasedThisFrame && isCharging)
            CancelCharge(); // si on relache avant la fin on annule l'attaque
        */

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

    void RotateToMouse()
    {
        mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCam.ScreenToWorldPoint(
            new Vector3(mousePos.x, mousePos.y, mainCam.nearClipPlane)
        );

        Vector3 dir = worldPos - transform.position;
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

    void Attack()
    {
        canAttack = false;

        swordSprite.enabled = true;

        DoArcDamage(damage, attackAngle);

        Invoke(nameof(EndAttack), activeTime);
    }

    void EndAttack()
    {
        swordSprite.enabled = false;
    }

    void StartCharge()
    {
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

    void CancelCharge()
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

        isCharging = false;
        canCharged = false;

        UnlockMovement();

        chargedAlreadyHit.Clear();
        isChargedActive = true;
        chargedHitTimer = 0f;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        chargedDashDir = (mouseWorldPos - transform.position).normalized;
        swordSprite.enabled = true;

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
                    Debug.Log("Ennemi touché pendant charge : " + hit.name + " Damage : " + dmg);
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



    ////////////////////////////////////////////////////////////
    // A implementer plsu tard lorsque joueur prend des degats
    ////////////////////////////////////////////////////////////
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

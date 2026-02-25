using UnityEngine;
using System.Collections;

public class SwordMob : MonoBehaviour
{
    Transform target;

    [Header("Attack")]
    [SerializeField] float attackCooldown = 2.0f;
    [SerializeField] float activeTime = 0.3f;
    [SerializeField] float anticipationTime = 0.8f;

    [SerializeField] float attackRadius = 1f; // porté de l'attaque du personnage

    [Range(0f, 360f)]
    [SerializeField] float attackAngle = 100f;

    [Header("References")]
    [SerializeField] Collider2D swordHitbox;
    [SerializeField] SpriteRenderer mobSprite;
    [SerializeField] SpriteRenderer swordSprite;
    [SerializeField] Rigidbody2D MobRb;
    EnemieHandler enemieHandler;

    bool canAttack = true;
    bool isAnticipating = false;

    float cooldownTimer;
    float anticipationTimer;

    AudioManager audioManager;

    private Animator animator;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        swordHitbox.enabled = false;
        swordSprite.enabled = false;
        enemieHandler = GetComponentInParent<EnemieHandler>();

        animator = transform.parent.gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        RotateToTarget();
        HandleCooldown();
        CheckAttack();
        HandleAnticipation();
        audioManager = transform.parent.GetComponentInChildren<AudioManager>();
    }

    void RotateToTarget()
    {
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void HandleCooldown()
    {
        if (!canAttack)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= attackCooldown)
            {
                canAttack = true;
                cooldownTimer = 0f;
            }
        }
    }

    void CheckAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        if (!canAttack || isAnticipating || distanceToPlayer > 2.5f || !enemieHandler.asDetected || enemieHandler.isFrozen)
            return;

        StartAnticipation();
    }

    void StartAnticipation()
    {
        isAnticipating = true;
        animator.SetInteger("AttackPhase", 1);
        anticipationTimer = 0f;
        StartCoroutine(BlinkEffect());
    }

    void HandleAnticipation()
    {
        if (!isAnticipating) return;

        anticipationTimer += Time.deltaTime;

        if (anticipationTimer >= anticipationTime)
        {
            isAnticipating = false;
            Debug.Log(Time.deltaTime);
            Attack();
        }
    }

    IEnumerator BlinkEffect()
    {
        while (isAnticipating)
        {
            mobSprite.enabled = !mobSprite.enabled;
            yield return new WaitForSeconds(0.1f);
        }

        mobSprite.enabled = true; // Assure qu'il reste visible
    }

    void DoArcDamage(float arcAngle)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Player"))
                continue;

            Vector2 dirToPlayer = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
            Vector2 facing = (Vector2)transform.right;

            float angleToPlayer = Vector2.Angle(facing, dirToPlayer);

            if (angleToPlayer <= arcAngle * 0.5f)
            {
                HealthManager health = hit.GetComponent<HealthManager>();
                if (health != null)
                {
                    health.Hit();
                }
            }
        }
        audioManager.PlayClipRand(audioManager.woosh);
    }

    void Attack()
    {
        canAttack = false;

        swordSprite.enabled = true;
        animator.SetInteger("AttackPhase", 2);

        DoArcDamage(attackAngle);

        Invoke(nameof(EndAttack), activeTime);
    }
    
    void EndAttack()
    {
        swordSprite.enabled = false;
        animator.SetInteger("AttackPhase", 0);
    }

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

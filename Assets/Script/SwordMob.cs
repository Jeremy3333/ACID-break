using UnityEngine;

public class SwordMob : MonoBehaviour
{
    [SerializeField] Transform target;

    [Header("Attack")]
    public float attackCooldown = 2.0f;
    public float activeTime = 0.3f;

    [Header("References")]
    [SerializeField] EnemieHandler enemieHandler;
    public Collider2D swordHitbox;
    public SpriteRenderer swordSprite;
    public Rigidbody2D MobRb;

    private bool canAttack = true;
    private float timer;

    void Start()
    {
        swordHitbox.enabled = false;
        swordSprite.enabled = false;
    }

    void Update()
    {
        RotateToTarget();
        HandleCooldown();
        Attack();
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
            timer += Time.deltaTime;
            if (timer >= attackCooldown)
            {
                canAttack = true;
                timer = 0f;
            }
        }
    }

    public void Attack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        if(!canAttack || distanceToPlayer > 2.5 || !enemieHandler.asDetected) return;
        canAttack = false;
        swordHitbox.enabled = true;
        swordSprite.enabled = true;
        Invoke(nameof(DisableHitbox), activeTime);
    }

    void DisableHitbox()
    {
        swordHitbox.enabled = false;
        swordSprite.enabled = false;
    }
}

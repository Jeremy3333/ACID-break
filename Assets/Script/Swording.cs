using UnityEngine;

public class Swording : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField] Transform target;

    [Header("Attack")]
    public int damage = 2;
    public float attackCooldown = 0.5f;
    public float activeTime = 0.15f;

    [Header("References")]
    public Collider2D swordHitbox;
    public SpriteRenderer swordSprite;
    public Rigidbody2D playerRb;

    private bool canAttack = true;
    private float timer;

    void Start()
    {
        mainCam = Camera.main;
        swordHitbox.enabled = false;
        swordSprite.enabled = false;
    }

    void Update()
    {
        RotateToTarget();
        HandleCooldown();
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
        if(canAttack == false) return;
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

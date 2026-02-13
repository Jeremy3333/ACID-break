using UnityEngine;

public class GrappleProjectile : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public Collider2D col;

    [Header("Settings")]
    public float maxDistance = 20f;          // auto‑retract if farther than this
    public float retractSpeed = 15f;
    public float closeEnoughDistance = 0.5f;

    [HideInInspector] public Grappling parentGrappling;
    [HideInInspector] public Rigidbody2D playerRb;
    [HideInInspector] public Vector2 firePointPos;

    private bool isRetracting = false;
    private bool isGrappled = false;

    private Vector2 targetPosition;
    private bool hasTarget;           // true when raycast hit something
    private bool hasTriggered = false; // prevent double activation

    void Start()
    {
        firePointPos = transform.position;
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void SetTarget(Vector2 target, bool valid)
    {
        targetPosition = target;
        hasTarget = valid;
    }

    void Update()
    {
        if (isRetracting)
        {
            Vector2 dir = (playerRb.position - rb.position).normalized;
            rb.linearVelocity = dir * retractSpeed;

            if (Vector2.Distance(playerRb.position, rb.position) < closeEnoughDistance)
                parentGrappling.DestroyCurrentProjectile();
        }
        else if (!isGrappled)
        {
            // If we have a guaranteed target and haven't triggered yet, check proximity
            if (hasTarget && !hasTriggered)
            {
                float distToTarget = Vector2.Distance(rb.position, targetPosition);
                if (distToTarget < closeEnoughDistance)
                {
                    hasTriggered = true;
                    parentGrappling.OnGrappleHit(targetPosition);
                    isGrappled = true;
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    col.enabled = false;
                    return;
                }
            }

            // Auto‑retract when too far (original behaviour, works even with target)
            if (Vector2.Distance(playerRb.position, rb.position) > maxDistance)
                StartRetract();
        }
    }

    void StartRetract()
    {
        isRetracting = true;
        col.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isRetracting || hasTriggered) return;

        // Collision detected – use it (works as backup)
        Vector2 hitPoint = other.ClosestPoint(transform.position);
        parentGrappling.OnGrappleHit(hitPoint);
        isGrappled = true;
        hasTriggered = true;
        transform.position = hitPoint;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        col.enabled = false;
    }
}
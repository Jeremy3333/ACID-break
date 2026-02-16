using UnityEngine;

public class GrappleProjectile : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;

    [Header("Settings")]
    public float maxDistance = 20f;          // auto‑retract if farther than this
    public float retractSpeed = 15f;
    float closeEnoughDistance = 1f;

    [HideInInspector] public Grappling parentGrappling;
    [HideInInspector] public Rigidbody2D playerRb;

    private bool isRetracting = false;
    private bool isGrappled = false;

    private Vector2 targetPosition;
    private bool hasTarget;           // true when raycast hit something
    private bool hasTriggered = false; // prevent double activation

    void Start()
    {
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
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
            Vector2 dir = (Vector2)parentGrappling.transform.position - rb.position;
            dir = dir.normalized;
            rb.linearVelocity = dir * retractSpeed;

            if (Vector2.Distance(parentGrappling.transform.position, rb.position) < closeEnoughDistance)
                parentGrappling.DestroyCurrentProjectile();
        }
        else if (!isGrappled)
        {
            // If we have a guaranteed target and haven't triggered yet, check proximity
            if (!hasTriggered)
            {
                float distToTarget = Vector2.Distance(rb.position, targetPosition);
                if (distToTarget < closeEnoughDistance && hasTarget)
                {
                    hasTriggered = true;
                    parentGrappling.OnGrappleHit(targetPosition);
                    isGrappled = true;
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    rb.position = targetPosition; // snap to target to ensure joint connection
                    return;
                } else if (distToTarget < closeEnoughDistance)
                {
                    StartRetract();
                }
            }
            if (hasTarget && !hasTriggered && !isRetracting)
            {
                Vector2 directionToTarget = (targetPosition - rb.position).normalized;
                float speed = rb.linearVelocity.magnitude; // maintain initial speed
                rb.linearVelocity = directionToTarget * speed;
            }
            if(Vector2.Distance(parentGrappling.transform.position, rb.position) > maxDistance)
            {
                StartRetract();
            }
        }
    }

    void StartRetract()
    {
        isRetracting = true;
    }
}
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

    void Start()
    {
        firePointPos = transform.position;
    }

    void Update()
    {
        if (isRetracting)
        {
            Vector2 dir = (playerRb.position - rb.position).normalized;
            rb.velocity = dir * retractSpeed;

            if (Vector2.Distance(playerRb.position, rb.position) < closeEnoughDistance)
                Destroy(gameObject);
        }
        else
        {
            if (playerRb != null && Vector2.Distance(playerRb.position, rb.position) > maxDistance)
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
        if (isRetracting) return;

        // No layer mask check – we rely on the Physics2D collision matrix.
        // If this trigger event fired, the layers are set to collide.
        Vector2 hitPoint = other.ClosestPoint(transform.position);
        parentGrappling.OnGrappleHit(hitPoint);
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (parentGrappling != null)
            parentGrappling.OnProjectileDestroyed(this);
    }
}
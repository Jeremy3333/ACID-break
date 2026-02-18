using UnityEngine;
using UnityEngine.InputSystem;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    public Rope rope;
    public Camera cam;
    [SerializeField] Transform target;

    public Transform firePoint;
    public Rigidbody2D rb;
    public SpringJoint2D joint;

    [Header("Grapple Settings")]
    [SerializeField] float jointFrequency = 6f;

    [SerializeField] float retractSpeed = 30f;       // right‑click retract (shorten rope)

    [SerializeField] float maxLength = 15f;

    [Header("Projectile Settings")]
    
    [SerializeField] GameObject grappleProjectilePrefab;
    [SerializeField] float projectileSpeed = 100f;
    [SerializeField] float projectileMaxDistance = 20f;   // how far the projectile can fly
    [SerializeField] float projectileRetractSpeed = 100f;  // speed when it returns automatically
    
    [Header("Collision")]
    [SerializeField] LayerMask grapplePlateformMask;   // set in Inspector to layers the projectile can hit
    [SerializeField] LayerMask grappleEnnemieMask;   // set in Inspector to layers the projectile can hit

    
    [Header("Attaque Settings")]

    [SerializeField] int attaqueDamage = 5;

    [HideInInspector] public Vector2 grapplePoint;

    GameObject currentProjectile;

    bool isGrappled = false;

    bool isEnnemieGrappled = false;

    EnemieHandler grapledEnnemieHandler;

    CharacterHandler characterCharacterHandler;

    void Start()
    {
        joint.enabled = false;
        rope.enabled = false;
    }

    void Update()
    {
        Vector2 targetPos;

        // Aim towards projectile if it exists, otherwise towards mouse
        if (currentProjectile != null)
        {
            targetPos = currentProjectile.transform.position;
        }
        else
        {
            targetPos = target.position;
        }

        Vector3 direction = (Vector3)targetPos - transform.position;
        float rotaZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotaZ);

        if(isEnnemieGrappled && grapledEnnemieHandler != null)
        {
            AttaqueEnnemie();
        }

        characterCharacterHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterHandler>();
    }

    void AttaqueEnnemie()
    {
        Retract();
        if (Vector2.Distance(rb.position, grapplePoint) < 0.5f)
        {
            grapledEnnemieHandler.Hit(attaqueDamage);
            isEnnemieGrappled = false;
            grapledEnnemieHandler.setFrozen(false);
            StopGrapple();
        }
    }

    public void TryGrapple()
    {
        if(isGrappled  || currentProjectile != null) return;

        if (joint.enabled)
            StopGrapple();

        if (currentProjectile != null)
            Destroy(currentProjectile);

        // Direction from firePoint to mouse
        Vector2 targetPos = target.position;
        Vector2 firePos = firePoint.position;
        Vector2 dir = (targetPos - firePos).normalized;

        // Raycast to find a valid grapple point
        RaycastHit2D hitPlateform = Physics2D.Raycast(firePos, dir, maxLength, grapplePlateformMask);
        RaycastHit2D hitEnnemie = Physics2D.Raycast(firePos, dir, maxLength, grappleEnnemieMask);
        Vector2 targetPoint;
        bool hasValidTarget = false;

        if(hitPlateform.collider != null && hitEnnemie.collider != null)
        {
            // If both a platform and an enemy are hit, choose the closer one
            if (hitPlateform.distance < hitEnnemie.distance)
            {
                targetPoint = hitPlateform.point;
                hasValidTarget = true;
            }
            else
            {
                hasValidTarget = true;
                isEnnemieGrappled = true;
                grapledEnnemieHandler = hitEnnemie.collider.GetComponent<EnemieHandler>();
                targetPoint = hitEnnemie.collider.transform.position;
                grapledEnnemieHandler.setFrozen(true);
            }
        }
        else if (hitPlateform.collider != null)
        {
            targetPoint = hitPlateform.point;
            hasValidTarget = true;
        }
        else if (hitEnnemie.collider != null)
        {
            hasValidTarget = true;
            isEnnemieGrappled = true;
            grapledEnnemieHandler = hitEnnemie.collider.GetComponent<EnemieHandler>();
            targetPoint = hitEnnemie.collider.transform.position;
            grapledEnnemieHandler.setFrozen(true);
        }
        else
        {
            targetPoint = firePos + dir * maxLength;
        }

        // Create and setup projectile
        GameObject proj = Instantiate(grappleProjectilePrefab, firePos, Quaternion.identity);
        currentProjectile = proj;

        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        projRb.linearVelocity = dir * projectileSpeed;   // initial direction

        GrappleProjectile projScript = proj.GetComponent<GrappleProjectile>();
        projScript.parentGrappling = this;
        projScript.playerRb = rb;
        projScript.maxDistance = projectileMaxDistance;   // for auto‑retract when no target
        projScript.retractSpeed = projectileRetractSpeed;

        // Pass the target information
        projScript.SetTarget(targetPoint, hasValidTarget);

        rope.enabled = true;
    }

    public void OnGrappleHit(Vector2 hitPoint)
    {
        grapplePoint = hitPoint;
        float distance = Vector2.Distance(rb.position, grapplePoint);
        isGrappled = true;
        characterCharacterHandler.setIsGrapple(true);
        if(distance > maxLength)
        {
            characterCharacterHandler.moveToDirection(distance - maxLength, grapplePoint);
        }
        ActivateJoint();
    }

    public void ActivateJoint()
    {
        joint.enabled = true;
        joint.connectedAnchor = grapplePoint;
        joint.autoConfigureDistance = false;
        joint.distance = Vector2.Distance(rb.position, grapplePoint);
        joint.frequency = jointFrequency;
    }

    public void Retract()
    {
        if (!joint.enabled) return;

        joint.distance -= retractSpeed * Time.deltaTime;
        joint.distance = Mathf.Max(joint.distance, 0.1f); // prevent distance from becoming negative
    }

    public void StopGrapple()
    {
        if(!isGrappled || isEnnemieGrappled) return;

        rope.enabled = false;
        joint.enabled = false;
        isGrappled = false;
        characterCharacterHandler.setIsGrapple(false);
        DestroyCurrentProjectile();
    }

    public Vector2 getGrappleProjectilePosition() {
        if (currentProjectile == null)
        {
            Debug.LogError("No Projectil");
            return Vector2.zero;
        }
        return currentProjectile.transform.position;
            
    }

    public void DestroyCurrentProjectile() {
        if (currentProjectile != null)
        {
            Destroy(currentProjectile);
            currentProjectile = null;
            rope.enabled = false;
        }
    }
}
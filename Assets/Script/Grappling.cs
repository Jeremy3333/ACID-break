using UnityEngine;
using UnityEngine.InputSystem;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    public Rope rope;
    public Camera cam;
    private Vector2 mousePos;

    public Transform firePoint;
    public Rigidbody2D rb;
    public SpringJoint2D joint;

    [Header("Grapple Settings")]
    public float maxDistance = 20f;        // max rope length after attach
    public float jointFrequency = 6f;

    public float retractSpeed = 10f;       // right‑click retract (shorten rope)
    public float minDistance = 1.5f;

    [Header("Projectile Settings")]
    public GameObject grappleProjectilePrefab;
    public float projectileSpeed = 20f;
    public float projectileMaxDistance = 20f;   // how far the projectile can fly
    public float projectileRetractSpeed = 15f;  // speed when it returns automatically

    [HideInInspector] public Vector2 grapplePoint;

    private GameObject currentProjectile;

    void Start()
    {
        joint.enabled = false;
        rope.enabled = false;
    }

    void Update()
    {
        // Aim towards mouse
        mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
        Vector3 rotation = worldPos - transform.position;
        float rotaZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotaZ);

        // Fire projectile on left click
        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryGrapple();

        // Retract joint (pull player) on right hold
        if (Mouse.current.rightButton.isPressed)
            Retract();

        // Release grapple on left release
        if (Mouse.current.leftButton.wasReleasedThisFrame)
            StopGrapple();
    }

    void TryGrapple()
    {
        if (joint.enabled)
            StopGrapple();

        if (currentProjectile != null)
            Destroy(currentProjectile);

        GameObject proj = Instantiate(grappleProjectilePrefab, firePoint.position, Quaternion.identity);
        currentProjectile = proj;

        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        Vector2 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = (mouseWorld - (Vector2)firePoint.position).normalized;
        projRb.velocity = dir * projectileSpeed;

        GrappleProjectile projScript = proj.GetComponent<GrappleProjectile>();
        projScript.parentGrappling = this;
        projScript.playerRb = rb;
        projScript.maxDistance = projectileMaxDistance;
        projScript.retractSpeed = projectileRetractSpeed;
    }

    public void OnGrappleHit(Vector2 hitPoint)
    {
        grapplePoint = hitPoint;
        rope.enabled = true;
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

    void Retract()
    {
        if (!joint.enabled) return;

        joint.distance -= retractSpeed * Time.deltaTime;
        joint.distance = Mathf.Max(joint.distance, minDistance);
    }

    void StopGrapple()
    {
        rope.enabled = false;
        joint.enabled = false;
    }

    public void OnProjectileDestroyed(GrappleProjectile proj)
    {
        if (currentProjectile == proj.gameObject)
            currentProjectile = null;
    }
}
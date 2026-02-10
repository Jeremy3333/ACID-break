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
    public float maxDistance = 20f;
    public LayerMask grappableLayer;
    public float jointFrequency = 6f;

    public float retractSpeed = 10f;
    public float minDistance = 1.5f;

    [HideInInspector] public Vector2 grapplePoint;

    void Start()
    {
        joint.enabled = false;
        rope.enabled = false;
    }

    void Update()
    {
        mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));

        Vector3 rotation = worldPos - transform.position;
        float rotaZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,rotaZ);

        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryGrapple();

        if (Mouse.current.rightButton.isPressed)
            Retract();

        if (Mouse.current.leftButton.wasReleasedThisFrame)
            StopGrapple();
    }

    void TryGrapple()
    {
        Vector2 mouseWorld =
            cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Vector2 dir = (mouseWorld - (Vector2)firePoint.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(
            firePoint.position,
            dir,
            maxDistance,
            grappableLayer
        );

        if (!hit) return;

        grapplePoint = hit.point;

        rope.enabled = true;
    }

    void Retract()
    {
        if (!joint.enabled) return;

        joint.distance -= retractSpeed * Time.deltaTime;
        joint.distance = Mathf.Max(joint.distance, minDistance);
    }

    public void ActivateJoint()
    {
        joint.enabled = true;
        joint.connectedAnchor = grapplePoint;
        joint.autoConfigureDistance = false;
        joint.distance = Vector2.Distance(rb.position, grapplePoint);
        joint.frequency = jointFrequency;
    }

    void StopGrapple()
    {
        rope.enabled = false;
        joint.enabled = false;
    }
}

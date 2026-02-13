using UnityEngine;

public class Rope : MonoBehaviour
{
    public LineRenderer line;
    public Grappling grappling;

    [Header("Rope Settings")]
    public float extendDuration = 0.15f;

    void Awake()
    {
        line.positionCount = 2;
        line.enabled = false;
    }

    void OnEnable()
    {
        line.enabled = true;
    }

    void OnDisable()
    {
        line.enabled = false;
    }

    void Update()
    {
        if (!line.enabled) return;

        line.SetPosition(0, grappling.firePoint.position);


        line.SetPosition(1, grappling.getGrappleProjectilePosition());
    }
}

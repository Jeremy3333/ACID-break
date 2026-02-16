using UnityEngine;

public class Rope : MonoBehaviour
{
    public LineRenderer line;
    public Grappling grappling;

    void Awake()
    {
        line.positionCount = 2;
        line.enabled = false;
    }

    void OnEnable()
    {
        line.SetPosition(0, grappling.firePoint.position);
        line.SetPosition(1, grappling.getGrappleProjectilePosition());
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

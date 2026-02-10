using UnityEngine;

public class Rope : MonoBehaviour
{
    public LineRenderer line;
    public Grappling grappling;

    [Header("Rope Settings")]
    public float extendDuration = 0.15f;

    float t;
    bool extending;

    void Awake()
    {
        line.positionCount = 2;
        line.enabled = false;
    }

    void OnEnable()
    {
        t = 0f;
        extending = true;
        line.enabled = true;
    }

    void OnDisable()
    {
        line.enabled = false;
        extending = false;
    }

    void Update()
    {
        if (!line.enabled) return;

        line.SetPosition(0, grappling.firePoint.position);

        if (extending)
        {
            t += Time.deltaTime / extendDuration;
            Vector2 pos = Vector2.Lerp(
                grappling.firePoint.position,
                grappling.grapplePoint,
                t
            );

            line.SetPosition(1, pos);

            if (t >= 1f)
            {
                extending = false;
                grappling.ActivateJoint();
            }
        }
        else
        {
            line.SetPosition(1, grappling.grapplePoint);
        }
    }
}

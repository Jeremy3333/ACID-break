using UnityEngine;
using UnityEngine.InputSystem;

public class Swording : MonoBehaviour
{
    private Camera mainCam;
    private Vector2 mousePos;

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
        RotateToMouse();
        HandleCooldown();

        if (Mouse.current.leftButton.wasPressedThisFrame && canAttack)
        {
            Attack();
        }
    }

    void RotateToMouse()
    {
        mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCam.ScreenToWorldPoint(
            new Vector3(mousePos.x, mousePos.y, mainCam.nearClipPlane)
        );

        Vector3 dir = worldPos - transform.position;
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

    void Attack()
    {
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

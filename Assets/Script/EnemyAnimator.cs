using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAnimator : MonoBehaviour
{

    public Rigidbody2D enemy;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private SpriteRenderer gunnerArmRenderer;

    private const float animEpsilon = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform gunnerArmTrans =transform.Find("RotaPoint").Find("GunnerArm");
        if (gunnerArmTrans != null)
        {
            gunnerArmRenderer = gunnerArmTrans.gameObject.GetComponent<SpriteRenderer>();
        }
            

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        enemy = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (Mathf.Abs(enemy.linearVelocity.x) > animEpsilon)
        {
            spriteRenderer.flipX = enemy.linearVelocity.x < 0f ? true : false;
            if (gunnerArmRenderer != null)
            {
                gunnerArmRenderer.flipY = spriteRenderer.flipX;
            }
        }

        animator.SetBool("IsChasing", Mathf.Abs(enemy.linearVelocity.x) > animEpsilon);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAnimator : MonoBehaviour
{
    Transform target;

    public Rigidbody2D enemy;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private SpriteRenderer gunnerArmRenderer;

    private const float animEpsilon = 1f;

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
        target = GameObject.FindGameObjectWithTag("Player").transform;
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

        animator.SetFloat("XSpeed", Mathf.Abs(enemy.linearVelocity.x));

        // If BasicEnemy
        if (gunnerArmRenderer == null)
        {
            // If enemy's attacking, make it look at the player
            if (animator.GetInteger("AttackPhase") > 0)
            {
                spriteRenderer.flipX = (target.transform.position.x < transform.position.x);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    public Rigidbody2D character;
    public GameObject torso;
    private Animator legsAnimator, torsoAnimator;
    private SpriteRenderer legsRenderer, torsoRenderer;

    private const float animEpsilon = 1f;

    private float bangFlipsUntil = 0f;
    const float timeAfterBang = 0.5f;

    private Keyboard keyboard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        legsAnimator = GetComponent<Animator>();
        legsRenderer = GetComponent<SpriteRenderer>();

        torsoAnimator = torso.GetComponent<Animator>();
        torsoRenderer = torso.GetComponent<SpriteRenderer>();

        keyboard = Keyboard.current;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // I flip it back to normal if we're not shootign or anything
        if(!torsoAnimator.GetBool("Bang") && (Time.time > timeAfterBang))
        {
            torso.transform.right = Vector2.right;
            torsoRenderer.flipX = legsRenderer.flipX;
            torsoRenderer.flipY = false;

            int dir = (keyboard.aKey.isPressed ? 0 : 1)
                - (keyboard.dKey.isPressed ? 0 : 1);

            if (Mathf.Abs(dir) >= 1)
            {
                legsRenderer.flipX =
                    torsoRenderer.flipX = dir < 0;
            }
        }

        float xSpeedAbs = Mathf.Abs(character.linearVelocity.x);
        legsAnimator.SetFloat("XSpeed", xSpeedAbs);
        torsoAnimator.SetFloat("XSpeed", xSpeedAbs);

        legsAnimator.SetFloat("YSpeed", character.linearVelocity.y);
        torsoAnimator.SetFloat("YSpeed", character.linearVelocity.y);
    }

    public void Bang(Vector3 mouseWorldPos, Vector3 gunPos)
    {
        // Rotate the torso in the aiming direction
        bangFlipsUntil = Time.time + timeAfterBang;
        torsoRenderer.flipX = mouseWorldPos.x > transform.position.x;
        torsoRenderer.flipY = true;
        torso.transform.right = (gunPos - torso.transform.position)
            * ((torsoRenderer.flipX) ? -1 : 1);

        torsoAnimator.SetTrigger("Bang");
    }

    public void Dash()
    {
        legsAnimator.SetTrigger("Dash");
        torsoAnimator.SetTrigger("Dash");
    }
}

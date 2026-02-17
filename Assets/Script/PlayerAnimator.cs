using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] InputActionAsset inputActions;
    private InputAction m_moveAction;
    private float moveInput;

    public Rigidbody2D character;
    public GameObject torso;
    private Animator legsAnimator, torsoAnimator;
    private SpriteRenderer legsRenderer, torsoRenderer;

    private const float animEpsilon = 1f;

    private float bangFlipsUntil = 0f;
    const float timeAfterBang = 0.5f;

    private bool dashFlipX;

    private Keyboard keyboard;

    private void Awake()
    {
        m_moveAction = inputActions.FindAction("Move");
    }

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

            moveInput = m_moveAction.ReadValue<float>();

            moveInput = m_moveAction.ReadValue<float>();

            if (Mathf.Abs(moveInput) >= 0.1f)
            {
                legsRenderer.flipX =
                    torsoRenderer.flipX = moveInput < 0;
            }
        }

        // Lock legs flip during dash
        if(legsAnimator.GetBool("IsDashing"))
        {
            legsRenderer.flipX = dashFlipX;
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

    public void Dash(bool toggle, bool flipX)
    {
        dashFlipX = flipX;
        legsAnimator.SetBool("IsDashing", toggle);
        torsoAnimator.SetBool("IsDashing", toggle);
    }
}

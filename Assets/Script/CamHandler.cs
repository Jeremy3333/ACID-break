using UnityEngine;
using UnityEngine.InputSystem;

public class CamHandler : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 0.2f;
    public float motionMagn = 0.4f;
    public float mouseOffset = 3f;

    private Camera cam;
    private Vector3 velocity;
    private Rigidbody2D character;

    void Start()
    {
        cam = Camera.main;
        character = player.gameObject.GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, 0));

        Vector3 dir = mouseWorld - player.position;
        dir = Vector3.ClampMagnitude(dir, mouseOffset);

        Vector3 targetPos = player.position + dir;
        targetPos.z = transform.position.z;

        Vector3 motionImpact = character.linearVelocity;
        motionImpact.x *= motionMagn;
        motionImpact.y *= motionMagn * Screen.height / Screen.width;
        targetPos = targetPos + motionImpact;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            followSpeed
        );
    }
}

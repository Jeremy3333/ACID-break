using UnityEngine;
using UnityEngine.InputSystem;

public class CamHandler : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 0.2f;
    public float mouseOffset = 3f;

    private Camera cam;
    private Vector3 velocity;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, 0));

        Vector3 dir = mouseWorld - player.position;
        dir = Vector3.ClampMagnitude(dir, mouseOffset);

        Vector3 targetPos = player.position + dir;
        targetPos.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            followSpeed
        );
    }
}

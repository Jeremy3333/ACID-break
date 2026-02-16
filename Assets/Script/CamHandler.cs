using UnityEngine;
using UnityEngine.InputSystem;

public class CamHandler : MonoBehaviour
{
    [SerializeField] float followSpeed = 0.2f;
    [SerializeField] float motionMagn = 0.4f;
    [SerializeField] float mouseOffset = 3f;

    Camera cam;
    Vector3 velocity;
    Rigidbody2D playerRb;

    BoxCollider2D lastScreenCollider;
    CharacterHandler characterHandler;

    void Start()
    {
        cam = Camera.main;
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        characterHandler = playerRb.GetComponent<CharacterHandler>();
        if(characterHandler == null)
        {
            Debug.LogError("No character handler found on player");
            Debug.Break();
        }
    }

    void LateUpdate()
    {
        // Get the current screen collider bounds
        BoxCollider2D screenCollider = characterHandler.getScreenCollider();
        if (screenCollider == null && lastScreenCollider == null)
        {
            Debug.LogError("No screen collider found");
            Debug.Break();
            return;
        }
        if (screenCollider != null)
            lastScreenCollider = screenCollider;

        Bounds screenBounds = lastScreenCollider.bounds;

        // Calculate camera extents (half the screen size in world units)
        float verticalExtent = cam.orthographicSize;
        float horizontalExtent = verticalExtent * cam.aspect;

        // Define the inner bounds where the camera's center can be
        float minX = screenBounds.min.x + horizontalExtent;
        float maxX = screenBounds.max.x - horizontalExtent;
        float minY = screenBounds.min.y + verticalExtent;
        float maxY = screenBounds.max.y - verticalExtent;

        // Handle cases where the collider is smaller than the camera view
        if (minX > maxX)
        {
            float midX = (minX + maxX) * 0.5f;
            minX = midX;
            maxX = midX;
        }
        if (minY > maxY)
        {
            float midY = (minY + maxY) * 0.5f;
            minY = midY;
            maxY = midY;
        }

        // Get mouse world position and clamp it to the inner bounds
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, 0));
        mouseWorld.x = Mathf.Clamp(mouseWorld.x, minX, maxX);
        mouseWorld.y = Mathf.Clamp(mouseWorld.y, minY, maxY);

        // Direction from player to clamped mouse, clamped in magnitude
        Vector3 dir = mouseWorld - (Vector3)playerRb.position;
        dir = Vector3.ClampMagnitude(dir, mouseOffset);

        // Base target = player + clamped mouse offset
        Vector3 targetPos = (Vector3)playerRb.position + dir;
        targetPos.z = transform.position.z;

        // Add velocity influence
        Vector3 motionImpact = playerRb.linearVelocity;
        motionImpact.x *= motionMagn;
        motionImpact.y *= motionMagn * Screen.height / Screen.width;
        targetPos += motionImpact;

        // Clamp the final target to the inner bounds (after velocity)
        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        // Smoothly move toward the clamped target
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            followSpeed
        );

        // Extra safety: clamp the final position to the inner bounds (prevents overshoot)
        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
        clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);
        transform.position = clampedPos;
    }
}

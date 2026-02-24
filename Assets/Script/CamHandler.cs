using UnityEngine;
using UnityEngine.InputSystem;

public class CamHandler : MonoBehaviour
{
    [SerializeField] float followSpeed = 0.2f;
    [SerializeField] float motionMagn = 0.4f;
    [SerializeField] float mouseOffset = 3f;
    [SerializeField] Transform target;

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
        updateScreenCollider();

        float minX, minY, maxX, maxY;
        getScreenBound(out minX, out minY, out maxX, out maxY);

        Vector3 targetPos = calculateTargetPos(minX, minY, maxX, maxY);
        
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

    void updateScreenCollider()
    {
        BoxCollider2D screenCollider = characterHandler.getScreenCollider();
        if(screenCollider == null && lastScreenCollider == null)
        {
            Debug.LogError("No screen collider found");
            Debug.Break();
            return;
        }
        if (screenCollider != null)
            lastScreenCollider = screenCollider;
    }

    void getScreenBound(out float minX, out float minY, out float maxX, out float maxY)
    {
        float verticalExtent, horizontalExtent;
        getCamExtent(out verticalExtent, out horizontalExtent);

        Bounds screenBounds = lastScreenCollider.bounds;
        minX = screenBounds.min.x + horizontalExtent;
        maxX = screenBounds.max.x - horizontalExtent;
        minY = screenBounds.min.y + verticalExtent;
        maxY = screenBounds.max.y - verticalExtent;

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

    }

    void getCamExtent(out float verticalExtent, out float horizontalExtent)
    {
        verticalExtent = cam.orthographicSize;
        horizontalExtent = verticalExtent * cam.aspect;
    }

    Vector3 calculateTargetPos(float minX, float minY, float maxX, float maxY)
    {
        Vector3 dir = calculateDisplaceDirection(minX, minY, maxX, maxY);

        Vector3 targetPos = (Vector3)playerRb.position + dir;
        targetPos.z = transform.position.z;

        targetPos += calculateMotionImpact();

        targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        return targetPos;
    }

    Vector3 calculateDisplaceDirection(float minX, float minY, float maxX, float maxY)
    {
        Vector3 dir = target.position - (Vector3)playerRb.position;
        return Vector3.ClampMagnitude(dir, mouseOffset);
    }

    Vector3 getMouseWorldPos(float minX, float minY, float maxX, float maxY)
    {
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, 0));
        mouseWorld.x = Mathf.Clamp(mouseWorld.x, minX, maxX);
        mouseWorld.y = Mathf.Clamp(mouseWorld.y, minY, maxY);
        return mouseWorld;
    }

    Vector3 calculateMotionImpact()
    {
        Vector3 motionImpact = playerRb.linearVelocity;
        motionImpact.x *= motionMagn;
        motionImpact.y *= motionMagn * Screen.height / Screen.width;
        return motionImpact;
    }
}

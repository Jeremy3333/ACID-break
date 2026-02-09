using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairCursor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane)
        );

        worldPos.z = 0f;
        transform.position = worldPos;
    }
}

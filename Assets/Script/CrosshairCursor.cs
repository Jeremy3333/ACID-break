using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairCursor : MonoBehaviour
{
    [SerializeField]
    InputActionAsset inputActions;
    [SerializeField]
    float stickSensitivity = 5f;
    
    private InputAction m_lookAction;
    Vector2 lookInput;
    
    [SerializeField]
    Transform center;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Cursor.visible = false;
        m_lookAction = inputActions.FindAction("Look");
    }

    // Update is called once per frame
    void Update()
    {
        lookInput = m_lookAction.ReadValue<Vector2>();
        
        if (lookInput.sqrMagnitude > 0.01f)
        {
            Vector2 stickOffset = lookInput * stickSensitivity;
            Vector2 targetPos = new Vector2(center.position.x + stickOffset.x, center.position.y + stickOffset.y);

            transform.position = new Vector3(targetPos.x, targetPos.y, 0);
        }
        else if( Mouse.current.delta.ReadValue().sqrMagnitude > 0.01f && Mouse.current != null)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane)
            );

            worldPos.z = 0f;
            transform.position = worldPos;
        }
    }
}

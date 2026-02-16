using UnityEngine;
using UnityEngine.InputSystem;

public class CombatModeManager : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField]
    InputActionAsset inputActions;
    private InputAction m_action1Action;
    private InputAction m_action2Action;
    private InputAction m_mode1Action;
    private InputAction m_mode2Action;
    private InputAction m_mode3Action;
    
    [Header("Modes:")]
    public MonoBehaviour gunMode;
    public MonoBehaviour swordMode;
    public MonoBehaviour grapplingGunMode;

    private ICombatMode currentMode;

    private void Awake()
    {
        m_action1Action = inputActions.FindAction("Action1");
        m_action2Action = inputActions.FindAction("Action2");
        m_mode1Action = inputActions.FindAction("Mode1");
        m_mode2Action = inputActions.FindAction("Mode2");
        m_mode3Action = inputActions.FindAction("Mode3");
    }

    void Start()
    {
        SwitchMode(swordMode as ICombatMode);
        currentMode?.Exit();

        SwitchMode(grapplingGunMode as ICombatMode);
        currentMode?.Exit();
        
        SwitchMode(gunMode as ICombatMode);
    }

    void Update()
    {
        if (m_mode1Action.WasPressedThisFrame())
            SwitchMode(gunMode as ICombatMode);

        if (m_mode2Action.WasPressedThisFrame())
            SwitchMode(swordMode as ICombatMode);

        if (m_mode3Action.WasPressedThisFrame())
            SwitchMode(grapplingGunMode as ICombatMode);

        currentMode?.HandleUpdate();

        currentMode?.Action1(m_action1Action);
        currentMode?.Action2(m_action2Action);
    }

    public void SwitchMode(ICombatMode newMode)
    {
        if (currentMode == newMode) return;

        currentMode?.Exit();
        currentMode = newMode;
        currentMode.Enter();
    }
}

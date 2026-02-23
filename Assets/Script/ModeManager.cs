using UnityEngine;
using UnityEngine.InputSystem;

public class CombatModeManager : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField]
    InputActionAsset inputActions;
    private InputAction m_action1Action;
    private InputAction m_action2Action;
    
    [Header("Modes:")]
    public MonoBehaviour swordMode;
    public MonoBehaviour grapplingGunMode;
    public MonoBehaviour gunMode;
    private ICombatMode currentMode;

    private void Awake()
    {
        m_action1Action = inputActions.FindAction("Action1");
        m_action2Action = inputActions.FindAction("Action2");
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

        currentMode?.HandleUpdate();

        currentMode?.Action2(m_action2Action);
        currentMode?.Action1(m_action1Action);
    }

    public void SwitchMode(ICombatMode newMode)
    {
        if (currentMode == newMode) return;

        currentMode?.Exit();
        currentMode = newMode;
        currentMode.Enter();
    }
}

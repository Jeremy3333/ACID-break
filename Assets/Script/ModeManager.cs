using UnityEngine;
using UnityEngine.InputSystem;

public class CombatModeManager : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField]
    InputActionAsset inputActions;
    private InputAction m_action1Action_batte;
    private InputAction m_action2Action_batte;
    private InputAction m_action1Action_grappin;
    private InputAction m_action2Action_grappin;
    
    [Header("Modes:")]
    public MonoBehaviour swordMode;
    public MonoBehaviour grapplingGunMode;
    public MonoBehaviour gunMode;
    private ICombatMode currentMode;

    private void Awake()
    {
        m_action1Action_batte = inputActions.FindAction("Action1_batte");
        m_action2Action_batte = inputActions.FindAction("Action2_batte");
        m_action1Action_grappin = inputActions.FindAction("Action1_grappin");
        m_action2Action_grappin = inputActions.FindAction("Action2_grappin");
    }

    void Start()
    {
        (swordMode as ICombatMode).Enter();

        (grapplingGunMode as ICombatMode).Enter();
    }

    void Update()
    {
        (swordMode as ICombatMode).Action1(m_action1Action_batte);
        (swordMode as ICombatMode).Action2(m_action2Action_batte);
        (grapplingGunMode as ICombatMode).Action1(m_action1Action_grappin);
        (grapplingGunMode as ICombatMode).Action2(m_action2Action_grappin);
    }
}

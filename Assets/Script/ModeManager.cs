using UnityEngine;
using UnityEngine.InputSystem;

public class CombatModeManager : MonoBehaviour
{
    public MonoBehaviour gunMode;
    public MonoBehaviour swordMode;

    private ICombatMode currentMode;

    void Start()
    {
        SwitchMode(swordMode as ICombatMode);
        currentMode?.Exit();
        
        SwitchMode(gunMode as ICombatMode);
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            SwitchMode(gunMode as ICombatMode);

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            SwitchMode(swordMode as ICombatMode);

        currentMode?.HandleUpdate();
    }

    void SwitchMode(ICombatMode newMode)
    {
        if (currentMode == newMode) return;

        currentMode?.Exit();
        currentMode = newMode;
        currentMode.Enter();
    }
}

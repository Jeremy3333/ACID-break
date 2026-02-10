using UnityEngine;
using UnityEngine.InputSystem;

public class CombatModeManager : MonoBehaviour
{
    public MonoBehaviour gunMode;
    public MonoBehaviour swordMode;
    public MonoBehaviour grapplingGunMode;

    private ICombatMode currentMode;

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
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            SwitchMode(gunMode as ICombatMode);

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            SwitchMode(swordMode as ICombatMode);

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            SwitchMode(grapplingGunMode as ICombatMode);

        currentMode?.HandleUpdate();
    }

    public void SwitchMode(ICombatMode newMode)
    {
        if (currentMode == newMode) return;

        currentMode?.Exit();
        currentMode = newMode;
        currentMode.Enter();
    }
}

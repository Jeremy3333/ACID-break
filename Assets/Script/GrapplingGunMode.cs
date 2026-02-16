using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingGunMode : MonoBehaviour, ICombatMode
{
    public Grappling grappling;

    public void Enter()
    {
        grappling.enabled = true;
    }

    public void Exit()
    {
        grappling.enabled = false;
    }

    public void HandleUpdate()
    {

    }
    public void Action1(InputAction input)
    {
        if(input.WasPressedThisFrame())
            grappling.TryGrapple();
        if(input.WasReleasedThisFrame())
            grappling.StopGrapple();
    }
    public void Action2(InputAction input)
    {
        if(input.IsPressed())
            grappling.Retract();
    }
}

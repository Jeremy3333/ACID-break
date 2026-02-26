using UnityEngine;
using UnityEngine.InputSystem;


public class GunMode : MonoBehaviour, ICombatMode
{
    public Shooting shooting;

    public void Enter()
    {
        shooting.enabled = true;
    }

    public void Exit()
    {
        shooting.enabled = false;
    }

    public void HandleUpdate(InputAction input)
    {
        
    }
    public void Action1(InputAction input)
    {
        if(input.IsPressed())
            shooting.FireSingle();
    }
    public void Action2(InputAction input)
    {
        if(input.IsPressed())
            shooting.FireShotgun();
    }
}
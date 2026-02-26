using UnityEngine;
using UnityEngine.InputSystem;


public class SwordMode : MonoBehaviour, ICombatMode
{
    public Swording swording;

    public void Enter()
    {
        swording.enabled = true;
    }

    public void Exit()
    {
        swording.enabled = false;
    }

    public void HandleUpdate()
    {

    }
    public void Action1(InputAction input)
    {
        if(input.WasPressedThisFrame())
            swording.Attack();
        
        
    }
    
    public void Action2(InputAction input)
    {
        if(input.WasPressedThisFrame())
            swording.StartCharge();
    }
}

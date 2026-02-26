using UnityEngine;
using UnityEngine.InputSystem;



public class SwordMode : MonoBehaviour, ICombatMode
{
    private float pressTimer = 0f;
    public float longPressThreshold = 0.1f;
    private bool isHolding = false;
    public Swording swording;

    public void Enter()
    {
        swording.enabled = true;
    }

    public void Exit()
    {
        swording.enabled = false;
    }

    public void HandleUpdate(InputAction input)
    {

        if(input.WasPressedThisFrame())
        {
            pressTimer = 0f;
            isHolding = true;
        }

        if(isHolding)
        {
            pressTimer += Time.deltaTime;

            if(pressTimer >= longPressThreshold)
            {
                swording.StartCharge();
                isHolding = false;
                pressTimer = 0f;
            }
        }

        if(input.WasReleasedThisFrame())
        {
            if(pressTimer < longPressThreshold)
            {
                swording.Attack();
            }
            isHolding = false;
            pressTimer = 0f;
        }

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

using UnityEngine;

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

    public void HandleUpdate()
    {
        
    }
}
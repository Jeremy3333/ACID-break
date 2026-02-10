using UnityEngine;

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
}

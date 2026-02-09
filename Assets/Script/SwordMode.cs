using UnityEngine;

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
}

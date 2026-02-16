using UnityEngine.InputSystem;

public interface ICombatMode
{
    void Enter();
    void Exit();
    void HandleUpdate();
    void Action1(InputAction input);
    void Action2(InputAction input);
}
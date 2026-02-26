using UnityEngine.InputSystem;

public interface ICombatMode
{
    void Enter();
    void Exit();
    void HandleUpdate(InputAction input);
    void Action1(InputAction input);
    void Action2(InputAction input);
}
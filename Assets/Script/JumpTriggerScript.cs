using UnityEngine;

public class JumpTriggerScript : MonoBehaviour
{
    public float jumpForce = 1f;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}

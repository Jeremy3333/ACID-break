using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit!!!");
        if (collision.collider.gameObject.tag == "Mob")
        {
            collision.gameObject.GetComponent<HealthManagerMob>().Hit(3);
        }
    }
}

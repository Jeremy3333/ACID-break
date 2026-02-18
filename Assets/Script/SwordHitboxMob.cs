using UnityEngine;

public class SwordHitboxMob : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<HealthManager>().Hit();
        }
    }
}

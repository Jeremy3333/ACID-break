using UnityEngine;

public class DamageTilemapScript : MonoBehaviour
{
    [SerializeField]
    private float knockbackForce = 10f;

    private HealthManager healthManager;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        healthManager = player.GetComponent<HealthManager>();
        rb = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Player"
            || collision.collider.gameObject.tag == "WallFriction")
        {
            healthManager.Hit();    
            rb.AddForce(
                (rb.position - (collision.contacts[0].point + collision.contacts[1].point) / 2).normalized
                    * knockbackForce,
                ForceMode2D.Impulse);
        }
    }
}

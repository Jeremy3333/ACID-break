using UnityEngine;
using UnityEngine.InputSystem;

public class BulletScripte : MonoBehaviour
{
    private Camera mainCam;
    private Rigidbody2D rb;
    
    public float force;
    public int damage;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init(Vector2 direction, int dmg)
    {
        damage = dmg;
        rb.linearVelocity = direction.normalized * force;

        float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 viewPos = mainCam.WorldToViewportPoint(transform.position);

        if (viewPos.x < 0 || viewPos.x > 1 ||
            viewPos.y < 0 || viewPos.y > 1)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.tag == "Mob")
        {
            collision.gameObject.GetComponent<HealthManagerMob>().Hit(damage);
        }
        if(collision.collider.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }
    }
}

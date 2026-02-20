using UnityEngine;
using UnityEngine.InputSystem;

public class BulletScripteMob : MonoBehaviour
{
    private Camera mainCam;
    private Rigidbody2D rb;
    
    public float force;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * force;

        float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 viewPos = mainCam.WorldToViewportPoint(transform.position);

        if (viewPos.x < -0.2 || viewPos.x > 1.2 ||
            viewPos.y < -0.2 || viewPos.y > 1.2)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<HealthManager>().Hit();
        }
        bool isTrigger = collision.collider.gameObject.tag == "LeftJumpTrigger" || collision.collider.gameObject.tag == "RightJumpTrigger";
        if(collision.collider.gameObject.tag != "Mob" && collision.collider.gameObject.tag != "Bullet" && !isTrigger)
        {
            Destroy(gameObject);
        }
    }
}

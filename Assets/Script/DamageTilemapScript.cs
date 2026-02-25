using UnityEngine;

public class DamageTilemapScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<HealthManager>().Hit();
        }
    }
}

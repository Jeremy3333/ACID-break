using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    private Camera mainCam;
    private Vector2 mousePos;
    public GameObject bullet;
    public Transform gun;
    public Rigidbody2D playerRb;
    public bool canFire;
    private float timer;
    private float timeBetweenFire;

    public float cadence1 = 0.3f;
    public float cadence2 = 1.3f;

    public int damage1 = 1;
    public int damage2 = 2;

    public int nbShotgun = 10;

    public float recoilForce1 = 3f;
    public float recoilForce2 = 7f;

    public float spreadAngle = 15f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));

        Vector3 rotation = worldPos - transform.position;
        float rotaZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,rotaZ);
        
        if(!canFire)
        {
            timer += Time.deltaTime;
            if(timer > timeBetweenFire) 
            {
                canFire = true;
                timer = 0;
            } 
        }

        if (Mouse.current.leftButton.isPressed && canFire)
        {
            timeBetweenFire = cadence1;
            canFire = false;
            Vector2 recoilDir = -gun.right;
            playerRb.AddForce(recoilDir * recoilForce1, ForceMode2D.Impulse);
            FireSingle(damage1);
        }

        if (Mouse.current.rightButton.isPressed && canFire)
        {
            timeBetweenFire = cadence2;
            canFire = false;
            Vector2 recoilDir = -gun.right;
            playerRb.AddForce(recoilDir * recoilForce2, ForceMode2D.Impulse);
            FireShotgun(damage2);
        }

    }

    void FireSingle(int damage)
    {
        Vector2 direction = gun.right;

        GameObject b = Instantiate(bullet, gun.position, Quaternion.identity);
        b.GetComponent<BulletScripte>().Init(direction, damage);
    }

    void FireShotgun(int damage)
    {
        for (int i = 0; i < nbShotgun; i++)
        {
            float angle = Random.Range(-spreadAngle, spreadAngle);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * gun.right;

            GameObject b = Instantiate(bullet, gun.position, Quaternion.identity);
            b.GetComponent<BulletScripte>().Init(direction, damage);
        }
    }
}

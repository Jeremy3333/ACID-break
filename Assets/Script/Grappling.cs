using UnityEngine;
using UnityEngine.InputSystem;

public class Grappling : MonoBehaviour
{
    private Camera mainCam;
    private Vector2 mousePos;
    public Transform gun;
    public Rigidbody2D playerRb;
    public bool canFire;
    private float timer;
    private float timeBetweenFire;

    public float cadence = 0.3f;

    public int damage = 1;

    public int nbShotgun = 10;

    public float returnForce = 3f;

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
            timeBetweenFire = cadence;
            canFire = false;
            // TODO
        }

        if (Mouse.current.rightButton.isPressed && canFire)
        {
            timeBetweenFire = cadence;
            canFire = false;
            // TODO
        }

    }

    void Fire(int damage)
    {
        // TODO
    }

    void Retract()
    {
        // TODO
    }
}

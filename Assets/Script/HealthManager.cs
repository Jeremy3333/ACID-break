using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int health;
    public bool isHittable = true;

    public ZoneHandler zoneHandler;
    Transform spawn;


    private float timer;

    private Swording swording;
    public float invincybilityTime = 0.6f;

    public Sprite brokenModule;
    public Sprite module;
    public Image[] moduleUI;

    void Awake()
    {
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawn = zoneHandler.spawn;
        Resapwn();
        swording = GetComponentInChildren<Swording>();
        UpDateUI();    
    }

    // Update is called once per frame
    void Update()
    {
        if(!isHittable)
        {
            timer += Time.deltaTime;
            if(timer > invincybilityTime) 
            {
                isHittable = true;
                timer = 0;
            } 
        }
        UpDateUI();

        if(health <= 0) Resapwn();
    }

    public void Hit()
    {
        if(isHittable)
        {
            isHittable = false;
            health = Mathf.Max(health-1, 0);
            swording.InterruptChargedAttack();
        }
    }

    public void Resapwn()
    {
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, 0f);
        health = 4;
        transform.position = spawn.position;
        zoneHandler.Set();
    }

    void Heal()
    {
        health = 4;
    }

    void UpDateUI()
    {
        for (int i = 0; i < moduleUI.Length; i++)
        {
            if (health > i) moduleUI[i].sprite = module;
            else moduleUI[i].sprite = brokenModule;
        }
    }

    void OnTriggerEnter2D(Collider2D  collision)
    {
        if(collision.CompareTag("Zone"))
        {
            zoneHandler = collision.gameObject.GetComponent<ZoneHandler>();
            spawn = zoneHandler.spawn;
            Heal();
            zoneHandler.Set();
        }
    }
    void OnTriggerExit2D(Collider2D  collision)
    {
        if(collision.CompareTag("Zone"))
        {
            collision.gameObject.GetComponent<ZoneHandler>().ReSet();
        }
    }
}

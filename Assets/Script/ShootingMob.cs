using UnityEngine;

public class ShootingMob : MonoBehaviour
{ 
    Transform target;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    [SerializeField] bool canFire;
    EnemieHandler enemieHandler;
    private float timer;
    private float timeBetweenFire;

    [SerializeField] float cadence = 0.3f;

    AudioManager audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        enemieHandler = GetComponentInParent<EnemieHandler>();
        timeBetweenFire = cadence;
        canFire = false;
        audioManager = transform.parent.GetComponentInChildren<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {   
        if(!enemieHandler.asDetected || enemieHandler.isFrozen) return;
        
        RotateToTarget();
        if(!canFire)
        {
            timer += Time.deltaTime;
            if(timer > timeBetweenFire) 
            {
                canFire = true;
                timer = 0;
            } 
        }
        else FireSingle();
    }

    void RotateToTarget()
    {
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void FireSingle()
    {
        if(!canFire) return;
        timeBetweenFire = cadence;
        canFire = false;
        Vector2 recoilDir = -gun.right;
        
        Vector2 direction = gun.right;

        GameObject b = Instantiate(bullet, gun.position, Quaternion.identity);
        b.GetComponent<BulletScripteMob>().Init(direction);

        audioManager.PlayClipRand(audioManager.gun);
    }
}

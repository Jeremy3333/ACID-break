using UnityEngine;

public class Shooting : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField] Transform target;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] bool canFire;
    private float timer;
    private float timeBetweenFire;

    [SerializeField] float cadence1 = 0.3f;
    [SerializeField] float cadence2 = 1.3f;

    [SerializeField] int damage1 = 1;
    [SerializeField] int damage2 = 2;

    [SerializeField] int nbShotgun = 10;

    [SerializeField] float recoilForce1 = 3f;
    [SerializeField] float recoilForce2 = 7f;

    [SerializeField] float spreadAngle = 15f;

    private PlayerAnimator playerAnimator;

    AudioManager audioManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        playerAnimator = playerRb.gameObject.GetComponent<PlayerAnimator>();
        audioManager = transform.parent.GetComponentInChildren<AudioManager>();

    }

    // Update is called once per frame
    void Update()
    {
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
        timeBetweenFire = cadence1;
        canFire = false;
        Vector2 recoilDir = -gun.right;
        playerRb.AddForce(recoilDir * recoilForce1, ForceMode2D.Impulse);
        
        Vector2 direction = gun.right;

        GameObject b = Instantiate(bullet, gun.position, Quaternion.identity);
        b.GetComponent<BulletScripte>().Init(direction, damage1);

        audioManager.PlayClipRand(audioManager.gun);
    }

    public void FireShotgun()
    {
        if(!canFire) return;
        timeBetweenFire = cadence2;
        canFire = false;
        Vector2 recoilDir = -gun.right;
        playerRb.AddForce(recoilDir * recoilForce2, ForceMode2D.Impulse);
        
        for (int i = 0; i < nbShotgun; i++)
        {
            float angle = Random.Range(-spreadAngle, spreadAngle);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * gun.right;

            GameObject b = Instantiate(bullet, gun.position, Quaternion.identity);
            b.GetComponent<BulletScripte>().Init(direction, damage2);
        }
        audioManager.PlayClipRand(audioManager.shotGun);
    }
}

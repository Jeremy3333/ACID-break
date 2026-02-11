using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int health;

    public LayerMask hitLayer;
    public Transform hitCheck;
    public Vector2 dim;
    public bool isHittable = true;

    private float timer;

    public float invincybilityTime = 0.6f;

    public Sprite brokenModule;
    public Sprite module;
    public Image[] moduleUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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

        if (isHittable && CheckHit())
        {
            isHittable = false;
            health = Mathf.Max(health-1, 0);
        }

        for (int i = 0; i < moduleUI.Length; i++)
        {
            if (health > i) moduleUI[i].sprite = module;
            else moduleUI[i].sprite = brokenModule;
        }
    }

    bool CheckHit()
    {
        return Physics2D.OverlapBox(
            hitCheck.position,
            dim,
            0,
            hitLayer
        );
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Vector3 dimmension = new Vector3(dim.x, dim.y, 0.0f);
        Gizmos.DrawWireCube(hitCheck.position, dimmension);
    }
}

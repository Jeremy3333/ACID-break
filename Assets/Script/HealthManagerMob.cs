using UnityEngine;
using UnityEngine.UI;

public class HealthManagerMob : MonoBehaviour
{
    [SerializeField]
    int MaxHealth;
    [SerializeField]
    int health;
    [SerializeField]
    EnemieHandler enemieHandler;

    bool isHittable = true;

    float timer;
    [SerializeField]
    float invincybilityTime = 0.6f;

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
    }

    public void Hit(int damage)
    {
        if (isHittable)
        {
            isHittable = false;
            health = Mathf.Max(health-damage, 0);
            if (health <= 0) Destroy(gameObject);
            enemieHandler.asDetected = true;
        }   
    }
}

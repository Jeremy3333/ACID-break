using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int health;
    public bool isHittable = true;

    private float timer;

    public float invincybilityTime = 0.6f;

    public Sprite brokenModule;
    public Sprite module;
    public Image[] moduleUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
    }

    public void Hit()
    {
        if(isHittable)
        {
            isHittable = false;
            health = Mathf.Max(health-1, 0);
        }
    }

    void UpDateUI()
    {
        for (int i = 0; i < moduleUI.Length; i++)
        {
            if (health > i) moduleUI[i].sprite = module;
            else moduleUI[i].sprite = brokenModule;
        }
    }
}

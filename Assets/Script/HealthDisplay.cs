using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public int health;
    public int maxHealth;

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
        for (int i = 0; i < moduleUI.Length; i++)
        {
            if (health > i) moduleUI[i].sprite = module;
            else moduleUI[i].sprite = brokenModule;
        }
    }
}

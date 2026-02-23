using UnityEngine;
using UnityEngine.SceneManagement;

public class PseudoKillPlane : MonoBehaviour
{
    public float killHeight = -20f;

    private HealthManager healthManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthManager = GetComponent<HealthManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < killHeight
            || healthManager.health == 0)
        {
            // Reload scene if dead
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

using UnityEngine;

public class PseudoKillPlane : MonoBehaviour
{
    public float killHeight = -20f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < killHeight)
        {
            GetComponent<HealthManager>().Respawn();
        }
    }
}

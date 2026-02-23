using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject mobPrefab;

    private GameObject currentMob;
    
    BoxCollider2D zoneCollider;
    float zoneMinX;
    float zoneMaxX;

    public void Start()
    {
        
        zoneCollider = GetComponentInParent<BoxCollider2D>();

        if (zoneCollider != null)
        {
            zoneMinX = zoneCollider.bounds.min.x;
            zoneMaxX = zoneCollider.bounds.max.x;
        }
    }

    // Fonction pour faire apparaître le mob
    public void SpawnMob()
    {
        // Si un mob existe déjà, on le détruit
        if (currentMob != null)
        {
            Destroy(currentMob);
        }

        // On instancie le prefab à la position du spawner
        currentMob = Instantiate(
            mobPrefab,
            transform.position,
            transform.rotation
        );

        float minX = zoneMinX;
        float maxX = zoneMaxX;

        // On transmet au mob
        currentMob.GetComponent<EnemieHandler>().SetPatrolBounds(zoneMinX, zoneMaxX);
    }
    public void Kill()
    {
        if (currentMob != null)
        {
            Destroy(currentMob);
        }
    }
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}

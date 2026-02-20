using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject mobPrefab;

    private GameObject currentMob;

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
    }
    public void Kill()
    {
        if (currentMob != null)
        {
            Destroy(currentMob);
        }
    }
}

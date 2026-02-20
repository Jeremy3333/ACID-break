using UnityEngine;

public class ZoneHandler : MonoBehaviour
{
    public Transform spawn;
    [SerializeField]
    GameObject[] mobSpawners;

    public void Set()
    {
        for (int i = 0; i < mobSpawners.Length; i++)
        {
            mobSpawners[i].GetComponent<MobSpawner>().SpawnMob();
        }
    }

    public void ReSet()
    {
        for (int i = 0; i < mobSpawners.Length; i++)
        {
            mobSpawners[i].GetComponent<MobSpawner>().Kill();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawn.position, 0.5f);
        for (int i = 0; i < mobSpawners.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(mobSpawners[i].transform.position, 0.5f);
        }
    }
}
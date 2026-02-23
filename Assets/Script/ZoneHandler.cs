using UnityEngine;
using System.Collections.Generic;

public class ZoneHandler : MonoBehaviour
{
    public Transform spawn;
    GameObject[] mobSpawners;

    private void Awake()
    {
        List<GameObject> spawners = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.CompareTag("SpawnerMob"))
            {
                spawners.Add(child.gameObject);
            }
        }

        mobSpawners = spawners.ToArray();
    }

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
        if (spawn != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawn.position, 0.5f);
        }
    }
}
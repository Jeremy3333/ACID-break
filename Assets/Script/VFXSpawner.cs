using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    public enum VFXEventType
    {
        Hit,
        Death
    }

    void OnEnable()
    {
        GameEvents.OnHit += HandleHit;
        GameEvents.OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        GameEvents.OnHit -= HandleHit;
        GameEvents.OnDeath -= HandleDeath;
    }

    void HandleHit(HitEvent e)
    {
        SpawnFromProfile(e.victim, VFXEventType.Hit, e.position, e.direction);
    }

    void HandleDeath(DeathEvent e)
    {
        SpawnFromProfile(e.victim, VFXEventType.Death, e.position, Vector2.right);
    }

    void SpawnFromProfile(GameObject victim, VFXEventType type, Vector2 pos, Vector2 dir)
    {
        if (victim == null)
            return;

        VFXProfile profile = victim.GetComponent<VFXProfile>();
        if (profile == null)
            return;

        GameObject prefab = profile.GetVFX(type);
        if (prefab == null)
            return;

        GameObject go = Instantiate(prefab, pos, Quaternion.identity);

        if (profile.rotateToDirection && dir.sqrMagnitude > 0.0001f)
        {
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            go.transform.rotation = Quaternion.Euler(0, 0, ang);
        }
    }
}
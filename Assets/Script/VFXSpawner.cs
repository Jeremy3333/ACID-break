using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    public enum VFXEventType
    {
        Hit,
        Death,
        Charge
    }

    void OnEnable()
    {
        GameEvents.OnHit += HandleHit;
        GameEvents.OnDeath += HandleDeath;
        GameEvents.OnCharge += HandleCharge;
    }

    void OnDisable()
    {
        GameEvents.OnHit -= HandleHit;
        GameEvents.OnDeath -= HandleDeath;
        GameEvents.OnCharge -= HandleCharge;
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

    private readonly System.Collections.Generic.Dictionary<GameObject, GameObject> activeChargeVFX = new System.Collections.Generic.Dictionary<GameObject, GameObject>();
    void HandleCharge(ChargeEvent e)
    {
        if (e.state == ChargeState.Started)
        {
            SpawnChargeVFX(e);
        }
        else if (e.state == ChargeState.Ended)
        {
            EndChargeVFX(e);
        }
    }

    void SpawnChargeVFX(ChargeEvent e)
    {
        if (e.victim == null) return;

        VFXProfile profile = e.victim.GetComponent<VFXProfile>();
        if (profile == null || profile.chargeVFX == null) return;

        GameObject go = Instantiate(profile.chargeVFX, e.position, Quaternion.identity, e.victim.transform);

        activeChargeVFX[e.victim] = go;
    }

    void EndChargeVFX(ChargeEvent e)
    {
        if (e.victim == null) return;

        if (activeChargeVFX.TryGetValue(e.victim, out GameObject go))
        {
            Destroy(go);
            activeChargeVFX.Remove(e.victim);
        }
    }
}
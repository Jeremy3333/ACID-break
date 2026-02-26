using UnityEngine;

public class VFXProfile : MonoBehaviour
{
    [Header("VFX References")]
    public GameObject hitVFX;
    public GameObject deathVFX;
    public GameObject chargeVFX;

    [Header("Rotation Settings")]
    public bool rotateToDirection = true;
    public GameObject GetVFX(VFXSpawner.VFXEventType type)
    {
        switch (type)
        {
            case VFXSpawner.VFXEventType.Hit:
                return hitVFX;

            case VFXSpawner.VFXEventType.Death:
                return deathVFX;

            case VFXSpawner.VFXEventType.Charge:
                return chargeVFX;

            default:
                return null;
        }
    }
}

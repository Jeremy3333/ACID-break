using UnityEngine;

public class AutoDestroyAfterAnim : MonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    [SerializeField]
    float fallbackLifetime = 0.5f;

    void Start() => Destroy(gameObject, fallbackLifetime);
}
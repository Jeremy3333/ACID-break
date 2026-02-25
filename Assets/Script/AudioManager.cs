using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSourceRand;
    public AudioClip walk;
    public AudioClip jump;
    public AudioClip hit;
    public AudioClip woosh;
    public AudioClip gun;
    public AudioClip shotGun;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void PlayClip(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlayClipRand(AudioClip clip)
    {
        audioSourceRand.pitch = Random.Range(0.8f, 1.2f);
        audioSourceRand.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
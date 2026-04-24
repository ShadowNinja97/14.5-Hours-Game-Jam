using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioClip[] clips;
    private AudioSource source;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
        source = GetComponent<AudioSource>();
    }

    public void PlayClip(int clip)
    {
        source.PlayOneShot(clips[clip]);
    }

}

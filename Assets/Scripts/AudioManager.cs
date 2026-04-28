using UnityEngine;

public class AudioManager : MonoBehaviour
{

    /*
    [0] = Node Activated
    [1] = Node Deactivated
    [2] = Door Open
    [3] = Door Unlocked
    [4] = Player Jump
    */


    public static AudioManager Instance;
    public AudioClip[] clips;
    public AudioSource mainSource;
    public AudioSource footstepSource;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
        mainSource = GetComponent<AudioSource>();
    }

    public void PlayClip(int clip)
    {
        mainSource.PlayOneShot(clips[clip]);
        if (clip == 4) footstepSource.Stop();
    }

    public void PlayFootsteps(float variance)
    {
        footstepSource.pitch = Random.Range(1 - variance, 1 + variance);
        footstepSource.Play();
    }

}

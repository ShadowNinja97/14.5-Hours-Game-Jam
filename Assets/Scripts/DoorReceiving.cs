using System.Collections;
using UnityEngine;

public class DoorReceiving : MonoBehaviour
{
    public void PlayAfterDelay(float seconds)
    {
        StartCoroutine(DelaySound(seconds));
    }
    IEnumerator DelaySound(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        AudioManager.Instance.PlayClip(2);
    }
}
